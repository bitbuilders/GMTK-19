using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Warrior
{
    [SerializeField, Range(1, 10)] public int m_StartingColumn = 3;
    [SerializeField] Transform m_Arm = null;
    [SerializeField] Weapon m_Weapon = null;
    [SerializeField, Range(0, 10)] int m_AttackPauses = 1;

    public bool Attacking { get; private set; }

    won m_won = null;
    Vector2Int m_GhostPosition = Vector2Int.zero;
    int m_Pauses = 0;

    private void Awake()
    {
        StartingPosition = Vector2Int.right * (m_StartingColumn - 1);
    }

    public override void Start()
    {
        base.Start();
        m_won = MakazeClan.Instance.Enemy;
        m_won.Allies.Add(this);

        m_Weapon.transform.parent = m_Arm;
        m_Weapon.transform.localPosition = Vector2.zero;

        WarriorBeat.Instance.AddBeatListener(OnBeat);
    }

    public void OnBeat()
    {
        if (!Attacking) return;

        if (m_Pauses < m_AttackPauses)
        {
            m_Pauses++;
            return;
        }

        GhostMove(Position - m_GhostPosition);
    }

    public void Attack(Vector2Int position)
    {
        Attacking = true;
        m_GhostPosition = position;
        m_Pauses = 0;
        GhostMove(position - Position);
    }

    public override void Kill()
    {
        WarriorBeat.Instance.RemoveBeatListener(OnBeat);
        m_won.Allies.Remove(this);
        Destroy(gameObject);
    }
}
