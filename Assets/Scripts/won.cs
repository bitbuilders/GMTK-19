using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    LEFT,
    RIGHT
}

public struct DirectionData
{
    public bool Queued;
    public Direction Direction;
}

public class won : Warrior
{
    [SerializeField, Range(0, 16)] int m_Won = 0;
    [SerializeField, Range(0, 16)] int m_WonCap = 16;
    [SerializeField, Range(0, 5)] int m_FlairWon = 1;
    [SerializeField, Range(0, 5)] int m_MoveWon = 2;
    [SerializeField, Range(0, 5)] int m_AttackWon = 3;
    [SerializeField, Range(0, 5)] int m_ThrowWon = 3;

    public int Streak { get; private set; }
    public int Won { get { return m_Won; } }

    DirectionData m_QueuedDirection;
    bool m_StreakedLastBeat = false;

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
    /// Called every beat frame
    /// </summary>
    public void BeatUpdate()
    {
        if (m_QueuedDirection.Queued)
        {
            m_QueuedDirection.Queued = false;
            MoveByDirection(m_QueuedDirection.Direction);
        }

        // TODO Reset streak the beat you didn't streak

        if (!m_StreakedLastBeat)
        {
            Streak = 0;
        }

        m_StreakedLastBeat = false;
    }

    private void MoveToSide(Direction direction)
    {
        Streak = 0;
        m_Won -= m_MoveWon;

        if (WarriorBeat.Instance.IsInBeat())
        {
            MoveByDirection(direction);
        }
        else
        {
            // Queue move
            if (m_QueuedDirection.Queued)
            {
                m_Won += m_MoveWon; // Refund
            }
            else
            {
                m_QueuedDirection.Queued = true;
                m_QueuedDirection.Direction = direction;
            }
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
        m_Won -= m_AttackWon;
        if (!WarriorBeat.Instance.IsInBeat())
        {
            Streak = 0;
            return;
        }

        StreakUp();
        m_Won += Streak;

        // Swing

    }

    void Flair()
    {
        m_Won -= m_FlairWon;

        if (WarriorBeat.Instance.IsInBeat()) StreakUp();
        else Streak = 0;

        m_Won += Streak;

        // Pump your fist

    }

    void Throw()
    {
        m_Won -= m_ThrowWon;

        if (WarriorBeat.Instance.IsInBeat()) StreakUp();
        else Streak = 0;

        m_Won += Streak;
        
        // Toss
        
    }

    void StreakUp()
    {
        Streak++;
        m_StreakedLastBeat = true;
    }
}
