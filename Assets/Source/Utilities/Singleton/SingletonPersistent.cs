using UnityEngine;

namespace NetDive.Utilities.Singleton
{
    public class SingletonPersistent<T> : MonoBehaviour where T : SingletonPersistent<T>, new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    SetupInstance();
                }

                return instance;
            }
        }

        private static void SetupInstance()
        {
            instance = FindObjectOfType<T>();

            if (instance is not null) return;

            var gameObj = new GameObject
            {
                name = typeof(T).Name
            };

            instance = gameObj.AddComponent<T>();
            DontDestroyOnLoad(instance);
        }
        

        private void RemoveDuplicates()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Awake()
        {
            RemoveDuplicates();
        }
    }
}
