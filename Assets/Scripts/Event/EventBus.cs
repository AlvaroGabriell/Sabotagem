using System;
using System.Collections.Generic;

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