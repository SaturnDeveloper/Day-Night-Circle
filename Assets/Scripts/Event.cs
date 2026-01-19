using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Event
{
    public string name;
    public int startDay;
    public int endDay;
    public string month;
    public UnityEvent eventFunction;
    public UnityEvent endEventFunction;
}
