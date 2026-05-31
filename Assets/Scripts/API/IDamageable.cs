public interface IDamageable
{
    void TakeDamage(IDamageSource source, LivingEntity pAttacker = null);
}
