using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;

public class Holder : MonoBehaviour
{
    public Action OnLose;

    int slots = 5;
    float cellSize;

    [SerializeField] float offsetY;
    [SerializeField] GameObject cellBg;

    private Grid<Item> grid;
    private GameSettings gameSettings;

    List<Item> items = new List<Item>();
    List<GameObject> bgs = new List<GameObject>();

    public void Init(GameSettings gameSettings)
    {
        this.gameSettings = gameSettings;

        cellSize = gameSettings.CellSize + .2f;

        Vector3 org = new Vector3(-slots * cellSize / 2, offsetY);

        grid = new Grid<Item>(slots, 1, cellSize, org, true);

        AddBackGround();
    }

    public void AddItem(Item item)
    {
        if (item == null) return;

        if (items.Count < slots)
        {
            for (int i = 0; i < slots; i++)
            {
                if (grid.GetGridObject(i, 0) == null)
                {
                    grid.SetGridObject(i, 0, item);

                    item.SetState(Item.State.ON_HOLDER);

                    items.Add(item);

                    item.AnimMoveToPosition(grid.GetCellPos(i, 0),
                        () =>
                        {
                            if (!FindMatch() && items.Count == slots)
                            {
                                OnLose?.Invoke();
                            }
                        });

                    break;
                }
            }

            
        }
    }

    public void RemoveItem(Item item)
    {
        if (item == null) return;
        grid.SetGridObject(item.TF.position, null);
        items.Remove(item);

        ShiftItemLeft();
    }

    public void Clear()
    {
        for (int x = 0; x < slots; x++)
        {
            ClearItem(x, 0);
        }
        items.Clear();

        ClearBackGround();

        void ClearItem(int x, int y)
        {
            Item item = grid.GetGridObject(x, y);
            if (item)
            {
                GameObject.Destroy(item.gameObject);
            }
        }
    }

    void AddBackGround()
    {
        for (int i = 0; i < slots; i++)
        {
            GameObject g = Instantiate(cellBg, transform);
            g.transform.position = grid.GetCellPos(i, 0);
            bgs.Add(g);
        }
    }

    void ClearBackGround()
    {
        for (int i = 0; i < bgs.Count; i++)
        {
            Destroy(bgs[i]);
        }
        bgs.Clear();
    }

    bool FindMatch()
    {
        bool match = false;
        Dictionary<Item.eType, int> itemCount = items.GroupBy(x => x.ItemType).ToDictionary(group => group.Key, group => group.Count());

        List<Item.eType> keys = itemCount.Keys.ToList();
        for (int i = 0; i < keys.Count; i++)
        {
            if (itemCount[keys[i]] >= gameSettings.MatchesMin)
            {
                RemoveMatchedItem(keys[i]);
            }
        }

        return match;
    }

    void RemoveMatchedItem(Item.eType type)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ItemType == type)
            {
                grid.SetGridObject(items[i].TF.position, null);

                items[i].ShowDestroyAnim(() => ShiftItemLeft());

                items.RemoveAt(i);

                i--;
            }
        }
    }

    void ShiftItemLeft()
    {
        for (int i = 1; i < slots; i++)
        {
            if (grid.GetGridObject(i, 0) != null)
            {
                Shift(i);
            }
        }

        void Shift(int x)
        {
            if (x == 0) return;
            while (grid.GetGridObject(x - 1, 0) == null)
            {
                Item temp = grid.GetGridObject(x, 0);

                temp.AnimMoveToPosition(grid.GetCellPos(x - 1, 0));

                grid.SetGridObject(x - 1, 0, temp);
                grid.SetGridObject(x, 0, null);
            }
        }
    }

}
