using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelVisualEffects
{
    private static readonly WaitForSeconds _waitForSeconds0_15 = new(0.15f);
    private readonly MonoBehaviour owner;
    private readonly Dictionary<Material, MaterialState> flashState = new();
    private Coroutine flashCoroutine;

    public ModelVisualEffects(MonoBehaviour owner)
    {
        this.owner = owner;
    }

    private Renderer[] GetRenderers()
    {
        return owner.GetComponentsInChildren<Renderer>();
    }

    public void Refresh()
    {
        if(flashCoroutine != null)
        {
            owner.StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        flashState.Clear();
    }

    private void SaveCurrentState()
    {
        flashState.Clear();

        foreach (Renderer renderer in GetRenderers())
        {
            foreach (Material mat in renderer.materials)
            {
                flashState[mat] = new MaterialState
                {
                    BaseColor = mat.GetColor("_BaseColor")
                };
            }
        }
    }

    private void RestoreSavedtate()
    {
        foreach (Renderer renderer in GetRenderers())
        {
            foreach (Material mat in renderer.materials)
            {
                if (flashState.TryGetValue(mat, out var state))
                {
                    mat.SetColor("_BaseColor", state.BaseColor);
                }
            }
        }
    }

    // ----------------------------------------------------------

    public void FlashRed()
    {
        if (flashCoroutine != null)
        {
            owner.StopCoroutine(flashCoroutine);
            flashCoroutine = null;
            RestoreSavedtate();
        }

        flashCoroutine = owner.StartCoroutine(FlashRedRoutine());
    }

    private IEnumerator FlashRedRoutine()
    {
        SaveCurrentState();
        SetColor(new Color(1, 0.2745098f, 0.2745098f));
        yield return _waitForSeconds0_15;
        RestoreSavedtate();
        flashCoroutine = null;
    }

    private void SetColor(Color color)
    {
        foreach (Renderer renderer in GetRenderers())
        {
            foreach (Material mat in renderer.materials)
            {
                Color current = mat.GetColor("_BaseColor");
                color.a = current.a;
                mat.SetColor("_BaseColor", color);
            }
            
        }
    }

    // ----------------------------------------------------------

    public void SetTransparent(float alpha)
    {
        foreach (Renderer renderer in GetRenderers())
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetFloat("_Surface", 1);
                mat.SetFloat("_Blend", 0.0f);

                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

                mat.SetInt("_ZWrite", 0);

                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                Color color = mat.color;
                color.a = alpha;
                mat.SetColor("_BaseColor", color);
            }
        }
    }

    public void SetOpaque()
    {
        foreach (Renderer renderer in GetRenderers())
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetFloat("_Surface", 0.0f);

                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);

                mat.SetInt("_ZWrite", 1);

                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

                mat.renderQueue = -1;

                Color color = mat.GetColor("_BaseColor");
                color.a = 1f;
                mat.SetColor("_BaseColor", color);
            }
        }
    }

    private struct MaterialState
    {
        public Color BaseColor;
    }
}