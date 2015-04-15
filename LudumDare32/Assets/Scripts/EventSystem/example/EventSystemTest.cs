using UnityEngine;
using System.Collections;

public class CTEvent1 : CTEvent
{
    public string message;
}

public class CTEvent2 : CTEvent
{
    public string message;
}

public class EventSystemTest : MonoBehaviour 
{
    public void Awake()
    {
        CTEventManager.AddListener<CTEvent1>(OnCallback1);
        CTEventManager.AddListener<CTEvent2>(OnCallback2);
    }

    public void OnDestroy()
    {
        CTEventManager.RemoveListener<CTEvent1>(OnCallback1);
        CTEventManager.RemoveListener<CTEvent2>(OnCallback2);
    }

	void Start () 
    {
        CTEventManager.FireEvent(new CTEvent1() { message = "message 1" });
        CTEventManager.FireEvent(new CTEvent2() { message = "message 2" });
        CTEventManager.FireEvent(new CTEvent1() { message = "message 3" });
        CTEventManager.FireEvent(new CTEvent2() { message = "message 4" });
	}


    public void OnCallback1(CTEvent1 eventData)
    {
        Debug.Log("callback 1: " + eventData.message);
    }

    public void OnCallback2(CTEvent2 eventData)
    {
        Debug.Log("callback 2: " + eventData.message);
    }
}
