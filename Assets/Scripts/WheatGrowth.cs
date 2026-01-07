using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;


public enum WheatState
{
    Seeded,
    Growing,
    Mature
}

public class WheatGrowth : MonoBehaviour
{
    [SerializeField]
    TimeManager timeManager;

    [SerializeField, Header("State")]
    WheatState currentState = WheatState.Seeded;

    [Header("Timing Options")]
    public bool isDaily = true;
    public float timeToGrowing = 3f; //In days or hours
    public float timeToMature = 5f; //In days or hours

    [Header("Visual Options")]
    public GameObject[] seededVisual;
    public GameObject[] growingVisual;
    public GameObject[] matureVisual;

    public static Action<WheatState> OnWheatStateChanged;

    public float growthTimer = 0f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        timeManager = FindObjectOfType<TimeManager>();

        if (timeManager == null || !timeManager.IsReady)
        {
            Debug.LogError("TimeManager not found in the scene.");
            return;
        }
        EnableScript();
        ApplyState();
 
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnable()
    {
        EnableScript();
    }

    void EnableScript() {

        if (timeManager != null)
        {
            if (isDaily)
                timeManager.OnSunrise += OnNewDay;
            else
                timeManager.OnHourChange += OnHourChanged;
        }
    }

    void OnDisable()
    {
        DisableScript();
    }

    void DisableScript()
    {
        if (isDaily)
            timeManager.OnSunrise -= OnNewDay;
        else
            timeManager.OnHourChange -= OnHourChanged;
    }

    void OnNewDay() { 
        growthTimer += 1f;
        CheckGrowth();
    }

    void OnHourChanged()
    {
        growthTimer += 1f;
        CheckGrowth();
    }

    void CheckGrowth()
    {
        switch(currentState)
        {
            case WheatState.Seeded:
                if (growthTimer >= timeToGrowing)
                    ChangedState(WheatState.Growing);
                break;
            case WheatState.Growing:
                if (growthTimer >= timeToMature)
                    ChangedState(WheatState.Mature);
                DisableScript();
                break;
        }
    }

    void ChangedState(WheatState newState)
    {
        currentState = newState;
        growthTimer = 0f;
        ApplyState();
        OnWheatStateChanged?.Invoke(currentState);
    }

    void ApplyState() 
    {
   
       SetVisuals(seededVisual, currentState == WheatState.Seeded);
       SetVisuals(growingVisual, currentState == WheatState.Growing);
       SetVisuals(matureVisual, currentState == WheatState.Mature);
    }

    void SetVisuals(GameObject[] visuals, bool active)
    {
        foreach (var visual in visuals)
        {
            visual.SetActive(active);
        }
    }
}
