using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EnemyBeat : UnityEvent { }
[System.Serializable]
public class PlayerBeat : UnityEvent { }
[System.Serializable]
public class PlayerPostBeat : UnityEvent { }

public class WarriorBeat : Singleton<WarriorBeat>
{
    [Tooltip("Beats per minute"), SerializeField, Range(0, 200)]
    int m_BPM = 60;
    [Tooltip("Beats per minute"), SerializeField, Range(0.0f, 5.0f)]
    float m_MusicTimeOffset = 0.5f;
    [Tooltip("How much time the player has to attack"), SerializeField, Range(0.0f, 0.25f)]
    float m_AttackWindow = 0.09f;
    [Tooltip("Percentage enemy offset from player BPM"), SerializeField, Range(0.0f, 1.0f)]
    float m_EnemyOffset = 0.5f;
    [SerializeField] EnemyBeat m_EnemyBeat = null;
    [SerializeField] PlayerBeat m_PlayerBeat = null;
    [SerializeField] PlayerPostBeat m_PlayerPostBeat = null;

    AudioSource m_AudioSource = null;
    float m_BPS = 0.0f;
    float m_LastBeat = 0.0f;
    float m_PlayerTime = 0.0f;
    float m_PostPlayerTime = 0.0f;
    float m_EnemyTime = 0.0f;

    private void Start()
    {
        m_BPS = m_BPM / 60.0f;
        m_EnemyTime = m_BPS * m_EnemyOffset;
        m_PlayerTime -= m_MusicTimeOffset;

        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.Play();
    }

    void Update()
    {
        m_PlayerTime += Time.deltaTime;
        if (m_PlayerTime >= m_BPS)
        {
            m_PlayerTime -= m_BPS;
            m_LastBeat = Time.time;
            m_PlayerBeat.Invoke();
        }

        m_PostPlayerTime += Time.deltaTime;
        if (m_PostPlayerTime >= m_BPS + m_AttackWindow)
        {
            m_PostPlayerTime -= m_BPS;
            m_PlayerPostBeat.Invoke();
        }

        m_EnemyTime += Time.deltaTime;
        if (m_EnemyTime >= m_BPS)
        {
            m_EnemyTime -= m_BPS;
            m_EnemyBeat.Invoke();
        }
    }

    public bool IsInBeat()
    {
        //print(Time.time - m_LastBeat);
        return Time.time - m_LastBeat <= m_AttackWindow ||
            m_BPS - (Time.time - m_LastBeat) <= m_AttackWindow;
    }

    public void AddBeatListener(UnityAction action)
    {
        m_PlayerBeat.AddListener(action);
    }

    public void AddPostBeatListener(UnityAction action)
    {
        m_PlayerPostBeat.AddListener(action);
    }

    public void AddEnemyBeatListener(UnityAction action)
    {
        m_EnemyBeat.AddListener(action);
    }

    public void RemoveBeatListener(UnityAction action)
    {
        m_PlayerBeat.RemoveListener(action);
    }

    public void RemovePostBeatListener(UnityAction action)
    {
        m_PlayerPostBeat.RemoveListener(action);
    }

    public void RemoveEnemyBeatListener(UnityAction action)
    {
        m_EnemyBeat.RemoveListener(action);
    }
}
