public static class AttributeEventHandler
{
    private static bool initialized = false;

    public static void Init()
    {
        if (initialized) return;

        EventBus<AttributeEvents.FinalValue>.Subscribe(OnAttributeFinalValue);

        initialized = true;
    }

    public static void Dispose()
    {
        EventBus<AttributeEvents.FinalValue>.Unsubscribe(OnAttributeFinalValue);

        initialized = false;
    }

    private static void OnAttributeFinalValue(AttributeEvents.FinalValue evt)
    {
        if(evt.attributeType != AttributeType.maxHealth) return;

        evt.target.Health.SetMaxHealth(evt.finalValue);
    }
}
