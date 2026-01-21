using System;
using UnityEngine.Events;

// Simple, serializable event
[Serializable]
public class Event
{
    // Event name
    public string name;

    // Start day of the event
    public int startDay;

    // End day of the event
    public int endDay;

    // Month of the event
    public string month;

    // Called when the event starts
    public UnityEvent eventFunction;

    // Called when the event ends
    public UnityEvent endEventFunction;
}