using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this.GetComponent<T>())
        {
            Destroy(gameObject);
            return;
        }

        Instance = GetComponent<T>();
    }
}