using System;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using Action = System.Action;
public class TimeService
{
    readonly TimeSettings settings;
    DateTime currentTime;
    int month = 0;
    int day = 0;
    int previousHour = -1;
    readonly TimeSpan sunriseTime;
    readonly TimeSpan sunsetTime;

    public String ThisMonth = "Spring";
    public DateTime CurrentTime => currentTime;
    public int Day => day;

    public event Action OnSunrise = delegate { };
    public event Action OnSunset = delegate { };
    public event Action OnHourChange = delegate { };
    public event Action<int> OnDayChanged;

    readonly Observable<bool> isDayTime;
    readonly Observable<int> currentHour;
    readonly Observable<int> currentDay;
    readonly Observable<int> currentMonth;

     EventManager eventManager;

    public TimeService(TimeSettings settings, EventManager eventManager)
    {
        this.settings = settings;
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(settings.startHour);
        sunriseTime = TimeSpan.FromHours(settings.sunriseHour);
        sunsetTime = TimeSpan.FromHours(settings.sunsetHour);

        isDayTime = new Observable<bool>(IsDayTime());
        currentHour = new Observable<int>(currentTime.Hour);
        currentDay = new Observable<int>(day);
        currentMonth = new Observable<int>(month);

        OnDayChanged += (day) => CheckEventsForDay(day, ThisMonth);

        isDayTime.ValueChanged += d => (d ? OnSunrise : OnSunset)?.Invoke();
        currentHour.ValueChanged += _ => OnHourChange?.Invoke();
        this.eventManager = eventManager;
    }

    public void UpdateTime(float deltaTime)
    {
        currentTime = currentTime.AddSeconds(deltaTime * settings.timeMultiplier);
        isDayTime.Value = IsDayTime();
        currentHour.Value = currentTime.Hour;
    }

    public void UpdateDay()
    {
        if (CurrentTime.Hour == 0 && previousHour != 0)
        {
            day++;

            // Monatswechsel prüfen
            if (day > settings.daysPerMonth)
            {
                day = 1; // Tag 1 des neuen Monats
                currentMonth.Value = (currentMonth.Value + 1) % settings.months.Length;
                ThisMonth = settings.months[currentMonth.Value];
            }

            currentDay.Value = day;

            // DayChanged Event feuern
            OnDayChanged?.Invoke(day);

            Debug.Log($"Neuer Tag: {day}, Monat: {ThisMonth}");
        }

        previousHour = CurrentTime.Hour;
    }


    public void UpdateMonth()
    {
        if(day == settings.daysPerMonth+1)
        {
            day = 0;
            currentDay.Value = day;
            if (currentMonth.Value != settings.months.Length-1)
                currentMonth.Value++;
            else currentMonth.Value = 0;

            ThisMonth = settings.months[currentMonth.Value];
        }
    }

    void CheckEventsForDay(int day, string thismonth)
    {
        Debug.Log($"Tag geändert: {day}. Suche Events...");

        foreach (var e in eventManager.events)
        {
            if (e == null) continue;

            Debug.Log(thismonth);
            if (day >= e.startDay && day <= e.endDay && thismonth == e.month)
                {
                    e.eventFunction?.Invoke();
                }
                else
                {
                    e.endEventFunction?.Invoke();
                }
        }
    }

    

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

    bool IsDayTime() => currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime;

    TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
    {
        TimeSpan difference = to - from;
        return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
    }
}