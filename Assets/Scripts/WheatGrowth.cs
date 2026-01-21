using System;
using UnityEngine;

// Growth states of the wheat
public enum WheatState
{
    Seeded,
    Growing,
    Mature
}

// Controls wheat growth over time
public class WheatGrowth : MonoBehaviour
{
    // Reference to the TimeManager
    [SerializeField] TimeManager timeManager;

    // Current growth state
    [SerializeField, Header("State")]
    WheatState currentState = WheatState.Seeded;

    // Growth timing
    [Header("Timing Options")]
    public bool isDaily = true;          // Grow by day or by hour
    public float timeToGrowing = 3f;     // Time until Growing
    public float timeToMature = 5f;      // Time until Mature

    // Visuals for each state
    [Header("Visual Options")]
    public GameObject[] seededVisual;
    public GameObject[] growingVisual;
    public GameObject[] matureVisual;

    // Event fired when wheat changes state
    public static Action<WheatState> OnWheatStateChanged;

    // Growth progress timer
    public float growthTimer = 0f;

    void Start()
    {
        // Find the TimeManager in the scene
        timeManager = FindObjectOfType<TimeManager>();

        if (timeManager == null || !timeManager.IsReady)
        {
            Debug.LogError("TimeManager not found in the scene.");
            return;
        }

        // Enable time-based growth
        EnableScript();

        // Apply correct visuals
        ApplyState();
    }

    void Update()
    {
        // No per-frame logic needed
    }

    void OnEnable()
    {
        EnableScript();
    }

    void OnDisable()
    {
        DisableScript();
    }

    // Subscribe to time events
    void EnableScript()
    {
        if (timeManager == null) return;

        if (isDaily)
            timeManager.OnSunrise += OnNewDay;
        else
            timeManager.OnHourChange += OnHourChanged;
    }

    // Unsubscribe from time events
    void DisableScript()
    {
        if (timeManager == null) return;

        if (isDaily)
            timeManager.OnSunrise -= OnNewDay;
        else
            timeManager.OnHourChange -= OnHourChanged;
    }

    // Called every new day
    void OnNewDay()
    {
        growthTimer += 1f;
        CheckGrowth();
    }

    // Called every hour
    void OnHourChanged()
    {
        growthTimer += 1f;
        CheckGrowth();
    }

    // Checks if the wheat should change state
    void CheckGrowth()
    {
        switch (currentState)
        {
            case WheatState.Seeded:
                if (growthTimer >= timeToGrowing)
                    ChangedState(WheatState.Growing);
                break;

            case WheatState.Growing:
                if (growthTimer >= timeToMature)
                {
                    ChangedState(WheatState.Mature);
                    DisableScript(); // Stop growing after mature
                }
                break;
        }
    }

    // Changes the wheat state
    void ChangedState(WheatState newState)
    {
        currentState = newState;
        growthTimer = 0f;

        ApplyState();
        OnWheatStateChanged?.Invoke(currentState);
    }

    // Updates visuals based on the current state
    void ApplyState()
    {
        SetVisuals(seededVisual, currentState == WheatState.Seeded);
        SetVisuals(growingVisual, currentState == WheatState.Growing);
        SetVisuals(matureVisual, currentState == WheatState.Mature);
    }

    // Enables or disables visual objects
    void SetVisuals(GameObject[] visuals, bool active)
    {
        foreach (var visual in visuals)
        {
            visual.SetActive(active);
        }
    }
}
