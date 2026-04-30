public static class AttributeEvents
{
    public static class BaseValue
    {
        public class ApplyUpgrade : AttributeEventBase
        {
            public float value;
        }

        public class SetValue : AttributeEventBase
        {
            public float value;
        }
    }

    public static class PercentValue
    {
        public class ApplyUpgrade : AttributeEventBase
        {
            public float percent;
        }

        public class SetValue : AttributeEventBase
        {
            public float percent;
        }
    }

    public class FinalValue : AttributeEventBase
    {
        public float finalValue;
        public float baseValue;
        public float modifier;
    }
}

public abstract class AttributeEventBase : ICancelableEvent
{
    public bool Canceled { get; set; }
    public LivingEntity target;
    public AttributeType attributeType;
}
