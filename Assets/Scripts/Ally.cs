using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Warrior
{
    [SerializeField, Range(1, 10)] public int m_StartingColumn = 3;
    [SerializeField] Transform m_Arm = null;
    [SerializeField] Weapon m_Weapon = null;
    [SerializeField, Range(0, 10)] int m_AttackPauses = 1;

    public Weapon Weapon { get { return m_Weapon; } }
    public bool Attacking { get; private set; }
    public bool ShopItem { get; set; }

    won m_won = null;
    Vector2Int m_GhostPosition = Vector2Int.zero;
    int m_Pauses = 0;

    private void Awake()
    {
        if (!ShopItem) StartingPosition = Vector2Int.right * (m_StartingColumn - 1);
    }

    public override void Start()
    {
        base.Start();
        m_won = MakazeClan.Instance.Enemy;
        if (!ShopItem) m_won.Allies.Add(this);

        if (m_Weapon)
        {
            m_Weapon.transform.parent = m_Arm;
            OrientWeapon();
        }


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
        
        Vector2Int pos = m_won.GetOpenTile(Position, this);
        Move(pos - Position, true);
        Attacking = false;
    }

    public void Attack(Vector2Int position)
    {
        Attacking = true;
        m_GhostPosition = position;
        m_Pauses = 0;
        GhostMove(position - Position);
        if (m_Weapon) m_Weapon.Attack(Position, transform.position);
    }

    public void SetWeapon(Weapon weapon)
    {
        if (m_Weapon) Destroy(m_Weapon.gameObject);

        m_Weapon = weapon;
        m_Weapon.transform.parent = m_Arm;
        OrientWeapon();
    }

    public void OrientWeapon()
    {
        m_Weapon.transform.localPosition = Vector2.zero;
    }

    public void Respawn()
    {
        m_won.Allies.Add(this);
        gameObject.SetActive(true);
        Dead = false;
    }

    public override void Kill()
    {
        WarriorBeat.Instance.RemoveBeatListener(OnBeat);
        m_won.Allies.Remove(this);
        gameObject.SetActive(false);
        Dead = true;
    }
}
