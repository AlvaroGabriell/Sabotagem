using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEditor;
using UnityEngine;

using StudioSystem = FMOD.Studio.System;
using StudioBank = FMOD.Studio.Bank;
using StudioEventDescription = FMOD.Studio.EventDescription;
using Debug = UnityEngine.Debug;
using INITFLAGS = FMOD.Studio.INITFLAGS;

public static class FMODAudioRegistryGenerator
{
    private const string OutputPath = "Assets/Generated/Scripts/AudioEvents.cs";

    private static readonly HashSet<string> Acronyms = new(StringComparer.OrdinalIgnoreCase){"sfx", "ui", "vfx", "bgm", "npc", "fx"};

    private static readonly HashSet<string> CSharpKeywords = new(StringComparer.Ordinal)
    {
        "class", "event", "namespace", "public", "static", "string", "int", "new", "return",
        "void", "null", "true", "false", "for", "foreach", "while", "if", "else", "switch",
        "case", "default", "using", "struct", "enum", "interface", "private", "protected",
        "internal", "readonly", "const", "this", "base", "get", "set", "add", "remove", "var"
    };
    
    [MenuItem("Tools/FMOD/Generate Audio Registry")]
    public static void Generate()
    {
        if (Settings.Instance == null)
        {
            Debug.LogError("FMOD Settings.Instance is null.");
            return;
        }

        var bankFolder = Path.GetFullPath(
            Path.Combine(Application.dataPath, "..", Settings.Instance.SourceBankPath));

        if (!Directory.Exists(bankFolder))
        {
            Debug.LogError($"FMOD bank folder not found: {bankFolder}");
            return;
        }

        StudioSystem studio = default;
        var loadedBanks = new List<StudioBank>();

        try
        {
            if (StudioSystem.create(out studio) != RESULT.OK)
            {
                Debug.LogError("Could not create FMOD StudioSystem.");
                return;
            }

            if (studio.initialize(256, INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero) != RESULT.OK)
            {
                Debug.LogError("Could not initialize FMOD StudioSystem.");
                return;
            }

            foreach (var bankPath in Directory
                         .GetFiles(bankFolder, "*.bank", SearchOption.AllDirectories)
                         .OrderBy(p => p.EndsWith(".strings.bank", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                         .ThenBy(p => p, StringComparer.OrdinalIgnoreCase))
            {
                if (studio.loadBankFile(bankPath, LOAD_BANK_FLAGS.NORMAL, out StudioBank bank) == RESULT.OK)
                    loadedBanks.Add(bank);
            }

            var paths = CollectEventPaths(studio);
            var tree = BuildTree(paths);
            var code = EmitCode(tree);

            var fullOutputPath = Path.GetFullPath(OutputPath);
            var dir = Path.GetDirectoryName(fullOutputPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(fullOutputPath, code, new UTF8Encoding(false));
            AssetDatabase.Refresh();

            Debug.Log($"Generated {paths.Count} FMOD events into {OutputPath}");
        }
        finally
        {
            foreach (var bank in loadedBanks)
            {
                if (bank.isValid())
                    bank.unload();
            }

            if (studio.isValid())
                studio.release();
        }
    }

    private static List<string> CollectEventPaths(StudioSystem studio)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);

        if (studio.getBankList(out StudioBank[] banks) != RESULT.OK)
            return new List<string>();

        foreach (var bank in banks)
        {
            if (!bank.isValid())
                continue;

            if (bank.getEventList(out StudioEventDescription[] events) != RESULT.OK)
                continue;

            foreach (var ev in events)
            {
                if (!ev.isValid())
                    continue;

                if (ev.getPath(out string path) != RESULT.OK)
                    continue;

                if (!string.IsNullOrWhiteSpace(path) && path.StartsWith("event:/", StringComparison.OrdinalIgnoreCase))
                    result.Add(path.Trim());
            }
        }

        return result.OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static Node BuildTree(IEnumerable<string> eventPaths)
    {
        var root = new Node();

        foreach (var fullPath in eventPaths)
        {
            var relative = StripEventPrefix(fullPath);
            var parts = relative
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => p.Length > 0)
                .ToArray();

            if (parts.Length == 0)
                continue;

            var node = root;

            for (int i = 0; i < parts.Length - 1; i++)
                node = node.GetOrCreateChild(parts[i]);

            node.AddField(parts[^1], fullPath);
        }

        return root;
    }

    private static string EmitCode(Node root)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated>");
        sb.AppendLine("// Generated by FMODAudioRegistryGenerator.");
        sb.AppendLine("// Do not edit by hand.");
        sb.AppendLine("// </auto-generated>");
        sb.AppendLine();
        sb.AppendLine("public static class AudioEvents");
        sb.AppendLine("{");

        EmitNode(sb, root, 1);

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static void EmitNode(StringBuilder sb, Node node, int indent)
    {
        foreach (var child in node.Children.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase))
        {
            AppendIndent(sb, indent);
            sb.AppendLine($"public static class {child.Name}");
            AppendIndent(sb, indent);
            sb.AppendLine("{");

            EmitNode(sb, child.Node, indent + 1);

            AppendIndent(sb, indent);
            sb.AppendLine("}");
            sb.AppendLine();
        }

        foreach (var field in node.Fields.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase))
        {
            AppendIndent(sb, indent);
            sb.AppendLine($"public const string {field.Name} = \"{Escape(field.Path)}\";");
        }
    }

    private static string StripEventPrefix(string path)
    {
        const string prefix = "event:/";
        return path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? path.Substring(prefix.Length)
            : path;
    }

    private static string Escape(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }

    private static void AppendIndent(StringBuilder sb, int indent) =>
        sb.Append(new string(' ', indent * 4));

    private static string ToIdentifier(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "Unnamed";

        var normalized = raw.Normalize(NormalizationForm.FormD);
        var cleaned = new StringBuilder();

        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark)
                continue;

            cleaned.Append(char.IsLetterOrDigit(ch) ? ch : ' ');
        }

        var tokens = cleaned.ToString()
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length == 0)
            return "Unnamed";

        var result = new StringBuilder();

        foreach (var token in tokens)
        {
            if (Acronyms.Contains(token.ToLowerInvariant()))
            {
                result.Append(token.ToUpperInvariant());
                continue;
            }
            
            result.Append(char.ToUpperInvariant(token[0]));

            if(token.Length > 1)
                result.Append(token[1..]);
        }

        var id = result.Length > 0 ? result.ToString() : "Unnamed";

        if (char.IsDigit(id[0]))
            id = "_" + id;

        if (CSharpKeywords.Contains(id))
            id += "_";

        return id;
    }

    private sealed class Node
    {
        public string OwnName { get; }

        private readonly Dictionary<string, ChildEntry> _childrenByRaw = new(StringComparer.Ordinal);
        private readonly Dictionary<string, FieldEntry> _fieldsByRaw = new(StringComparer.Ordinal);

        public List<ChildEntry> Children { get; } = new();
        public List<FieldEntry> Fields { get; } = new();
        private HashSet<string> UsedNames { get; } = new(StringComparer.Ordinal);

        public Node(string name = null)
        {
            OwnName = name;
        }

        public Node GetOrCreateChild(string raw)
        {
            if (_childrenByRaw.TryGetValue(raw, out var existing))
                return existing.Node;

            var name = MakeUnique(ToIdentifier(raw));
            var node = new Node(name);
            var entry = new ChildEntry(name, node);

            _childrenByRaw[raw] = entry;
            Children.Add(entry);

            return node;
        }

        public void AddField(string raw, string path)
        {
            if (_fieldsByRaw.ContainsKey(raw))
                return;

            var name = ToIdentifier(raw);

            if(name == OwnName)
                name += "Event";

            name = MakeUnique(name);

            var entry = new FieldEntry(name, path);

            _fieldsByRaw[raw] = entry;
            Fields.Add(entry);
        }

        private string MakeUnique(string baseName)
        {
            var name = baseName;
            var i = 2;

            while (!UsedNames.Add(name))
            {
                name = $"{baseName}_{i}";
                i++;
            }

            return name;
        }
    }

    private sealed class ChildEntry
    {
        public string Name { get; }
        public Node Node { get; }

        public ChildEntry(string name, Node node)
        {
            Name = name;
            Node = node;
        }
    }

    private sealed class FieldEntry
    {
        public string Name { get; }
        public string Path { get; }

        public FieldEntry(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
