using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class Ux_TablePool
{
    private readonly RectTransform _root;
    private readonly Stack<RectTransform> _rows = new();
    private readonly Stack<RectTransform> _cells = new();

    public Ux_TablePool(RectTransform root)
    {
        _root = root;
    }

    public bool IsFor(RectTransform root)
    {
        return _root == root;
    }

    public RectTransform RentRow()
    {
        return Rent(_rows, "r", static go => Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Ux_TonkersTableTopiaRow>(go));
    }

    public RectTransform RentCell()
    {
        return Rent(_cells, "c", static go => Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Ux_TonkersTableTopiaCell>(go));
    }

    public void ReturnRow(RectTransform rt)
    {
        Return(rt, _rows);
    }

    public void ReturnCell(RectTransform rt)
    {
        Return(rt, _cells);
    }

    private RectTransform Rent(Stack<RectTransform> pool, string name, Action<GameObject> init)
    {
        if (Application.isPlaying && pool.Count > 0)
        {
            RectTransform rt = pool.Pop();
            rt.gameObject.SetActive(true);
            rt.gameObject.name = name;
            return rt;
        }

        GameObject go = Ux_TonkersTableTopiaObjectUtility.CreateUiObject(name);
        init(go);
        return go.GetComponent<RectTransform>();
    }

    private void Return(RectTransform rt, Stack<RectTransform> pool)
    {
        if (!Application.isPlaying || rt == null || _root == null)
            return;

        rt.gameObject.SetActive(false);
        rt.SetParent(_root, false);
        pool.Push(rt);
    }
}