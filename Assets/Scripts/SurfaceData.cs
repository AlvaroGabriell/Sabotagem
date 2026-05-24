using UnityEngine;

public class SurfaceData : MonoBehaviour
{
    public SurfaceType surfaceType = default;
    public bool isSafe = false;
}

public enum SurfaceType
{
    CONCRETE,
    METAL,
    DUCT,
}