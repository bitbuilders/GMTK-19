using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField] List<WaveEnemy> m_WaveEnemies = null;
    [SerializeField] List<GameObject> m_Rows = null;
    //[SerializeField] bool set;

    public List<bool> GetSpawnData()
    {
        List<bool> enemySpawns = new List<bool>(m_WaveEnemies.Count);

        foreach (WaveEnemy enemy in m_WaveEnemies)
        {
            enemySpawns.Add(enemy.gameObject.activeSelf);
            if (enemy.LastOne) break;
        }

        return enemySpawns;
    }

    private void OnValidate()
    {
        //if (set)
        //{
        //    for (int i = 0; i < m_WaveEnemies.Count; i++)
        //    {
        //        int x = i % 5;
        //        int xPos = x - 2;
        //        m_WaveEnemies[i].transform.localPosition = Vector3.right * xPos;
        //        //m_WaveEnemies[i].GetComponent<SpriteRenderer>().enabled = true;
        //        //m_WaveEnemies[i].gameObject.SetActive(false);
        //    }
        //    set = false;
        //}

        for (int i = 0; i < m_Rows.Count; i++)
        {
            m_Rows[i].transform.localPosition = Vector3.down * i;
        }
    }
}
