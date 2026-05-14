using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class LivingEntity : MonoBehaviour
{
    public Rigidbody Rb { get; private set; }
    public EntityAttributes Attributes { get; private set; }
    public HealthSystem Health { get; private set; }

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Attributes ??= CreateDefaultAttributes();
        Health = new HealthSystem(this, Attributes);
    }

    private EntityAttributes CreateDefaultAttributes()
    {
        EntityAttributes defaultAttributes = new(this);

        defaultAttributes.Get(AttributeType.maxHealth).SetBaseValue(20f);
        defaultAttributes.Get(AttributeType.moveSpeed).SetBaseValue(3f);
        defaultAttributes.Get(AttributeType.jumpForce).SetBaseValue(70f);
        defaultAttributes.Get(AttributeType.attackDamage).SetBaseValue(1f);

        // Outros atributos padrão. Não vão ser usados, mas é bom ter eles aqui pra referência e pro futuro.
        //defaultAttributes.Get(AttributeType.healthRegen).SetBaseValue(0f);
        //defaultAttributes.Get(AttributeType.regenSpeed).SetBaseValue(2f);
        //defaultAttributes.Get(AttributeType.attackSpeed).SetBaseValue(1.5f);
        //defaultAttributes.Get(AttributeType.criticalChance).SetPercentValue(5f);
        //defaultAttributes.Get(AttributeType.criticalMultiplier).SetBaseValue(2f);

        return defaultAttributes;
    }

    public void ApplyKnockback(Vector3 force)
    {
        Rb.AddForce(force, ForceMode.Impulse);
    }
}
