using System;
using UnityEngine;

public class TimeService
{
    readonly TimeSettings settings;
    DateTime currentTime;
    int day = 0;
    int previousHour = -1;
    readonly TimeSpan sunriseTime;
    readonly TimeSpan sunsetTime;

    public DateTime CurrentTime => currentTime;
    public int Day => day;

    public event Action OnSunrise = delegate { };
    public event Action OnSunset = delegate { };
    public event Action OnHourChange = delegate { };

    readonly Observable<bool> isDayTime;
    readonly Observable<int> currentHour;
    readonly Observable<int> currentDay;

    public TimeService(TimeSettings settings)
    {
        this.settings = settings;
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(settings.startHour);
        sunriseTime = TimeSpan.FromHours(settings.sunriseHour);
        sunsetTime = TimeSpan.FromHours(settings.sunsetHour);

        isDayTime = new Observable<bool>(IsDayTime());
        currentHour = new Observable<int>(currentTime.Hour);
        currentDay = new Observable<int>(day);

        isDayTime.ValueChanged += d => (d ? OnSunrise : OnSunset)?.Invoke();
        currentHour.ValueChanged += _ => OnHourChange?.Invoke();
    }

    public void UpdateTime(float deltaTime)
    {
        currentTime = currentTime.AddSeconds(deltaTime * settings.timeMultiplier);
        isDayTime.Value = IsDayTime();
        currentHour.Value = currentTime.Hour;
    }

    public void UpdateDay()
    {
        Debug.Log("uPDATE day");
        if (CurrentTime.Hour == 0 && !isDayTime.Value && previousHour != CurrentTime.Hour)
        {
            day++;
            currentDay.Value = day;
            Debug.Log($"Neuer Tag: {day}");
        }

        previousHour = CurrentTime.Hour;
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