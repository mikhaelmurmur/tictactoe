using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager :  Singleton<EventManager>
{
    public delegate void delegateMethod(object[] arg);
    Dictionary<string, List<delegateMethod>> delegateList = new Dictionary<string, List<delegateMethod>>();

    public enum events
    {
        CellTaped,
        GameEnded
    }

    public void Add(events name, delegateMethod functionToCall)
    {
        if(delegateList.ContainsKey(name.ToString()))
        {
            delegateList[name.ToString()].Add(functionToCall);
        }
        else
        {
            delegateList[name.ToString()] = new List<delegateMethod>();
            delegateList[name.ToString()].Add(functionToCall);
        }
    }

    public void Remove(events name, delegateMethod functionToRemove)
    {
        if(delegateList.ContainsKey(name.ToString()))
        {
            delegateList[name.ToString()].Remove(functionToRemove);
        }
    }


    public void Call(events callEvent, object[] args)
    {
        if(delegateList.ContainsKey(callEvent.ToString()))
        {
            foreach(delegateMethod method in delegateList[callEvent.ToString()])
            {
                method.Invoke(args);
            }
        }
    }

}
