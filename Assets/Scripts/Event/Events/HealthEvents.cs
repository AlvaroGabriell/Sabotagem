public static class HealthEvents
{
    public class DamageEvent : ICancelableEvent
    {
        public bool Canceled { get; set; }
        public float damage;
        public DamageSource source;
        public LivingEntity target;
        public LivingEntity attacker;
    }

    public class DeathEvent : ICancelableEvent
    {
        public bool Canceled { get; set; }
        public DamageSource source;
        public LivingEntity target;
        public LivingEntity attacker;
    }

    public class ReviveEvent : ICancelableEvent
    {
        public bool Canceled { get; set; }
        public LivingEntity target;
    }

    public class HealEvent : ICancelableEvent
    {
        public bool Canceled { get; set; }
        public float healing;
        public LivingEntity target;
    }
}
