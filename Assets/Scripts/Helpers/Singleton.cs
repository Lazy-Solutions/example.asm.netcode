using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake()
    {
        Instance = (T)(object)this;
    }

    protected virtual void OnDestroy()
    {
        if (Equals(Instance, this))
            Instance = default;
    }
}
