using UnityEngine;

public static class Utils
{
    public static Vector3 GetVisualCenter(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if(renderers.Length == 0) return obj.transform.position;

        Bounds bounds = renderers[0].bounds;

        foreach(var r in renderers) bounds.Encapsulate(r.bounds);

        return bounds.center;
    }
}
