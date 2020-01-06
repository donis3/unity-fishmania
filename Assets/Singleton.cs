using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhinotap.Toolkit
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();//find the instance in the scene
                    if (_instance == null)
                        Debug.LogError("Singleton Error: Can't find an object with " + typeof(T).ToString() + " component in the scene");
                }
                return _instance;
            }
        }
    }
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T instance => _instance;

        protected void initializeSingleton()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();//find the instance in the scene
                if (_instance == null)
                    Debug.LogError("Singleton Error: Can't find an object with " + typeof(T).ToString() + " component in the scene");
                else
                    DontDestroyOnLoad(_instance.gameObject);
            }
        }

        protected void Awake()
        {
            initializeSingleton();
        }

    }
}
