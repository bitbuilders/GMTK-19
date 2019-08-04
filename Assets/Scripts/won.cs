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
    public Ally Ally { get; set; }
    public Vector2Int StartPosition { get; set; }
}

public class won : Warrior
{
    [SerializeField, Range(1, 10)] int m_StartingColumn = 3;
    [SerializeField, Range(0, 32)] int m_Won = 0;
    [SerializeField, Range(0, 32)] int m_WonCap = 16;
    [SerializeField, Range(0, 5)] int m_FlairWon = 1;
    [SerializeField, Range(0, 5)] int m_PickupWon = 2;
    [SerializeField, Range(0, 5)] int m_SwapWon = 2;
    [SerializeField, Range(0, 5)] int m_DropWon = 1;
    [SerializeField, Range(0, 5)] int m_MoveWon = 2;
    [SerializeField, Range(0, 5)] int m_AttackWon = 3;
    [SerializeField, Range(0, 5)] int m_ThrowWon = 3;
    [SerializeField, Range(0.0f, 2.0f)] float m_HoldHeight = 0.4f;

    [SerializeField] AudioClip m_FlairClip;
    [SerializeField] AudioClip m_PunchClip;
    [SerializeField] AudioClip m_PickupClip;

    public int Streak { get; private set; }
    public int Won { get { return m_Won; } }
    public List<Ally> Allies { get; private set; }


    HeldAlly m_HeldAlly = null;
    Movement m_QueuedDirection;
    bool m_StreakedLastBeat = false;
    bool m_MovedThisBeat = false;
    bool m_UsedFirstFlair = false;
    AudioSource m_Voice;

    private void Awake()
    {
        StartingPosition = Vector2Int.right * (m_StartingColumn - 1);
        Allies = new List<Ally>();
        m_HeldAlly = new HeldAlly();
        m_Voice = GetComponent<AudioSource>();
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
            if (Shop.Instance.Visiting)
            {
                if (Position.x == 2)
                {
                    if (m_HeldAlly.Ally)
                    {
                        Ally ally = GetAllyOnTile(Position);
                        if (ally) Swap(ally);
                    }

                    Shop.Instance.Exit();
                }
                else
                {
                    Shop.Instance.BuyItem();
                }
            }
            else
            {
                if (m_HeldAlly.Ally) Throw();
                else Attack();
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Ally ally = GetAllyOnTile(Position);
            if (m_HeldAlly.Ally == null)
            {
                if (ally && !ally.Attacking)
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

        Ally allyOnSprite = GetAllyOnTile(Position);
        if (m_HeldAlly.Ally)
        {
            if (allyOnSprite) DirectionList.Instance.SetSwapSprite();
            else DirectionList.Instance.SetDropSprite();

            DirectionList.Instance.SetThrowSprite();
        }
        else
        {
            if (allyOnSprite && !allyOnSprite.Attacking) DirectionList.Instance.SetPickupSprite();
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
        if (!m_StreakedLastBeat)
        {
            Streak = 0;
        }
        m_StreakedLastBeat = false;
    }

    private void MoveToSide(Direction direction)
    {
        Streak = 0;
        SpendWon(m_MoveWon);

        if (WarriorBeat.Instance.IsInBeatForgiving())
        {
            MoveByDirection(direction);
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

    public override void Move(Vector2Int amount, bool setPosAnyways = false)
    {
        base.Move(amount, setPosAnyways);
        m_Voice.panStereo = -1 + Position.x * .5f;
    }

    void Attack()
    {
        SpendWon(m_AttackWon);
        if (!WarriorBeat.Instance.IsInBeat()) return;

        m_Voice.clip = m_PunchClip;
        m_Voice.Play();
        Streak = 0;
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
            m_Voice.clip = m_FlairClip;
            m_Voice.Play();
        }
        else
        {
            Streak = 0;
        }

        // Pump your fist

    }

    void Pickup(Ally ally)
    {
        if (ally.Attacking) return;

        SpendWon(m_PickupWon);
        m_Voice.clip = m_PickupClip;
        m_Voice.Play();
        m_HeldAlly.Ally = ally;
        m_HeldAlly.StartPosition = Position;
        UpdateHeldAllyPosition();
    }

    void Drop(bool spendWon = true)
    {
        if (!m_HeldAlly.Ally) return;

        if (spendWon) SpendWon(m_DropWon);

        ResetHeldAllyPosition();
        m_HeldAlly.Ally = null;
    }

    void Swap(Warrior ally)
    {
        SpendWon(m_SwapWon);

        ally.Move(m_HeldAlly.StartPosition - ally.Position);
        Drop(false);
    }

    void Throw()
    {
        if (!m_HeldAlly.Ally) return;

        SpendWon(m_ThrowWon);

        if (WarriorBeat.Instance.IsInBeat()) StreakUp();
        else Streak = 0;

        Ally allyToThrow = m_HeldAlly.Ally;
        Drop(false);

        Vector2Int tossPos = new Vector2Int(Position.x, BattleGrid.Instance.Bounds.y + 2);

        Makaze makaze = GetMakazeInColumn();
        if (makaze) tossPos = makaze.Position + Vector2Int.up;

        allyToThrow.Attack(tossPos);
        if (!allyToThrow.Weapon) makaze.Kill();
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

    public Ally GetAllyOnTile(Vector2Int position, bool checkHeld = true)
    {
        foreach (Ally ally in Allies)
        {
            if (!ally) continue;
            if (checkHeld && ally == m_HeldAlly.Ally) continue;

            if (ally.Position - position == Vector2Int.zero)
            {
                return ally;
            }
        }

        return null;
    }

    Ally GetAllyOnTile(Vector2Int position, Ally source, out int count)
    {
        Ally a = null;
        count = 0;

        foreach (Ally ally in Allies)
        {
            if (!ally || ally == m_HeldAlly.Ally) continue;

            if (ally.Position - position == Vector2Int.zero)
            {
                if (ally != source || a == null) a = ally;
                count++;
            }
        }

        return a;
    }

    public Vector2Int GetOpenTile(Vector2Int near, Ally source)
    {
        Vector2Int tile = near;
        int count;
        Ally a = GetAllyOnTile(tile, source, out count);
        Ally currentAlly = a == source && count == 1 ? null : a;
        int val = 1;
        bool left = true;
        
        while ((currentAlly != null && currentAlly != source) || !BattleGrid.Instance.ValidSpace(tile))
        {
            if (left)
            {
                tile = near + (Vector2Int.left * val);
                left = false;
            }
            else
            {
                tile = near + (Vector2Int.right * val);
                left = true;
                val++;
            }

            currentAlly = GetAllyOnTile(tile);
            if (val >= BattleGrid.Instance.Bounds.x / 2 + 1) break;
        }
        
        return tile;
    }

    void StreakUp(bool gainWon = true)
    {
        Streak++;
        m_StreakedLastBeat = true;
        if (gainWon)
        {
            m_Won += Streak;
            m_Won = Mathf.Clamp(m_Won, 0, m_WonCap);
        }
    }

    public void SpendWon(int won)
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

    public void EquipWeapon(Weapon weapon)
    {
        m_HeldAlly.Ally.SetWeapon(weapon);
    }

    public void Respawn(bool deadToo = true)
    {
        gameObject.SetActive(true);
        m_Won = 1;
        m_UsedFirstFlair = false;
        Dead = false;

        ResetPosition();

        //Ally[] tempAllies = Allies.ToArray();
        //foreach (Ally ally in tempAllies)
        //{
        //    Allies.Remove(ally);
        //    Destroy(ally.gameObject);
        //}

        if (!deadToo)
        {
            Ally[] tempAllies = Allies.ToArray();
            foreach (Ally ally in tempAllies)
            {
                if (ally.Dead)
                {
                    Allies.Remove(ally);
                    Destroy(ally.gameObject);
                }
            }
        }
        foreach (Ally ally in Allies)
        {
            if (ally.Dead) ally.Respawn();
            ally.ResetPosition();
        }
    }

    public override void Kill()
    {
        Drop(false);
        gameObject.SetActive(false);
        Dead = true;
        Respawn(false);

        Game.Instance.ReplayCurrentLevel();
    }
}
