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
    public string footstepSound;
    public string landingSound;
    public bool isUnstable;

    public static SurfaceInfo Default => new()
    {
        surfaceType = SurfaceType.CONCRETE,
        footstepSound = AudioEvents.SFX.Steps.Concrete,
        landingSound = AudioEvents.SFX.JumpAction.Land.Concrete,
        isUnstable = false
    };
}

public enum SurfaceType
{
    CONCRETE,
    METAL,
    DUCT,
}