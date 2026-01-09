using System;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI monthText;

    [SerializeField] Light sun;
    [SerializeField] Light moon;
    [SerializeField] AnimationCurve lightIntensityCurve;
    [SerializeField] float maxSunIntensity = 1;
    [SerializeField] float maxMoonIntensity = 0.5f;
    [SerializeField] float skyBlendSpeed = 2f;

    [SerializeField] Color dayAmbientLight;
    [SerializeField] Color nightAmbientLight;
    [SerializeField] Volume volume;
    [SerializeField] Material skyboxMaterial;

    ColorAdjustments colorAdjustments;

    [SerializeField] TimeSettings timeSettings;
    [SerializeField] EventManager eventManager;


    public bool IsReady => service != null;


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
        service = new TimeService(timeSettings, eventManager);

    }

    void Start()
    {
        volume.profile.TryGet(out colorAdjustments);
        OnSunrise += () => Debug.Log("Sunrise");
        OnSunset += () => Debug.Log("Sunset");
        OnHourChange += () => Debug.Log("Hour change");
        RenderSettings.skybox = skyboxMaterial;
    }

    void Update()
    {
        Debug.Log(service.CurrentTime.Hour);
        UpdateDayUI();
        UpdateMonthUI();
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings(); 


        //UpdateSkyBlend();


        if (Input.GetKeyDown(KeyCode.Space))
        {
            timeSettings.timeMultiplier *= 2;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            timeSettings.timeMultiplier /= 2;
        }
    }

    void UpdateDayUI()
    {
        service.UpdateDay();
        Debug.Log(service.Day.ToString());

        if(dayText.text != "Day: " + service.Day.ToString())

        dayText.text = "Day: " + service.Day.ToString();
        


    }

    void UpdateMonthUI()
    {
        service.UpdateMonth();
        Debug.Log(service.ThisMonth);
        monthText.text = service.ThisMonth;



    }

    

    void UpdateSkyBlend(float blend)
    {
        if (skyboxMaterial == null) return;
        // Clamp to [0,1] just in case
        Debug.Log("Blend: " + blend);
        blend = Mathf.Clamp01(blend);
        skyboxMaterial.SetFloat("_Blend", blend);
    }

    void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
        float lightIntensity = lightIntensityCurve.Evaluate(dotProduct);

        sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensity);
        moon.intensity = Mathf.Lerp(maxMoonIntensity, 0, lightIntensity);

        float blendedIntensity = Mathf.Pow(lightIntensity, 0.6f);
        float smoothBlend = Mathf.Lerp(skyboxMaterial.GetFloat("_Blend"), blendedIntensity, Time.deltaTime * skyBlendSpeed);
        UpdateSkyBlend(smoothBlend);

        if (colorAdjustments == null) return;
        colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, lightIntensity);

       
    }

    void RotateSun()
    {
        float rotation = service.CalculateSunAngle();
        sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);
     
    }

    void UpdateTimeOfDay()
    {
        service.UpdateTime(Time.deltaTime);
        if (timeText != null)
        {
            timeText.text = service.CurrentTime.ToString("hh:mm");
        }
    }



}