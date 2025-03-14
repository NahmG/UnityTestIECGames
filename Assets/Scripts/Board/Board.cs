using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class Board : MonoBehaviour
{
    public Action OnWin;

    private int fieldWidth;
    private int fieldHeight;
    private float cellSize;

    [SerializeField] float offsetY;

    private Item itemPref;

    private Grid<Item> grid;

    private Transform m_root;

    private Holder m_holder;

    private BoardController controller;

    List<Item> items = new List<Item>();

    public void Init(Transform tf, GameSettings gameSetting, Item itemPref, Holder holder, BoardController controller)
    {
        this.fieldWidth = gameSetting.FieldWidth;
        this.fieldHeight = gameSetting.FieldHeigth;
        this.cellSize = gameSetting.CellSize;

        this.m_root = tf;
        this.itemPref = itemPref;

        this.m_holder = holder;
        this.controller = controller;

        Vector3 boardOrg = new Vector3(-fieldWidth * cellSize / 2, offsetY);

        grid = new Grid<Item>(fieldWidth, fieldHeight, cellSize, boardOrg, false);
    }

    public void FirstTimeFill()
    {
        int totalItem = fieldWidth * fieldHeight;

        List<Item.eType> list = new List<Item.eType>();

        Array values = Enum.GetValues(typeof(Item.eType));

        while (totalItem > 0)
        {
            foreach (Item.eType type in values)
            {
                for (int i = 0; i < 3; i++)
                {
                    list.Add(type);
                    totalItem--;
                }
            }
        }

        Utils.Shuffle(list);


        for (int i = 0; i < fieldWidth; i++)
        {
            for (int j = 0; j < fieldHeight; j++)
            {
                Item newItem = GameObject.Instantiate(itemPref, m_root);

                newItem.TF.position = grid.GetCellPos(i, j);
                newItem.SetType(list[fieldWidth * j + i]);
                newItem.SetView();
                newItem.SetState(Item.State.ON_BOARD);
                newItem.BoardXY.Set(i, j);
                newItem.ShowAppearAnim();

                newItem.OnItemSelected += controller.HandleItemSelected;

                grid.SetGridObject(i, j, newItem);

                items.Add(newItem);
            }
        }
    }

    public void FillBoard()
    {
        for (int i = 0; i < fieldWidth; i++)
        {
            for (int j = 0; j < fieldHeight; j++)
            {
                Item newItem = GameObject.Instantiate(itemPref, m_root);

                newItem.TF.position = grid.GetCellPos(i, j);
                newItem.SetType(Utils.GetRandomNormalType());
                newItem.SetView();
                newItem.SetState(Item.State.ON_BOARD);
                newItem.BoardXY.Set(i, j);
                newItem.ShowAppearAnim();

                newItem.OnItemSelected += controller.HandleItemSelected;

                grid.SetGridObject(i, j, newItem);

                items.Add(newItem);
            }
        }
    }


    public void Clear()
    {
        foreach(Item i in items)
        {
            Destroy(i.gameObject);
        }
        items.Clear();
    }

    public void AddItem(Item item)
    {
        if (item == null) return;
        grid.SetGridObject(item.BoardXY.x, item.BoardXY.y, item);

        item.AnimMoveToPosition(grid.GetCellPos(item.BoardXY.x, item.BoardXY.y));

        item.SetState(Item.State.ON_BOARD);

        items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        if (item == null) return;

        items.Remove(item);
        grid.SetGridObject(item.TF.position, null);

        if (items.Count <= 0)
        {
            StartCoroutine(OnBoardEmpty());
        }
    }

    IEnumerator OnBoardEmpty()
    {
        yield return new WaitForSeconds(1f);
        OnWin?.Invoke();
    }
}
