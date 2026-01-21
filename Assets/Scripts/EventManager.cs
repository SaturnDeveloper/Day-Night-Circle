using System.Collections.Generic;
using UnityEngine;

// Manages world events and their effects
public class EventManager : MonoBehaviour
{
    // List of all available events
    public Event[] events;

    // Banner object shown during the Moon Festival
    public GameObject MoonBanner;

    // Called when the Moon Festival starts
    public void MoonFestivalEvent()
    {
        // Show the banner
        MoonBanner.SetActive(true);
    }

    // Called when the Moon Festival ends
    public void EndMoonFestivalEvent()
    {
        // Hide the banner
        MoonBanner.SetActive(false);
    }
}

