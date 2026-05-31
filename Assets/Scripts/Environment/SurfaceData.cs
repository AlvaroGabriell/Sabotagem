using System;
using UnityEngine;

public class SurfaceData : MonoBehaviour, IJumpableSurface
{
    public SurfaceInfo surfaceInfo = SurfaceInfo.Default;
}

[Serializable]
public struct SurfaceInfo
{
    public SurfaceType surfaceType;
    public bool isUnstable;

    public static SurfaceInfo Default => new()
    {
        surfaceType = SurfaceType.CONCRETE,
        isUnstable = false
    };
}

public enum SurfaceType
{
    CONCRETE,
    METAL,
    DUCT,
}