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
    [Tooltip("Normal music"), SerializeField]
    AudioClip m_NormalMusic = null;
    [Tooltip("Shop music"), SerializeField]
    AudioClip m_ShopMusic = null;
    [SerializeField] EnemyBeat m_EnemyBeat = null;
    [SerializeField] PlayerBeat m_PlayerBeat = null;
    [SerializeField] PlayerPostBeat m_PlayerPostBeat = null;

    public float BPS { get; private set; }
    public int BPM
    {
        get { return m_BPM; }
        set
        {
            m_BPM = value;
            m_AudioSource.Stop();
            ResetValues();
        }
    }

    AudioSource m_AudioSource = null;
    AudioClip m_CurrentClip = null;
    float m_LastBeat = 0.0f;
    float m_PlayerTime = 0.0f;
    float m_PostPlayerTime = 0.0f;
    float m_EnemyTime = 0.0f;
    float m_LastPlayerTime = 0.0f;
    float m_LastEnemyTime = 0.0f;
    float m_HalfWindow = 0.0f;

    float m_Lifetime = 0.0f;
    float m_EnemyLifetime = 0.0f;
    bool m_DonePost = false;
    bool m_DoneEnemy = false;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        PlayNormalMusic();
    }

    void ResetValues()
    {
        BPS = 60.0f / m_BPM;
        m_EnemyTime = BPS * m_EnemyOffset;
        m_PlayerTime = -m_MusicTimeOffset;
        m_HalfWindow = m_AttackWindow / 2.0f;
        m_AudioSource.clip = m_CurrentClip;
        m_AudioSource.Play();
    }

    public void RestartTrack()
    {
        m_AudioSource.Stop();
        ResetValues();
    }

    public void PlayNormalMusic()
    {
        m_CurrentClip = m_NormalMusic;
        BPM = 120;
    }

    public void PlayShopMusic()
    {
        m_CurrentClip = m_ShopMusic;
        BPM = 60;
    }

    bool first = true;
    void Update()
    {
        if (m_AudioSource.time == 0.0f) return;
        else if (first)
        {
            m_Lifetime = m_AudioSource.time;
            SetEnemyLifetime();
            first = false;
        }
        else
        {
            m_Lifetime += Time.deltaTime;
            m_EnemyLifetime += Time.deltaTime;
        }

        m_PlayerTime = m_AudioSource.time % BPS;
        if (m_PlayerTime < m_LastPlayerTime)
        {
            //m_Lifetime = m_AudioSource.time;
            //m_PlayerTime = m_Lifetime % BPS;
            m_LastBeat = Time.time;
            m_PlayerBeat.Invoke();
            m_DonePost = false;
            m_DoneEnemy = false;
        }
        
        if (m_PlayerTime >= m_AttackWindow && !m_DonePost)
        {
            m_PlayerPostBeat.Invoke();
            m_DonePost = true;
        }
        
        if (m_PlayerTime >= m_EnemyOffset * BPS && !m_DoneEnemy)
        {
            m_EnemyBeat.Invoke();
            m_DoneEnemy = true;
        }

        m_LastPlayerTime = m_PlayerTime;
    }

    void SetEnemyLifetime()
    {
        m_EnemyLifetime = m_Lifetime + BPS * m_EnemyOffset;
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
