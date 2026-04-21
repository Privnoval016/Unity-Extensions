using UnityEngine;

namespace Extensions.Singleton
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);

                    if (_instance == null)
                    {
                        Debug.LogWarning($"No instance of {typeof(T).Name} found in the scene. Please ensure that an instance is present.");
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"Duplicate {typeof(T).Name} found on {gameObject.name}, destroying.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}