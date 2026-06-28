using System;
using System.Collections.Generic;
using System.Reflection;

public static class EventBus<T>
{
    private static readonly List<EventListener<T>> listeners = new();

    // Método pra inscrever um listener para um evento do tipo T, com prioridade opcional.
    // Listeners com prioridade mais alta são chamados primeiro.
    public static void Subscribe(Action<T> callback, int priority = 0)
    {
        listeners.Add(new EventListener<T> { callback = callback, Priority = priority });
        // Ordena os listeners por prioridade logo na inscrição pro Publish ser mais eficiente.
        listeners.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    public static void Unsubscribe(Action<T> callback)
    {
        listeners.RemoveAll(l => l.callback == callback);
    }

    public static void Publish(T evt){
        // Cria uma cópia da lista de listeners pra evitar problemas se a lista for modificada durante o publish.
        var listenersCopy = new List<EventListener<T>>(listeners);
        foreach (var listener in listenersCopy)
        {
            // Se o evento for cancelável e tiver sido cancelado, para de chamar os listeners.
            if(evt is ICancelableEvent {Canceled: true}) break;

            listener.callback?.Invoke(evt);
        }
    }
}

public class EventListener<T>
{
    public Action<T> callback;
    public int Priority;
}

public static class EventBusAutoSubscriber
{
    private static readonly List<(Type eventBusType, object del)> registeredDelegates = new();

    public static void ScanAndSubscribe()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if(type.GetCustomAttribute<EventBusSubscriberAttribute>() == null) continue;

                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var subscribeAttr = method.GetCustomAttribute<SubscribeEventAttribute>();
                    if( subscribeAttr == null) continue;

                    var parameters = method.GetParameters();
                    if(parameters.Length != 1) continue;

                    var eventType = parameters[0].ParameterType;

                    var eventBusType = typeof(EventBus<>).MakeGenericType(eventType);
                    var subscribeMethod = eventBusType.GetMethod("Subscribe");

                    var delegateType = typeof(Action<>).MakeGenericType(eventType);
                    var del = Delegate.CreateDelegate(delegateType, method);

                    subscribeMethod.Invoke(null, new object[] { del, subscribeAttr.Priority });

                    registeredDelegates.Add((eventBusType, del));
                }
            }
        }
    }

    public static void UnsubscribeAll()
    {
        foreach(var (eventBusType, del) in registeredDelegates)
        {
            var unsubscribeMethod = eventBusType.GetMethod("Unsubscribe");
            unsubscribeMethod.Invoke(null, new object[] { del} );
        }
        registeredDelegates.Clear();
    }
}