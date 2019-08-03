using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    LEFT,
    RIGHT
}

public struct Action
{
    public bool Queued;
    public Direction Direction;
}

public class won : Warrior
{
    [SerializeField, Range(1, 10)] int m_StartingColumn = 3;
    [SerializeField, Range(0, 16)] int m_Won = 0;
    [SerializeField, Range(0, 16)] int m_WonCap = 16;
    [SerializeField, Range(0, 5)] int m_FlairWon = 1;
    [SerializeField, Range(0, 5)] int m_MoveWon = 2;
    [SerializeField, Range(0, 5)] int m_AttackWon = 3;
    [SerializeField, Range(0, 5)] int m_ThrowWon = 3;

    public int Streak { get; private set; }
    public int Won { get { return m_Won; } }

    Action m_QueuedDirection;
    bool m_StreakedLastBeat = false;
    bool m_MovedThisBeat = false;

    private void Awake()
    {
        StartingPosition = Vector2Int.right * (m_StartingColumn - 1);
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveToSide(Direction.LEFT);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveToSide(Direction.RIGHT);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            // TODO check if throw also

            Attack();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Flair();
        }
    }

    /// <summary>
    /// Called every beat
    /// </summary>
    public void BeatUpdate()
    {

    }

    /// <summary>
    /// Called after attack window for every beat
    /// </summary>
    public void PostBeatUpdate()
    {
        m_MovedThisBeat = false;
        //if (!m_StreakedLastBeat)
        //{
        //    Streak = 0;
        //}

        //m_StreakedLastBeat = false;
    }

    private void MoveToSide(Direction direction)
    {
        if (m_MovedThisBeat) return;

        Streak = 0;
        SpendWon(m_MoveWon);

        if (WarriorBeat.Instance.IsInBeatForgiving())
        {
            MoveByDirection(direction);
            m_MovedThisBeat = true;
        }
    }

    void MoveByDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.LEFT:
                Move(Vector2Int.left);
                break;
            case Direction.RIGHT:
                Move(Vector2Int.right);
                break;
        }
    }

    void Attack()
    {
        SpendWon(m_AttackWon);
        if (!WarriorBeat.Instance.IsInBeat())
        {
            Streak = 0;
            return;
        }

        StreakUp();

        // Swing
        foreach (Makaze m in MakazeClan.Instance.Makazes)
        {
            if (m.Position - Position == Vector2.down)
            {
                m.Kill();
                break;
            }
        }
    }

    void Flair()
    {
        SpendWon(m_FlairWon);

        if (WarriorBeat.Instance.IsInBeat()) StreakUp();
        else Streak = 0;

        // Pump your fist

    }

    void Throw()
    {
        SpendWon(m_ThrowWon);

        if (WarriorBeat.Instance.IsInBeat()) StreakUp();
        else Streak = 0;
        
        // Toss
        
    }

    void StreakUp()
    {
        Streak++;
        m_Won += Streak;
        m_Won = Mathf.Clamp(m_Won, 0, m_WonCap);

        m_StreakedLastBeat = true;
    }

    void SpendWon(int won)
    {
        m_Won -= won;
        m_Won = Mathf.Max(m_Won, 0);
        if (m_Won == 0)
        {
            //Kill();
        }
    }

    public override void Kill()
    {
        gameObject.SetActive(false);
    }
}
