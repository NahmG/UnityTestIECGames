using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class Item : MonoBehaviour
{
    public Action<Item> OnItemSelected;

    [field: SerializeField] public Transform TF { get; private set; }

    [SerializeField] Transform View;

    public enum State
    {
        ON_BOARD,
        ON_HOLDER
    }

    public State state;

    [Serializable]
    public struct BoardCordinate
    {
        public int x, y;

        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public BoardCordinate BoardXY;

    public void SetState(State state)
    {
        this.state = state;
    }

    public void SetView()
    {
        string prefabname = GetPrefabName();

        if (!string.IsNullOrEmpty(prefabname))
        {
            GameObject prefab = Resources.Load<GameObject>(prefabname);
            if (prefab)
            {
                Instantiate(prefab, View);
            }
        }
    }

    #region Type
    public enum eType
    {
        TYPE_ONE,
        TYPE_TWO,
        TYPE_THREE,
        TYPE_FOUR,
        TYPE_FIVE,
        TYPE_SIX,
        TYPE_SEVEN
    }

    public eType ItemType;

    public void SetType(eType type)
    {
        ItemType = type;
    }

    string GetPrefabName()
    {
        string prefabname = string.Empty;
        switch (ItemType)
        {
            case eType.TYPE_ONE:
                prefabname = Constants.PREFAB_NORMAL_TYPE_ONE;
                break;
            case eType.TYPE_TWO:
                prefabname = Constants.PREFAB_NORMAL_TYPE_TWO;
                break;
            case eType.TYPE_THREE:
                prefabname = Constants.PREFAB_NORMAL_TYPE_THREE;
                break;
            case eType.TYPE_FOUR:
                prefabname = Constants.PREFAB_NORMAL_TYPE_FOUR;
                break;
            case eType.TYPE_FIVE:
                prefabname = Constants.PREFAB_NORMAL_TYPE_FIVE;
                break;
            case eType.TYPE_SIX:
                prefabname = Constants.PREFAB_NORMAL_TYPE_SIX;
                break;
            case eType.TYPE_SEVEN:
                prefabname = Constants.PREFAB_NORMAL_TYPE_SEVEN;
                break;
        }

        return prefabname;
    }

    internal bool IsSameType(Item other)
    {
        return other != null && other.ItemType == this.ItemType;
    }

    #endregion

    #region Anim
    internal void AnimMoveToPosition(Vector3 position)
    {
        TF.DOMove(position, 0.2f);
    }

    internal void AnimMoveToPosition(Vector3 position, TweenCallback callback)
    {
        TF.DOMove(position, 0.2f).OnComplete(() => callback?.Invoke());
    }

    internal void ShowAppearAnim()
    {
        Vector3 scale = TF.localScale;
        TF.localScale = Vector3.one * 0.1f;
        TF.DOScale(scale, 0.1f);
    }

    internal void ShowDestroyAnim(TweenCallback callback)
    {
        TF.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(
            () =>
            {
                callback?.Invoke();
                Destroy(TF.gameObject);
            }
            );
    }
    #endregion

    private void OnMouseDown()
    {
        OnItemSelected?.Invoke(this);
    }
}
