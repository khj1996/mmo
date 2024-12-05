using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    protected bool IsDeleted { get; private set; }

    public static T Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;

                if (_instance is null) return null;
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (!(Instance is null) && Instance != this)
        {
            DestroyImmediate(gameObject);
            IsDeleted = true;
            Debug.LogWarning($"{typeof(T)} 중복 오브젝트 파괴.");
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;

        IsDeleted = false;
        OnDestroyCleanUp();
    }

    protected virtual void OnDestroyCleanUp()
    {
    }
}