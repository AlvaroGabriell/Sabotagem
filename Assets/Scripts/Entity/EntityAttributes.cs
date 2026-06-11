using System;
using System.Collections.Generic;

public class EntityAttributes
{
    private Dictionary<AttributeType, ScalableAttribute> attributes;

    public LivingEntity Owner { get; }

    public EntityAttributes(LivingEntity owner)
    {
        Owner = owner;
        attributes = new Dictionary<AttributeType, ScalableAttribute>();
    }

    public ScalableAttribute Get(AttributeType type)
    {
        if(!attributes.TryGetValue(type, out ScalableAttribute attr))
        {
            attr = new ScalableAttribute(Owner, type);
            attributes[type] = attr;
        }

        return attr;
    }

    public Dictionary<AttributeType, ScalableAttribute> GetAttributeDictionary()
    {
        return attributes;
    }
}

public class ScalableAttribute
{
    public LivingEntity Owner { get; }
    public AttributeType Type { get; }

    private float baseValue = 1;
    private float modifier = 1f; // 1 = 100%

    public float FinalValue => baseValue * modifier;

    public ScalableAttribute(LivingEntity owner, AttributeType type)
    {
        Owner = owner;
        Type = type;
    }

    public void ApplyBaseUpgrade(float pValue)
    {
        var attributeEvent = new AttributeEvents.BaseValue.ApplyUpgrade()
        {
            target = Owner,
            attributeType = Type,
            value = pValue,
        };

        EventBus<AttributeEvents.BaseValue.ApplyUpgrade>.Publish(attributeEvent);

        if(attributeEvent.Canceled) return;

        baseValue += attributeEvent.value;

        PublishFinalValueEvent();
    }

    public void SetBaseValue(float pValue)
    {
        var attributeEvent = new AttributeEvents.BaseValue.SetValue()
        {
            target = Owner,
            attributeType = Type,
            value = pValue,
        };

        EventBus<AttributeEvents.BaseValue.SetValue>.Publish(attributeEvent);

        if(attributeEvent.Canceled) return;

        baseValue = attributeEvent.value;

        PublishFinalValueEvent();
    }

    public float GetBaseValue()
    {
        return baseValue;
    }

    public void ApplyPercentUpgrade(float pPercent)
    {
        var attributeEvent = new AttributeEvents.PercentValue.ApplyUpgrade()
        {
            target = Owner,
            attributeType = Type,
            percent = pPercent,
        };

        EventBus<AttributeEvents.PercentValue.ApplyUpgrade>.Publish(attributeEvent);

        if(attributeEvent.Canceled) return;

        modifier *= 1f + (attributeEvent.percent / 100f);

        PublishFinalValueEvent();
    }

    public void SetPercentValue(float pPercent)
    {
        var attributeEvent = new AttributeEvents.PercentValue.SetValue()
        {
            target = Owner,
            attributeType = Type,
            percent = pPercent,
        };

        EventBus<AttributeEvents.PercentValue.SetValue>.Publish(attributeEvent);

        if(attributeEvent.Canceled) return;

        modifier = attributeEvent.percent / 100f;

        PublishFinalValueEvent();
    }

    public float GetPercentValue()
    {
        return modifier * 100f;
    }

    private void PublishFinalValueEvent()
    {
        var finalValueEvent = new AttributeEvents.FinalValue()
        {
            target = Owner,
            attributeType = Type,
            finalValue = FinalValue,
            baseValue = baseValue,
            modifier = modifier,
        };

        EventBus<AttributeEvents.FinalValue>.Publish(finalValueEvent);
    }
}

public enum AttributeType
{
    maxHealth,
    healthRegen,
    regenSpeed,
    moveSpeed,
    jumpForce,
    attackDamage,
    attackSpeed,
    criticalChance,
    criticalMultiplier,
    invulnerabilityTime,
}