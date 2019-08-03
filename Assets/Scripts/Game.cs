using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : Singleton<Game>
{
    [SerializeField] List<GameObject> m_Levels = null;

    GameObject m_PreviousLevel = null;
    GameObject m_PreviousData = null;
    List<Ally> m_PreviousAllies = null;
    int m_CurrentLevel = 0;

    private void Start()
    {
        m_PreviousAllies = new List<Ally>();
        CreateNextLevel();
    }

    public void ReplayPreviousLevel()
    {
        if (m_CurrentLevel > 1) CreatePreviousLevel();
        else ReplayCurrentLevel();

        SetupLevel();
    }

    public void ReplayCurrentLevel()
    {
        CreateCurrentLevel();
        SetupLevel();
    }

    public void PlayNextLevel()
    {
        if (m_CurrentLevel <= m_Levels.Count) CreateNextLevel();
        else Finish();

        SetupLevel();
    }

    public void Finish()
    {

    }

    void CreateNextLevel()
    {
        if (m_PreviousLevel) Destroy(m_PreviousLevel);
        m_CurrentLevel++;
        CreateLevel();
    }

    void CreateCurrentLevel()
    {
        if (m_PreviousLevel) Destroy(m_PreviousLevel);
        CreateLevel();
    }

    void CreatePreviousLevel()
    {
        if (m_PreviousLevel) Destroy(m_PreviousLevel);
        CreateLevel();
    }

    void CreateLevel()
    {
        m_PreviousLevel = Instantiate(m_Levels[m_CurrentLevel - 1]);
    }

    void SetupLevel()
    {
        MakazeClan.Instance.Refresh();
    }
}
