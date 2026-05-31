using System;

// Usado pra marcar métodos que devem ser inscritos automaticamente no EventBus, com prioridade opcional.
// Listeners com prioridade mais alta são chamados primeiro.
[AttributeUsage(AttributeTargets.Method)]
public class SubscribeEventAttribute : Attribute
{
    public int Priority { get; }
    public SubscribeEventAttribute(int priority = 0)
    {
        Priority = priority;
    }
}

// Usado pra marcar classes que tem métodos com [SubscribeEvent] pra serem registrados automaticamente no EventBus.
[AttributeUsage(AttributeTargets.Class)]
public class EventBusSubscriberAttribute : Attribute {}