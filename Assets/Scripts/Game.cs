using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : Singleton<Game>
{
    [SerializeField] List<Wave> m_Waves = null;

    List<bool> m_Spawns = null;
    int m_CurrentWave = 0;

    private void Start()
    {
        CreateNextLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    public void ReplayCurrentLevel()
    {
        CreateCurrentLevel();
    }

    public void PlayNextLevel()
    {
        if (!Shop.Instance.Visiting && m_CurrentWave < m_Waves.Count)
        {
            Shop.Instance.Visit();
            StopLevel();
        }
        else if (m_CurrentWave < m_Waves.Count)
        {
            CreateNextLevel();
        }
        else
        {
            Finish();
        }
    }

    public void Finish()
    {
        // TODO
        print("Game Over");
        
    }

    void CreateNextLevel()
    {
        m_CurrentWave++;
        CreateLevel();
    }

    void CreateCurrentLevel()
    {
        CreateLevel(false);
    }

    void CreateLevel(bool refreshShop = true)
    {
        Wave w = Instantiate(m_Waves[m_CurrentWave - 1]);
        m_Spawns = w.GetSpawnData();
        Destroy(w.gameObject);

        Shop.Instance.Visiting = false;
        if (refreshShop) Shop.Instance.Refresh();
        SetupLevel();
    }

    void SetupLevel()
    {
        MakazeClan.Instance.Refresh(m_Spawns);
        MakazeClan.Instance.Playing = true;
    }

    void StopLevel()
    {
        MakazeClan.Instance.Playing = false;
    }
}
