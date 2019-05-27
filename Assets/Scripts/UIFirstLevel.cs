using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFirstLevel : MonoBehaviour
{
    public Button[] _buttons;
    public RectTransform _slider;
    public GameObject _panelPrefab;
    public float _useTime = 0.5f;

    private Dictionary<string, GameObject> _panelDict;
    private GameObject _curPanel;
    private GameObject _nextPanel;
    private int _index = -1;
    private bool _isTweening;

    // Use this for initialization
    void Start()
    {
        Init();
        for (int i = 0; i < _buttons.Length; i++)
        {
            AddEvent(i, _buttons[i]);
        }
    }

    void Init()
    {
        MoveSlider(_buttons[0]);
        MovePanel(0, "Panel0");
    }

    void AddEvent(int index, Button button)
    {
        button.onClick.AddListener(() => MoveSlider(button));
        button.onClick.AddListener(() => MovePanel(index, "Panel" + index));
    }

    void MoveSlider(Button button)
    {
        if (_isTweening) return;
        RectTransform target = button.GetComponent<RectTransform>();
        _slider.DOLocalMoveX(target.localPosition.x, _useTime);
        Vector2 targetVector2 = target.sizeDelta;
        targetVector2.y = _slider.sizeDelta.y;
        _slider.DOSizeDelta(targetVector2, _useTime);
    }

    void MovePanel(int index, string name)
    {
        if (_isTweening) return;
        if (_index == index) return;
        _isTweening = true;
        _nextPanel = GetPanel(name, _index > index);
        if (_curPanel != null) HidePanel(_curPanel, _index < index);
        if (_nextPanel != null) ShowPanel(_nextPanel, ChangePanel);
        _index = index;
    }

    GameObject GetPanel(string name, bool right)
    {
        if (_panelDict == null) _panelDict = new Dictionary<string, GameObject>();
        GameObject obj = null;
        if (!_panelDict.TryGetValue(name, out obj))
        {
            obj = Instantiate(_panelPrefab);
            obj.transform.GetComponent<UIFirstPanel>()._panelName = name;
            obj.transform.SetParent(transform, false);
            obj.name = name;
            _panelDict.Add(name, obj);
        }
        RectTransform rect = obj.GetComponent<RectTransform>();
        if (right) rect.localPosition = new Vector2(rect.rect.width, rect.localPosition.y);
        else rect.localPosition = new Vector2(-rect.rect.width, rect.localPosition.y);
        obj.SetActive(true);
        return obj;
    }

    void HidePanel(GameObject obj, bool right)
    {
        Debug.Log(obj.name + (right ? "向右移动" : "向左移动"));
        RectTransform rect = obj.GetComponent<RectTransform>();
        float targetX = rect.rect.width * (right ? 1 : -1);
        rect.DOLocalMoveX(targetX, _useTime);
    }

    void ShowPanel(GameObject obj, TweenCallback action = null)
    {
        ///Debug.Log(obj.name + "移到中间");
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.DOLocalMoveX(0, _useTime).OnComplete(action);
    }

    void ChangePanel()
    {
        if (_curPanel != null) _curPanel.SetActive(false);
        _curPanel = _nextPanel;
        _curPanel.GetComponent<UIFirstPanel>().CreatCells();
        _isTweening = false;
    }
}
