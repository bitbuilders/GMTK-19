﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGrid : Singleton<BattleGrid>
{
    [Tooltip("How many tiles wide the grid is"), SerializeField, Range(1, 15)]
    int m_GridWidth = 5;
    [Tooltip("How many tiles tall the grid is"), SerializeField, Range(1, 15)]
    int m_GridHeight = 12;
    [SerializeField] Grid m_Grid = null;

    public Vector2Int Bounds { get { return new Vector2Int(m_GridWidth - 1, -m_GridHeight + 1); } }
    public Vector3 TileOffset { get; private set; }


    private void Awake()
    {
        TileOffset = m_Grid.cellSize / 2.0f;
    }

    public Vector3 GetPosition(Vector2Int moveTo)
    {
        Vector3 moveDist = Vector3.zero;

        if (ValidSpace(moveTo))
        {
            moveDist = GetWorldPosition(moveTo);
            moveDist += TileOffset;
        }

        return moveDist;
    }

    public bool ValidSpace(Vector2Int position)
    {
        return position.x >= 0 && position.x <= m_GridWidth - 1 &&
            position.y <= 0 && position.y >= -(m_GridHeight - 1);
    }

    public Vector3 GetWorldPosition(Vector2Int tile)
    {
        return m_Grid.CellToWorld((Vector3Int)tile);
    }

    public Vector2Int GetTilePosition(Vector3 world)
    {
        return (Vector2Int)m_Grid.WorldToCell(world);
    }
}
