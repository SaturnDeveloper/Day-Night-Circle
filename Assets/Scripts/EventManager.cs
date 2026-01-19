using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public Event[] events;

    public GameObject MoonBanner;

    public void MoonFestivalEvent()
    {
        MoonBanner.SetActive(true);
    }

    public void EndMoonFestivalEvent()
    {
        MoonBanner.SetActive(false);
    }
}

