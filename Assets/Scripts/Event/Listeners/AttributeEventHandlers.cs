[EventBusSubscriber]
public static class AttributeEventHandlers
{
    [SubscribeEvent]
    private static void OnAttributeFinalValue(AttributeEvents.FinalValue evt)
    {
        if(evt.attributeType != AttributeType.maxHealth || evt.target.Health == null) return;

        evt.target.Health.SetMaxHealth(evt.finalValue);
    }
}
