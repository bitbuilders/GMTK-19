using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakazeClan : Singleton<MakazeClan>
{
    [SerializeField] GameObject m_MakazeTemplate = null;
    [SerializeField] won m_Won = null;
    [SerializeField, Range(0, 20)] int m_SpawnRateMin = 1;
    [SerializeField, Range(0, 20)] int m_SpawnRateMax = 3;

    public won Enemy { get { return m_Won; } }
    public List<Makaze> Makazes { get; private set; }
    public List<bool> Spawns { get; private set; }
    public bool Playing { get; set; } = false;
    public bool Spawning { get; private set; }

    int m_SpawnRate = 0;
    int m_BeatsPassed = 0;

    private void Start()
    {
        Makazes = new List<Makaze>();
    }

    public void Refresh(List<bool> spawns)
    {
        Clear();
        Spawns = spawns;

        m_BeatsPassed = 0;
        Spawning = true;
    }

    public void Clear()
    {
        Makaze[] tempMakazes = Makazes.ToArray();

        foreach (Makaze m in tempMakazes)
        {
            Destroy(m.gameObject);
        }

        Makazes.Clear();
    }

    public void OnBeat()
    {
        if (!Playing || !Spawning) return;

        int startIndex = m_BeatsPassed * 5;
        for (int i = startIndex; i < startIndex + 5; i++)
        {
            if (i >= Spawns.Count)
            {
                Spawning = false;
                break;
            }

            if (!Spawns[i]) continue;

            Vector2Int startPos = GetStartingPosition(i - startIndex);
            SpawnMakaze(startPos);
        }

        m_BeatsPassed++;
    }

    public void OnPostBeat()
    {
        if (!Playing) return;
        
        if (Makazes.Count == 0 && !Spawning)
        {
            Playing = false;
            Game.Instance.PlayNextLevel();
        }
    }

    void SpawnMakaze(Vector2Int startingPosition)
    {
        GameObject go = Instantiate(m_MakazeTemplate, Vector3.zero, Quaternion.identity, null);
        Makaze m = go.GetComponent<Makaze>();
        m.StartingPosition = startingPosition;
        Makazes.Add(m);
    }

    Vector2Int GetStartingPosition(int x)
    {
        int y = BattleGrid.Instance.Bounds.y;

        return new Vector2Int(x, y);
    }

    public void RegisterAttack(Vector2Int[] hitPoints)
    {
        Makaze[] tempMakazes = Makazes.ToArray();
        for (int i = 0; i < hitPoints.Length; i++)
        {
            for (int j = 0; j < tempMakazes.Length; j++)
            {
                if (tempMakazes[j].Position == hitPoints[i])
                {
                    tempMakazes[j].Kill();
                }
            }
        }
    }
}
