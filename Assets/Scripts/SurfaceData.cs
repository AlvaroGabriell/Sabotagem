using UnityEngine;

public class SurfaceData : MonoBehaviour
{
    public SurfaceType surfaceType;
}

public enum SurfaceType
{
    CONCRETE,
    METAL,
    DUCT,
}