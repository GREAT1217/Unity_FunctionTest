using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFirstLevel : MonoBehaviour
{
    public Image[] _buttons;
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

    void AddEvent(int index, Image button)
    {
        //第一种silder
        UIEventTrigger.Add(button).PointerClick += () => MoveSlider(button);
        //第二种slider
        //UIEventTrigger.Add(button.gameObject).onPointerDown = () => MaxSlider(index);
        //UIEventTrigger.Add(button.gameObject).onPointerUp = () => MinSlider(index);
        //UIEventTrigger.Add(button.gameObject).onPointerExit = () => MinSlider(_index);
        //panel
        UIEventTrigger.Add(button).PointerClick += () => MovePanel(index, "Panel" + index);

        //button.onClick.AddListener(() => MoveSlider(button));
        //button.onClick.AddListener(() => MovePanel(index, "Panel" + index));
    }


    void MoveSlider(Image button)
    {
        if (_isTweening) return;
        RectTransform target = button.GetComponent<RectTransform>();
        _slider.DOLocalMoveX(target.localPosition.x, _useTime);
        Vector2 targetVector2 = target.sizeDelta;
        targetVector2.y = _slider.sizeDelta.y;
        _slider.DOSizeDelta(targetVector2, _useTime);
    }

    #region shibai
    //void MaxSlider(int index)
    //{
    //    if (_isTweening) return;
    //    if (_index == index) return;

    //    RectTransform curButton = _buttons[_index].GetComponent<RectTransform>();
    //    RectTransform nextButton = _buttons[index].GetComponent<RectTransform>();
    //    float targetX = Vector2.Distance(curButton.localPosition, nextButton.localPosition) + curButton.sizeDelta.x / 2 + nextButton.sizeDelta.x / 2;
    //    Vector2 localPos = _slider.anchoredPosition;
    //    Debug.Log("locl" + localPos);
    //    if (index > _index)
    //    {
    //        _slider.pivot = Vector2.up / 2;
    //        _slider.localPosition = localPos - Vector2.right * (curButton.sizeDelta.x / 2);
    //    }
    //    else
    //    {
    //        _slider.pivot = Vector2.up / 2 + Vector2.right;
    //        _slider.localPosition = localPos + Vector2.right * (curButton.sizeDelta.x / 2);
    //    }
    //    Debug.Log("locl2" + localPos);
    //    _slider.DOSizeDelta(new Vector2(targetX, _slider.sizeDelta.y), _useTime);
    //} 

    //void MinSlider(int index)
    //{
    //    RectTransform nextButton = _buttons[index].GetComponent<RectTransform>();
    //    float targetX = 0;
    //    if (index > _index)
    //    {
    //        targetX = nextButton.localPosition.x + nextButton.sizeDelta.x / 2;
    //    }
    //    else
    //    {
    //        targetX = nextButton.localPosition.x - nextButton.sizeDelta.x / 2;
    //    }
    //    _slider.DOLocalMoveX(targetX, _useTime);
    //    float pivotX = _slider.pivot.x == 0 ? 1 : 0;
    //    _slider.pivot = new Vector2(pivotX, _slider.pivot.y);
    //    _slider.DOSizeDelta(new Vector2(nextButton.sizeDelta.x, _slider.sizeDelta.y), _useTime);
    //}
    #endregion

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
        //Debug.Log(obj.name + (right ? "向右移动" : "向左移动"));
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
