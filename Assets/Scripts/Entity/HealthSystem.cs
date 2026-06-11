using UnityEngine;

/** <summary>
* Script da vida. Seta um valor de vida máximo para o objeto, com variáveis pra controlar se
* o objeto pode tomar dano, pode morrer e se está vivo. Tem também métodos pra mudar a vida,
* pegar, curar e dar dano. 
* </summary> **/
public class HealthSystem
{
    [Header("Health")]
    private float maxHealth = 20f;
    private float health; 
    public bool canDie = true, canRegen = true, isAlive = true, isInvulnerable = false;

    public LivingEntity Owner { get; private set; }
    private EntityAttributes attributes;
    private DamageFlash damageFlash;

    private float regenTimer = 0f;
    
    public HealthSystem(LivingEntity owner, EntityAttributes attributes)
    {
        Owner = owner;
        this.attributes = attributes;
        damageFlash = new DamageFlash(owner);
        maxHealth = attributes.Get(AttributeType.maxHealth).FinalValue;
        health = maxHealth;
    }

    public void SetMaxHealth(float pMaxHealth)
    {
        maxHealth = pMaxHealth;
    }
    public void SetMaxHealthAndFullHeal(float pMaxHealth)
    {
        maxHealth = pMaxHealth;
        HealFullHealth();
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetHealth(float pHealth)
    {
        health = pHealth;
    }

    public void TakeDamage(float pDamage, DamageSource pSource, LivingEntity pAttacker = null)
    {
        if (isInvulnerable || !isAlive) return;

        var damageEvent = new HealthEvents.DamageEvent()
        {
            damage = pDamage,
            source = pSource,
            target = Owner,
            attacker = pAttacker,
        };

        EventBus<HealthEvents.DamageEvent>.Publish(damageEvent);

        if(damageEvent.Canceled) return;

        health = Mathf.Max(health - damageEvent.damage, 0);

        damageFlash.Flash();

        if (ShouldDie() && canDie == true) Die(damageEvent.source, damageEvent.attacker);
    }

    public void Kill(DamageSource source)
    {
        if(!isAlive) return;

        Die(source);
    }

    private void Die(DamageSource pSource, LivingEntity pAttacker = null)
    {
        var deathEvent = new HealthEvents.DeathEvent()
        {
            source = pSource,
            target = Owner,
            attacker = pAttacker,
        };

        EventBus<HealthEvents.DeathEvent>.Publish(deathEvent);

        if(deathEvent.Canceled) return;
        
        isAlive = false;
    }

    public void Revive()
    {
        var reviveEvent = new HealthEvents.ReviveEvent()
        {
            target = Owner,
        };

        EventBus<HealthEvents.ReviveEvent>.Publish(reviveEvent);

        if(reviveEvent.Canceled) return;

        isAlive = true;
        HealFullHealth();
    }

    public void HealHealth(float pHealing)
    {
        var healingEvent = new HealthEvents.HealEvent()
        {
            healing = pHealing,
            target = Owner,
        };

        EventBus<HealthEvents.HealEvent>.Publish(healingEvent);

        if(healingEvent.Canceled) return;

        health = Mathf.Min(health + healingEvent.healing, maxHealth);
    }
    public void HealFullHealth()
    {
        HealHealth(maxHealth);
    }

    public float GetHealth()
    {
        return health;
    }

    public bool ShouldDie()
    {
        return health <= 0;
    }

    public bool ShouldRegen()
    {
        return health < maxHealth && canRegen && isAlive && !isInvulnerable;
    }
    
    private void HandleRegen(float deltaTime)
    {
        if(!ShouldRegen()) return;

        regenTimer += deltaTime;

        float regenInterval = Mathf.Max(4f / attributes.Get(AttributeType.regenSpeed).FinalValue, 0f);

        if (regenTimer >= regenInterval)
        {
            HealHealth(attributes.Get(AttributeType.healthRegen).FinalValue);
            regenTimer = 0f;
        }
    }
}

public enum DamageSource
{
    PLAYER,
    ENEMY,
    ENVIRONMENT,
    SELF,
    VOID
}