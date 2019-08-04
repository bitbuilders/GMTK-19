using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Makaze : Warrior
{
    [SerializeField, Range(0, 3)] int m_MaxPauses = 1;

    int m_Pauses = 0;

    new void Start()
    {
        base.Start();
        WarriorBeat.Instance.AddBeatListener(OnBeat);
        WarriorBeat.Instance.AddEnemyBeatListener(OnAttackBeat);
    }

    public void OnBeat()
    {
        if (Position.y < -1)
        {
            Move(Vector2Int.up);
        }
    }

    public void OnAttackBeat()
    {
        if (Position.y == -1 && m_Pauses < m_MaxPauses)
        {
            m_Pauses++;
            return;
        }

        Warrior enemy = GetAboveEnemy();
        if (enemy)
        {
            Attack(enemy);
        }
        else if (Position.y == -1 && m_Pauses == m_MaxPauses)
        {
            Move(Vector2Int.up);
        }
        else if (Position.y == 0)
        {
            MakazeClan.Instance.Enemy.Kill();
        }
    }

    void Attack(Warrior enemy)
    {
        enemy.Kill();
    }

    Warrior GetAboveEnemy()
    {
        Warrior enemy = null;

        if (MakazeClan.Instance.Enemy.Position - Position == Vector2.up)
        {
            enemy = MakazeClan.Instance.Enemy;
        }
        else
        {
            foreach (Warrior enemyAlly in MakazeClan.Instance.Enemy.Allies)
            {
                if (enemyAlly.Position - Position == Vector2.up)
                {
                    enemy = enemyAlly;
                    break;
                }
            }
        }

        return enemy;
    }

    public override void Kill()
    {
        MakazeClan.Instance.Makazes.Remove(this);
        WarriorBeat.Instance.RemoveBeatListener(OnBeat);
        WarriorBeat.Instance.RemoveEnemyBeatListener(OnAttackBeat);
        Destroy(gameObject);
    }
}
