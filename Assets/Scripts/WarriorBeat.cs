using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EnemyBeatListeners : UnityEvent { }
[System.Serializable]
public class PlayerBeatListener : UnityEvent { }

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
    [SerializeField] EnemyBeatListeners m_EnemyBeatListeners = null;
    [SerializeField] PlayerBeatListener m_PlayerBeatListener = null;

    AudioSource m_AudioSource = null;
    float m_BPS = 0.0f;
    float m_LastBeat = 0.0f;
    float m_Time = 0.0f;
    float m_EnemyTime = 0.0f;

    private void Start()
    {
        m_BPS = m_BPM / 60.0f;
        m_EnemyTime = m_BPS * m_EnemyOffset;
        m_Time -= m_MusicTimeOffset;

        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.Play();
    }

    void Update()
    {
        m_Time += Time.deltaTime;
        if (m_Time >= m_BPS)
        {
            m_Time -= m_BPS;
            m_LastBeat = Time.time;
            m_PlayerBeatListener.Invoke();
        }

        m_EnemyTime += Time.deltaTime;
        if (m_EnemyTime >= m_BPS)
        {
            m_EnemyTime -= m_BPS;
            m_EnemyBeatListeners.Invoke();
        }
    }

    public bool IsInBeat()
    {
        return Time.time - m_LastBeat <= m_AttackWindow ||
            m_BPS - (Time.time - m_LastBeat) <= m_AttackWindow;
    }
}
