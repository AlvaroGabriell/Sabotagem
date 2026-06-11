using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash
{
    private readonly MonoBehaviour owner;
    private readonly Renderer[] renderers;
    private readonly Dictionary<Material, Color> originalColors = new();
    private Coroutine flashCoroutine;

    public DamageFlash(MonoBehaviour owner)
    {
        this.owner = owner;
        renderers = owner.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat.HasProperty("_BaseColor"))
                {
                    originalColors[mat] = mat.GetColor("_BaseColor");
                }
            }
        }
    }

    public void Flash()
    {
        if (flashCoroutine != null)
        {
            owner.StopCoroutine(flashCoroutine);
            flashCoroutine = null;
            ResetColors();
        }

        flashCoroutine = owner.StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        SetColor(new Color(1, 0.2745098f, 0.2745098f));
        yield return new WaitForSeconds(0.15f);
        ResetColors();
        flashCoroutine = null;
    }

    private void SetColor(Color color)
    {
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetColor("_BaseColor", color);
            }
            
        }
    }

    private void ResetColors()
    {
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetColor("_BaseColor", originalColors[mat]);
            }
        }
    }
}
