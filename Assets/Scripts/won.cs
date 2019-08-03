using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    LEFT,
    RIGHT
}

public struct Movement
{
    public bool Queued;
    public Direction Direction;
}

public class HeldAlly
{
    public Warrior Ally { get; set; }
    public Vector2Int StartPosition { get; set; }
}

public class won : Warrior
{
    [SerializeField, Range(1, 10)] int m_StartingColumn = 3;
    [SerializeField, Range(0, 16)] int m_Won = 0;
    [SerializeField, Range(0, 16)] int m_WonCap = 16;
    [SerializeField, Range(0, 5)] int m_FlairWon = 1;
    [SerializeField, Range(0, 5)] int m_PickupWon = 2;
    [SerializeField, Range(0, 5)] int m_DropWon = 1;
    [SerializeField, Range(0, 5)] int m_MoveWon = 2;
    [SerializeField, Range(0, 5)] int m_AttackWon = 3;
    [SerializeField, Range(0, 5)] int m_ThrowWon = 3;
    [SerializeField, Range(0.0f, 2.0f)] float m_HoldHeight = 0.4f;

    public int Streak { get; private set; }
    public int Won { get { return m_Won; } }
    public List<Ally> Allies { get; private set; }

    HeldAlly m_HeldAlly = null;
    Movement m_QueuedDirection;
    bool m_MovedThisBeat = false;
    bool m_UsedFirstFlair = false;

    private void Awake()
    {
        StartingPosition = Vector2Int.right * (m_StartingColumn - 1);
        Allies = new List<Ally>();
        m_HeldAlly = new HeldAlly();
    }

    public override void Start()
    {
        base.Start();
        DirectionList.Instance.SetMoveSprite();
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
            if (m_HeldAlly.Ally) Throw();
            else Attack();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Warrior ally = GetAllyOnTile();
            if (m_HeldAlly.Ally == null)
            {
                if (ally)
                {
                    // Pickup
                    Pickup(ally);
                }
                else
                {
                    Flair();
                }
            }
            else
            {
                // Drop or Swap
                if (ally) Swap(ally);
                else Drop();
            }
        }

        Warrior allyOnSprite = GetAllyOnTile();
        if (m_HeldAlly.Ally)
        {
            if (allyOnSprite) DirectionList.Instance.SetSwapSprite();
            else DirectionList.Instance.SetDropSprite();

            DirectionList.Instance.SetThrowSprite();
        }
        else
        {
            if (allyOnSprite) DirectionList.Instance.SetPickupSprite();
            else DirectionList.Instance.SetFlairSprite();

            DirectionList.Instance.SetPunchSprite();
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
        Vector2Int movement = Vector2Int.zero;
        switch (direction)
        {
            case Direction.LEFT:
                movement = Vector2Int.left;
                break;
            case Direction.RIGHT:
                movement = Vector2Int.right;
                break;
        }

        Move(movement);
        if (m_HeldAlly.Ally)
        {
            m_HeldAlly.Ally.Move(movement);
            UpdateHeldAllyPosition();
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

        if (WarriorBeat.Instance.IsInBeat() || !m_UsedFirstFlair)
        {
            StreakUp();
            m_UsedFirstFlair = true;
        }
        else
        {
            Streak = 0;
        }

        // Pump your fist

    }

    void Pickup(Warrior ally)
    {
        SpendWon(m_PickupWon);

        m_HeldAlly.Ally = ally;
        m_HeldAlly.StartPosition = Position;
        UpdateHeldAllyPosition();
    }

    void Drop()
    {
        SpendWon(m_DropWon);

        ResetHeldAllyPosition();
        m_HeldAlly.Ally = null;
    }

    void Swap(Warrior ally)
    {
        ally.Move(m_HeldAlly.StartPosition - ally.Position);
        Drop();
    }

    void Throw()
    {
        SpendWon(m_ThrowWon);

        if (WarriorBeat.Instance.IsInBeat()) StreakUp();
        else Streak = 0;

        // Toss
        Vector2Int tossPos = new Vector2Int(Position.x, BattleGrid.Instance.Bounds.y);

        Makaze makaze = GetMakazeInColumn();
        if (makaze) tossPos = makaze.Position + Vector2Int.up;
    }

    Makaze GetMakazeInColumn()
    {
        Makaze makaze = null;
        int dist = int.MaxValue;

        foreach (Makaze m in MakazeClan.Instance.Makazes)
        {
            if (m.Position.x == Position.x)
            {
                int sqrDist = (Position - m.Position).sqrMagnitude;
                if (sqrDist < dist)
                {
                    dist = sqrDist;
                    makaze = m;
                }
            }
        }

        return makaze;
    }

    Warrior GetAllyOnTile()
    {
        foreach (Warrior ally in Allies)
        {
            if (!ally || ally == m_HeldAlly.Ally) continue;

            if (ally.Position - Position == Vector2Int.zero)
            {
                return ally;
            }
        }

        return null;
    }

    void StreakUp()
    {
        Streak++;
        m_Won += Streak;
        m_Won = Mathf.Clamp(m_Won, 0, m_WonCap);
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

    void UpdateHeldAllyPosition()
    {
        if (!m_HeldAlly.Ally) return;

        m_HeldAlly.Ally.transform.position = transform.position + Vector3.up * m_HoldHeight;
    }

    void ResetHeldAllyPosition()
    {
        if (!m_HeldAlly.Ally) return;

        m_HeldAlly.Ally.transform.position = transform.position;
    }

    public override void Kill()
    {
        gameObject.SetActive(false);
        m_UsedFirstFlair = false;
    }
}
