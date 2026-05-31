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

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(this);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }
}
