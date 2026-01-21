using System;
using System.Collections.Generic;

/// <summary>
/// A generic observable value that notifies listeners when it changes.
/// </summary>
[Serializable]
public class Observable<T>
{
    // The actual stored value
    private T value;

    // Event called when value changes
    public event Action<T> ValueChanged;

    // Property to get/set the value
    public T Value
    {
        get => value;
        set => Set(value); // Calls Set() to notify listeners
    }

    // Allows implicit conversion to the underlying type
    public static implicit operator T(Observable<T> observable) => observable.value;

    // Constructor
    public Observable(T value, Action<T> onValueChanged = null)
    {
        this.value = value;

        if (onValueChanged != null)
            ValueChanged += onValueChanged;
    }

    // Sets a new value and notifies listeners if it changed
    public void Set(T value)
    {
        // Only notify if value is different
        if (EqualityComparer<T>.Default.Equals(this.value, value))
            return;

        this.value = value;
        Invoke();
    }

    // Invokes the ValueChanged event manually
    public void Invoke()
    {
        ValueChanged?.Invoke(value);
    }

    // Adds a listener to ValueChanged
    public void AddListener(Action<T> handler)
    {
        ValueChanged += handler;
    }

    // Removes a listener from ValueChanged
    public void RemoveListener(Action<T> handler)
    {
        ValueChanged -= handler;
    }

    // Clears all listeners and resets value
    public void Dispose()
    {
        ValueChanged = null;
        value = default;
    }
}
