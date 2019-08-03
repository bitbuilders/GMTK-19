using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Warrior : MonoBehaviour
{
    [Tooltip("The column this unit will start in"), SerializeField, Range(1, 10)]
    int m_StartigColumn = 3;

    public int StartingColumn
    {
        get { return m_StartigColumn; }
        set { m_StartigColumn = value; }
    }
    public Vector2Int Position { get; protected set; }

    private void Start()
    {
        Vector2Int startTile = Vector2Int.right * (StartingColumn - 1);
        transform.position = BattleGrid.Instance.GetPosition(startTile);
        Position = startTile;
    }

    protected void Move(Vector2Int amount)
    {
        Vector2Int tile = Position + amount;
        Vector3 position = BattleGrid.Instance.GetPosition(tile);

        if (position != Vector3.zero)
        {
            Position = tile;
            transform.position = position;
        }
    }
}
