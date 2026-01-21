using System;
using UnityEngine;
using Action = System.Action;

// Handles in-game time, days, months, and events
public class TimeService
{
    // Settings for time speed and calendar
    readonly TimeSettings settings;

    // Current in-game time
    DateTime currentTime;

    // Current date values
    int month = 0;
    int day = 0;
    int previousHour = -1;

    // Sunrise and sunset times
    readonly TimeSpan sunriseTime;
    readonly TimeSpan sunsetTime;

    // Current month name
    public string ThisMonth = "Spring";

    // Public access to time and day
    public DateTime CurrentTime => currentTime;
    public int Day => day;

    // Time-related events
    public event Action OnSunrise = delegate { };
    public event Action OnSunset = delegate { };
    public event Action OnHourChange = delegate { };
    public event Action<int> OnDayChanged;

    // Observable values
    readonly Observable<bool> isDayTime;
    readonly Observable<int> currentHour;
    readonly Observable<int> currentDay;
    readonly Observable<int> currentMonth;

    // Reference to the EventManager
    EventManager eventManager;

    // Constructor
    public TimeService(TimeSettings settings, EventManager eventManager)
    {
        this.settings = settings;
        this.eventManager = eventManager;

        // Set starting time
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(settings.startHour);

        sunriseTime = TimeSpan.FromHours(settings.sunriseHour);
        sunsetTime = TimeSpan.FromHours(settings.sunsetHour);

        // Initialize observables
        isDayTime = new Observable<bool>(IsDayTime());
        currentHour = new Observable<int>(currentTime.Hour);
        currentDay = new Observable<int>(day);
        currentMonth = new Observable<int>(month);

        // Check events when the day changes
        OnDayChanged += (day) => CheckEventsForDay(day, ThisMonth);

        // Fire sunrise / sunset events
        isDayTime.ValueChanged += d => (d ? OnSunrise : OnSunset)?.Invoke();

        // Fire hour change event
        currentHour.ValueChanged += _ => OnHourChange?.Invoke();
    }

    // Updates the current time
    public void UpdateTime(float deltaTime)
    {
        currentTime = currentTime.AddSeconds(deltaTime * settings.timeMultiplier);
        isDayTime.Value = IsDayTime();
        currentHour.Value = currentTime.Hour;
    }

    // Checks if a new day has started
    public void UpdateDay()
    {
        if (CurrentTime.Hour == 0 && previousHour != 0)
        {
            day++;

            // Change month if needed
            if (day > settings.daysPerMonth)
            {
                day = 1;
                currentMonth.Value = (currentMonth.Value + 1) % settings.months.Length;
                ThisMonth = settings.months[currentMonth.Value];
            }

            currentDay.Value = day;

            // Trigger day changed event
            OnDayChanged?.Invoke(day);

            Debug.Log($"New day: {day}, Month: {ThisMonth}");
        }

        previousHour = CurrentTime.Hour;
    }

    // Checks and updates the month (legacy / optional)
    public void UpdateMonth()
    {
        if (day == settings.daysPerMonth + 1)
        {
            day = 0;
            currentDay.Value = day;

            if (currentMonth.Value != settings.months.Length - 1)
                currentMonth.Value++;
            else
                currentMonth.Value = 0;

            ThisMonth = settings.months[currentMonth.Value];
        }
    }

    // Checks which events are active on the current day
    void CheckEventsForDay(int day, string thisMonth)
    {
        Debug.Log($"Day changed: {day}. Checking events...");

        foreach (var e in eventManager.events)
        {
            if (e == null) continue;

            if (day >= e.startDay && day <= e.endDay && thisMonth == e.month)
                e.eventFunction?.Invoke();
            else
                e.endEventFunction?.Invoke();
        }
    }

    // Calculates the sun rotation angle
    public float CalculateSunAngle()
    {
        bool isDay = IsDayTime();

        float startDegree = isDay ? 0 : 180;
        TimeSpan start = isDay ? sunriseTime : sunsetTime;
        TimeSpan end = isDay ? sunsetTime : sunriseTime;

        TimeSpan totalTime = CalculateDifference(start, end);
        TimeSpan elapsedTime = CalculateDifference(start, currentTime.TimeOfDay);

        double percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;
        return Mathf.Lerp(startDegree, startDegree + 180, (float)percentage);
    }

    // Returns true if it is daytime
    bool IsDayTime()
    {
        return currentTime.TimeOfDay > sunriseTime &&
               currentTime.TimeOfDay < sunsetTime;
    }

    // Calculates time difference, supports day wrap
    TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
    {
        TimeSpan difference = to - from;
        return difference.TotalHours < 0
            ? difference + TimeSpan.FromHours(24)
            : difference;
    }
}
