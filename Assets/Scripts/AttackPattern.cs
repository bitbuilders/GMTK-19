using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPattern : MonoBehaviour
{
    [SerializeField] List<Transform> m_HitPoints = null;
    [SerializeField] bool m_UpdateRotation = false;
    
    public void UnleashPower(Vector2Int direction)
    {
        if (m_UpdateRotation)
        {
            float angle = Mathf.Atan2(direction.y, direction.x);
            transform.rotation = Quaternion.Euler(Vector3.forward * Mathf.Rad2Deg * angle);
        }

        Vector2Int[] tilesHit = new Vector2Int[m_HitPoints.Count];

        for (int i = 0; i < m_HitPoints.Count; i++)
        {
            tilesHit[i] = BattleGrid.Instance.GetTilePosition(m_HitPoints[i].position);
        }

        MakazeClan.Instance.RegisterAttack(tilesHit);
        Destroy(gameObject, WarriorBeat.Instance.BPS);
    }
}
