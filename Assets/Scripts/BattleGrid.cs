﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleGrid : Singleton<BattleGrid>
{
    [Tooltip("The tilemap with the tiles all warriors will use"), SerializeField]
    Tilemap m_BattleMap = null;
    [Tooltip("How many tiles wide the grid is"), SerializeField, Range(1, 10)]
    int m_GridWidth = 5;
    [Tooltip("How many tiles tall the grid is"), SerializeField, Range(1, 10)]
    int m_GridHeight = 6;

    public Vector3 TileOffset { get; private set; }

    Grid m_Grid = null;

    private void Start()
    {
        m_Grid = GetComponent<Grid>();
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

    private bool ValidSpace(Vector2Int position)
    {
        return position.x >= 0 && position.x <= m_GridWidth - 1 &&
            position.y <= 0 && position.y >= -(m_GridHeight - 1);
    }

    public Vector3 GetWorldPosition(Vector2Int tile)
    {
        return m_Grid.CellToWorld((Vector3Int)tile);
    }
}
