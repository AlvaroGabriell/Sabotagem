using UnityEngine;

public static class Utils
{
    public static (Vector3 center, float radius) GetVisualBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if(renderers.Length == 0) return (obj.transform.position, 0f);

        Bounds bounds = renderers[0].bounds;
        foreach(var r in renderers) bounds.Encapsulate(r.bounds);

        return (bounds.center, bounds.extents.magnitude);
    }

    public static bool TryGetPlayers(out GameObject[] players)
    {
        var ps = GameObject.FindGameObjectsWithTag("Player");
        if (ps.Length <= 0)
        {
            players = new GameObject[0];
            return false;
        }

        players = ps;
        return true;
    }

}