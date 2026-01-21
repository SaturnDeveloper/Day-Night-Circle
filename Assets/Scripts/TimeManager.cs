using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Controls time, lighting, UI, and sky based on TimeService
public class TimeManager : MonoBehaviour
{
    // UI elements
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI monthText;

    // Sun and moon lights
    [SerializeField] Light sun;
    [SerializeField] Light moon;

    // Light intensity settings
    [SerializeField] AnimationCurve lightIntensityCurve;
    [SerializeField] float maxSunIntensity = 1;
    [SerializeField] float maxMoonIntensity = 0.5f;
    [SerializeField] float skyBlendSpeed = 2f;

    // Ambient light colors
    [SerializeField] Color dayAmbientLight;
    [SerializeField] Color nightAmbientLight;

    // Post processing and sky
    [SerializeField] Volume volume;
    [SerializeField] Material skyboxMaterial;

    ColorAdjustments colorAdjustments;

    // Time and event settings
    [SerializeField] TimeSettings timeSettings;
    [SerializeField] EventManager eventManager;

    // True when TimeService is ready
    public bool IsReady => service != null;

    // Time events (forwarded from TimeService)
    public event Action OnSunrise
    {
        add => service.OnSunrise += value;
        remove => service.OnSunrise -= value;
    }

    public event Action OnSunset
    {
        add => service.OnSunset += value;
        remove => service.OnSunset -= value;
    }

    public event Action OnHourChange
    {
        add => service.OnHourChange += value;
        remove => service.OnHourChange -= value;
    }

    TimeService service;

    void Awake()
    {
        // Create the time service
        service = new TimeService(timeSettings, eventManager);
    }

    void Start()
    {
        // Get post-processing settings
        volume.profile.TryGet(out colorAdjustments);

        // Debug events
        OnSunrise += () => Debug.Log("Sunrise");
        OnSunset += () => Debug.Log("Sunset");
        OnHourChange += () => Debug.Log("Hour change");

        // Set skybox
        RenderSettings.skybox = skyboxMaterial;
    }

    void Update()
    {
        // Update UI
        UpdateMonthUI();
        UpdateDayUI();

        // Update time and visuals
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();

        // Speed up time
        if (Input.GetKeyDown(KeyCode.Space))
            timeSettings.timeMultiplier *= 2;

        // Slow down time
        if (Input.GetKeyDown(KeyCode.LeftShift))
            timeSettings.timeMultiplier /= 2;
    }

    // Updates the day UI
    void UpdateDayUI()
    {
        service.UpdateDay();

        if (dayText.text != "Day: " + service.Day)
            dayText.text = "Day: " + service.Day;
    }

    // Updates the month UI
    void UpdateMonthUI()
    {
        service.UpdateMonth();
        monthText.text = service.ThisMonth;
    }

    // Blends the skybox between day and night
    void UpdateSkyBlend(float blend)
    {
        if (skyboxMaterial == null) return;

        blend = Mathf.Clamp01(blend);
        skyboxMaterial.SetFloat("_Blend", blend);
    }

    // Updates lights, sky, and ambient color
    void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
        float lightIntensity = lightIntensityCurve.Evaluate(dotProduct);

        sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensity);
        moon.intensity = Mathf.Lerp(maxMoonIntensity, 0, lightIntensity);

        float blendedIntensity = Mathf.Pow(lightIntensity, 0.6f);
        float smoothBlend = Mathf.Lerp(
            skyboxMaterial.GetFloat("_Blend"),
            blendedIntensity,
            Time.deltaTime * skyBlendSpeed
        );

        UpdateSkyBlend(smoothBlend);

        if (colorAdjustments == null) return;
        colorAdjustments.colorFilter.value =
            Color.Lerp(nightAmbientLight, dayAmbientLight, lightIntensity);
    }

    // Rotates the sun based on time
    void RotateSun()
    {
        float rotation = service.CalculateSunAngle();
        sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);
    }

    // Updates the current time
    void UpdateTimeOfDay()
    {
        service.UpdateTime(Time.deltaTime);

        if (timeText != null)
            timeText.text = service.CurrentTime.ToString("hh:mm");
    }
}
