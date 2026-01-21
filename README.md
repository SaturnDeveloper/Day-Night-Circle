# Unity Time and Event System
![Screenshot](Screenshots/Screenshot%202026-01-19%20190200.png)
## Overview
This project is a time- and event-driven system for Unity. 

### It provides
- Dynamic day/night cycle with sun and moon lighting
- Day and Month tracking
- Events that automatically start and end
- Easy integration into existing projects

## Features 
### TimeService
- Manages the in-game time
- Calculates sun rotation angles for lighting
- Handles day and month changes
- Triggers events on day change, sunrise and sunset
### EventManager
- Starts events based on day and month

![Screenshot](Screenshots/Screenshot%2026-01-21%172748.png)

### Wheat Growth System
Demonstrates time-driven gameplay using the TimeManager:
- Wheat has three states: Seeded, Growing, Mature
- Growth progresses daily or hourly, depending on settings
- Visuals automatically switch based on state
- Uses TimeManager events (OnSunrise or OnHourChange)
### Vertext Horizon Shader 
- Supports stylized vertex-based horizon shading (like Animal Crossing) 

## Usage 
### Day/Month Cycle
- Time progresses automatically based on TimeSettings.timeMultiplier
- Sun and moon lighting adjusts dynamically
- UI is updated automatically
### Events 

Define an event in the EventManager: 
```csharp
[System.Serializable]
public class Event
{
    public string month;              // Month the event occurs in
    public int startDay;              // Event start day (0-based)
    public int endDay;                // Event end day (0-based)
    public Action eventFunction;      // Function to start the event
    public Action endEventFunction;   // Function to end the event
}
```
Example: Moon Festival Event
```csharp
Event moonFestival = new Event
{
    month = "Winter",
    startDay = 0,
    endDay = 1,
    eventFunction = eventManager.MoonFestivalEvent,
    endEventFunction = eventManager.EndMoonFestivalEvent
};
```
- CheckEventsForDay(day, month) is auotmatically called by the TimeService each day. 
