using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private void Awake()
    {
        if (_instance != null)
        {
            if (_instance != this)
            {
//                Debug.Log("Instance already exist; Destroying " + name);
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            _instance = (T)((System.Object)this);
        }

        if (OrderDontDestroyOnLoad)
        {
            if (gameObject.transform.parent != null)
            {
                gameObject.transform.parent = null;
            }
            DontDestroyOnLoad(gameObject);
            //            print (gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        OnAwake();
    }

    protected virtual void OnAwake()
    {

    }

    protected virtual bool OrderDontDestroyOnLoad
    {
        get
        {
            return false;
        }
    }

    protected virtual void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
    }

    protected static T _instance;

    public static bool HasInstance
    {
        get
        {
            return _instance != null;
        }
    }

    public static T GetInstance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogWarning("Instance not initialized! Have to seek it! WARNING! Bad architecture!");
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null)
                {
                    Debug.LogError("Not found instance of SINGLETON object!!!");
                }
            }

            return _instance;
        }
    }

    public static T GetOrCreateInstance
    {
        get
        {
            if (_instance == null)
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<T>();
                    _instance.name = "(singleton) " + typeof(T).ToString();
                }
            }

            return _instance;
        }
    }
}
