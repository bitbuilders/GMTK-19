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
    int m_SpawnRate = 0;
    int m_BeatsPassed = 0;

    private void Start()
    {
        Makazes = new List<Makaze>();
        ResetSpawnRate();
    }

    public void Refresh()
    {
        Clear();

        m_BeatsPassed = 0;
        ResetSpawnRate();
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
        m_BeatsPassed++;
        if (m_BeatsPassed >= m_SpawnRate)
        {
            m_BeatsPassed = 0;
            ResetSpawnRate();
            SpawnMakaze();
        }
    }

    void SpawnMakaze()
    {
        GameObject go = Instantiate(m_MakazeTemplate, Vector3.zero, Quaternion.identity, null);
        Makaze m = go.GetComponent<Makaze>();
        m.StartingPosition = GetStartingPosition();
        Makazes.Add(m);
    }

    Vector2Int GetStartingPosition()
    {
        int maxX = BattleGrid.Instance.Bounds.x;
        int x = Random.Range(0, maxX + 1);
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

    void ResetSpawnRate()
    {
        m_SpawnRate = Random.Range(m_SpawnRateMin, m_SpawnRateMax + 1);
    }
}
