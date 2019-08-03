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
    [Tooltip("How much time the player has to attack"), SerializeField, Range(0.0f, 0.5f)]
    float m_AttackWindow = 0.09f;
    [Tooltip("How much time the player has to attack"), SerializeField, Range(1.0f, 5.0f)]
    float m_ForgivingMultiplier = 2.0f;
    [Tooltip("Percentage enemy offset from player BPM"), SerializeField, Range(0.0f, 1.0f)]
    float m_EnemyOffset = 0.5f;
    [SerializeField] EnemyBeat m_EnemyBeat = null;
    [SerializeField] PlayerBeat m_PlayerBeat = null;
    [SerializeField] PlayerPostBeat m_PlayerPostBeat = null;

    public float BPS { get; private set; }
    public float BPM { get { return m_BPM; } }

    AudioSource m_AudioSource = null;
    float m_LastBeat = 0.0f;
    float m_PlayerTime = 0.0f;
    float m_PostPlayerTime = 0.0f;
    float m_EnemyTime = 0.0f;
    float m_LastSongTime = 0.0f;
    float m_HalfWindow = 0.0f;

    private void Start()
    {
        ResetValues();

        m_AudioSource = GetComponent<AudioSource>();
        //m_AudioSource.pitch = m_BPM / 60.0f;
        m_AudioSource.Play();
    }

    public void Replay()
    {
        ResetValues();
        m_AudioSource.Play();
    }

    void ResetValues()
    {
        BPS = 60.0f / m_BPM;
        m_EnemyTime = BPS * m_EnemyOffset;
        m_PlayerTime = -m_MusicTimeOffset;
        m_HalfWindow = m_AttackWindow / 2.0f;
    }

    void Update()
    {
        float songDelta = m_AudioSource.time - m_LastSongTime;
        if (m_AudioSource.time < m_LastSongTime) songDelta += m_AudioSource.clip.length;

        m_PlayerTime += songDelta;
        if (m_PlayerTime >= BPS)
        {
            m_PlayerTime -= BPS;
            m_LastBeat = Time.time;
            m_PlayerBeat.Invoke();
        }

        m_PostPlayerTime += songDelta;
        if (m_PostPlayerTime >= BPS + m_AttackWindow)
        {
            m_PostPlayerTime -= BPS;
            m_PlayerPostBeat.Invoke();
        }

        m_EnemyTime += songDelta;
        if (m_EnemyTime >= BPS)
        {
            m_EnemyTime -= BPS;
            m_EnemyBeat.Invoke();
        }
        
        m_LastSongTime = m_AudioSource.time;
    }

    public bool IsInBeat()
    {
        //print($"{Time.time - m_LastBeat} | W: {m_HalfWindow}");
        return Time.time - m_LastBeat <= m_HalfWindow ||
            BPS - (Time.time - m_LastBeat) <= m_HalfWindow;
    }

    public bool IsInBeatForgiving()
    {
        float leeway = m_HalfWindow * m_ForgivingMultiplier;
        return Time.time - m_LastBeat <= m_HalfWindow ||
            BPS - (Time.time - m_LastBeat) <= leeway;
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
