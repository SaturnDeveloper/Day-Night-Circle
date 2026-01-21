using UnityEngine;

/// <summary>
/// Stores settings for the in-game time system.
/// Can be created as a ScriptableObject asset.
/// </summary>
[CreateAssetMenu(fileName = "TimeSettings", menuName = "TimeSettings")]
public class TimeSettings : ScriptableObject
{
    // Multiplier for how fast time passes in the game
    public float timeMultiplier = 2000;

    // Starting hour when the game begins
    public float startHour = 12;

    // Hour when the sun rises
    public float sunriseHour = 6;

    // Hour when the sun sets
    public float sunsetHour = 18;

    // Number of days in a month
    public int daysPerMonth = 30;

    // Names of the months
    public string[] months = { "Spring", "Summer", "Autumn", "Winter" };
}
