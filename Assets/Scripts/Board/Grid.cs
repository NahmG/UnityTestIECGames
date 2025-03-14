using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grid<TObject>
{
    public event Action<int, int> OnGridObjectChanged;

    private int width;
    private int height;
    private float cellSize;
    Vector3 originalPos;

    private TObject[,] gridArrays;

    public Grid(int width, int height, float cellSize, Vector3 originalPos, bool debug)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originalPos = originalPos;

        gridArrays = new TObject[width, height];

        if (debug)
        {
            for (int i = 0; i < gridArrays.GetLength(0); i++)
            {
                for (int j = 0; j < gridArrays.GetLength(1); j++)
                {
                    Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }
    }

    public int GetWidth => width;
    public int GetHeight => height;
    public float GetCellSize => cellSize;

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originalPos;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition.x - originalPos.x) / cellSize);
        y = Mathf.FloorToInt((worldPosition.y - originalPos.y) / cellSize);
    }

    public Vector3 GetCellPos(int x, int y)
    {
        Vector3 pos = GetWorldPosition(x, y);

        return new Vector3(pos.x + cellSize / 2, pos.y + cellSize / 2);
    }

    public Vector3 GetCellPos(Vector3 worldPos)
    {
        GetXY(worldPos, out int x, out int y);

        return GetCellPos(x, y);
    }

    public void GetXYFromCell(Vector3 cellPos, out int x, out int y)
    {
        Vector3 worldPos = new Vector3(cellPos.x - cellSize / 2, cellPos.y - cellSize / 2);
        GetXY(worldPos, out x, out y);
    }

    public void SetGridObject(int x, int y, TObject obj)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArrays[x, y] = obj;
            TriggerGridObjectChanged(x, y);
        }
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        OnGridObjectChanged?.Invoke(x, y);
    }

    public void SetGridObject(Vector3 worldPosition, TObject obj)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, obj);
    }

    public TObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArrays[x, y];
        }
        else { return default; }
    }

    public TObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

}
