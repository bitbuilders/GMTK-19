using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : Singleton<Game>
{
    [SerializeField] List<GameObject> m_Levels = null;

    GameObject m_PreviousLevel = null;
    int m_CurrentLevel = 0;

    private void Start()
    {
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
        if (!Shop.Instance.Visiting)
        {
            Shop.Instance.Visit();
            StopLevel();
        }
        else if (m_CurrentLevel <= m_Levels.Count)
        {
            CreateNextLevel();
            SetupLevel();
        }
        else
        {
            Finish();
        }
    }

    public void Finish()
    {
        // TODO

    }

    void CreateNextLevel()
    {
        if (m_PreviousLevel) Destroy(m_PreviousLevel);
        m_CurrentLevel++;
        CreateLevel();
        Shop.Instance.Visiting = false;
        Shop.Instance.Refresh();
        SetupLevel();
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
        //m_PreviousLevel = Instantiate(m_Levels[m_CurrentLevel - 1]);
    }

    void SetupLevel()
    {
        MakazeClan.Instance.Refresh();
        MakazeClan.Instance.Playing = true;
    }

    void StopLevel()
    {
        MakazeClan.Instance.Playing = false;
    }
}
