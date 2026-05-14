using UnityEngine;

public class HazardBehaviour : MonoBehaviour, IDamageSource
{
    public float GetDamage()
    {
        return 1;
    }
    public DamageSource GetDamageSource()
    {
        return DamageSource.ENVIRONMENT;
    }
}
