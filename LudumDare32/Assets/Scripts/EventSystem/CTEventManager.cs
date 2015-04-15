using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void CTEventCallBack<T>(T eventData) where T : CTEvent;

public class CTEventManager : MonoBehaviour 
{
    private static Dictionary<System.Type, List<System.Delegate>> eventDictionary = new Dictionary<System.Type, List<System.Delegate>>();

    public static void AddListener<T>(CTEventCallBack<T> callback) where T : CTEvent
    {
        if (eventDictionary.ContainsKey(typeof(T)))
        {
            foreach(KeyValuePair<System.Type, List<System.Delegate>> callBackDefinition in eventDictionary)
            {
                if (callBackDefinition.Key == typeof(T))
                {
                    callBackDefinition.Value.Add(callback);
                }
            }
        }
        else
        {
            List<System.Delegate>  callbacks = new List<System.Delegate>();
            callbacks.Add(callback);
            eventDictionary.Add(typeof(T), callbacks);
        }

    }

    public static void RemoveListener<T>(CTEventCallBack<T> callback) where T : CTEvent
    {
        if (eventDictionary.ContainsKey(typeof(T)))
        {
            foreach (KeyValuePair<System.Type, List<System.Delegate>> callBackDefinition in eventDictionary)
            {
                if (callBackDefinition.Key == typeof(T))
                {
                    callBackDefinition.Value.Remove(callback);
                }
            }
        }
    }

    public static void FireEvent(CTEvent ctEvent)
    {
        if (eventDictionary.ContainsKey(ctEvent.GetType()))
        {
            foreach (KeyValuePair<System.Type, List<System.Delegate>> callBackDefinition in eventDictionary)
            {
                if (callBackDefinition.Key == ctEvent.GetType())
                {
                    foreach (System.Delegate callback in callBackDefinition.Value)
                    {
                        callback.DynamicInvoke(ctEvent);
                    }
                }
            }
        }
    }
}
