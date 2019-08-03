using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Warrior : MonoBehaviour
{
    public Vector2Int StartingPosition { get; set; }
    public Vector2Int Position { get; protected set; }

    virtual public void Start()
    {
        transform.position = BattleGrid.Instance.GetPosition(StartingPosition);
        Position = StartingPosition;
    }

    public void Move(Vector2Int amount, bool setPosAnyways = false)
    {
        Vector2Int tile = Position + amount;
        Vector3 position = BattleGrid.Instance.GetPosition(tile);

        if (position != Vector3.zero || setPosAnyways)
        {
            Position = tile;
            transform.position = position;
        }
    }

    public void GhostMove(Vector2Int amount)
    {
        Vector2Int tile = Position + amount;
        Vector3 position = BattleGrid.Instance.GetPosition(tile);

        if (position != Vector3.zero)
        {
            transform.position = position;
        }
    }

    abstract public void Kill();
}
