using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIFirstPanel : MonoBehaviour
{
    public string _panelName;
    public GameObject _cellPrefab;
    public float _useTime = 0.2f;

    private List<GameObject> _cellList;
    private int _cellNum = 9;

    public void CreatCells()
    {
        if (_cellList != null) return;
        _cellList = new List<GameObject>();
        InitCell();
    }

    //tween
    void InitCell()
    {
        GameObject cell = CreatCell();
        if (cell == null) return;
        cell.SetActive(true);
        cell.GetComponent<CanvasGroup>().DOFade(1, _useTime).OnStart(InitCell);
    }

    GameObject CreatCell()
    {
        if (_cellList.Count >= _cellNum) return null;
        GameObject cell = Instantiate(_cellPrefab);
        string text= _panelName + "'s Button" + _cellList.Count;
        cell.GetComponentInChildren<Text>().text = text;
        cell.GetComponent<Button>().onClick.AddListener(()=>CellClick(text));
        cell.transform.SetParent(transform,false);
        _cellList.Add(cell);
        return cell;
    }

    void CellClick(string text)
    {
        UIController.Instance._secondLevel.ShowPanel(text);
    }

    //计算GridLayoutGroup

}
