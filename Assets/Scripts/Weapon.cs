using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] AttackPattern m_AttackPattern = null;

    public void Attack(Vector2Int sourceTile, Vector3 sourcePosition)
    {
        AttackPattern attackPattern = Instantiate(m_AttackPattern);
        attackPattern.transform.position = sourcePosition + Vector3.down;

        Vector2Int dir = Vector2Int.right;
        if (sourceTile.x > BattleGrid.Instance.Bounds.x / 2 - 1) dir = Vector2Int.left;
        attackPattern.UnleashPower(dir);
    }
}
