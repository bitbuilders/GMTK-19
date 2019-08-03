using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T m_Instance = null;

    static public T Instance
    {
        get
        {
            if (!m_Instance)
            {
                T[] objs = Resources.FindObjectsOfTypeAll<T>();
                if (objs != null && objs.Length > 0) m_Instance = objs[0];

                if (!m_Instance)
                {
                    GameObject go = new GameObject($"{typeof(T)} Singleton");

                    m_Instance = go.AddComponent<T>();
                }
            }

            return m_Instance;
        }
    }
}
