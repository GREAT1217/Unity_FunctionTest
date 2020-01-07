using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public enum UIDir
{
    Horizontal,
    Vertical
}

public class UIAutoCenterScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public UIDir _scrollDir;
    public UIAutoCenterItem _itemPrefab;
    public int _itemSpacing;//间隔
    public Vector3 _centerScale = Vector3.one * 1.2f;//中心放大

    private ScrollRect _scrollRect;
    private int _itemCount;
    private UIAutoCenterItem[] _items;
    private int _centerIndex = -1;
    private bool _posing;
    private float _targetPos;

    void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _scrollRect.horizontal = _scrollDir == UIDir.Horizontal ? true : false;
        _scrollRect.vertical = _scrollDir == UIDir.Vertical ? true : false;
        HorizontalOrVerticalLayoutGroup layout;
        if (_scrollDir == UIDir.Horizontal)
        {
            layout = _scrollRect.content.gameObject.AddComponent<HorizontalLayoutGroup>();
            RectTransform rectTrans = _itemPrefab.GetComponent<RectTransform>();
            float margin = (_scrollRect.viewport.rect.width - rectTrans.rect.width) / 2;
            layout.padding.left = layout.padding.right = (int)margin;
            _scrollRect.content.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        else
        {
            layout = _scrollRect.content.gameObject.AddComponent<VerticalLayoutGroup>();
            RectTransform rectTrans = _itemPrefab.GetComponent<RectTransform>();
            float margin = (_scrollRect.viewport.rect.height - rectTrans.rect.height) / 2;
            layout.padding.top = layout.padding.bottom = (int)margin;
            _scrollRect.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        layout.childControlHeight = false;
        layout.childControlWidth = false;
        layout.spacing = _itemSpacing;
        layout.childAlignment = TextAnchor.MiddleCenter;
    }

    void Update()
    {
        if (_posing)
        {
            if (_scrollDir == UIDir.Horizontal)
            {
                _scrollRect.horizontalNormalizedPosition = Mathf.Lerp(_scrollRect.horizontalNormalizedPosition, _targetPos, _scrollRect.elasticity);
                if (Mathf.Abs(_scrollRect.horizontalNormalizedPosition - _targetPos) < 0.005f)
                {
                    _scrollRect.horizontalNormalizedPosition = _targetPos;
                    _posing = false;
                }
            }
            else
            {
                _scrollRect.verticalNormalizedPosition = Mathf.Lerp(_scrollRect.verticalNormalizedPosition, _targetPos, _scrollRect.elasticity);
                if (Mathf.Abs(_scrollRect.verticalNormalizedPosition - _targetPos) < 0.005f)
                {
                    _scrollRect.verticalNormalizedPosition = _targetPos;
                    _posing = false;
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _posing = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        CheckNearstIndex();
        SetItemScale();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetItemAndContent();
    }

    public void InitItem(UnityAction<UIAutoCenterItem, int> show, int count, int center = 0)
    {
        _itemCount = count;
        _items = new UIAutoCenterItem[count];
        for (int i = 0; i < count; i++)
        {
            Debug.Log(_itemPrefab.name);
            Debug.Log(_scrollRect.name);
            UIAutoCenterItem item = Instantiate(_itemPrefab, _scrollRect.content);
            show(item, i);
            _items[i] = item;
        }
        SetCenterItem(center);
    }

    public void SetCenterItem(int index)
    {
        if (index == _centerIndex) return;
        _centerIndex = index;
        SetItemScale();
        SetItemAndContent();
    }

    void CheckNearstIndex()
    {
        float pos = _scrollDir == UIDir.Horizontal ? _scrollRect.horizontalNormalizedPosition : _scrollRect.verticalNormalizedPosition;
        float nearstDis = Mathf.Abs(0 - pos);
        int index = 0;
        for (int i = 1; i < _itemCount; i++)
        {
            float tempPos = (float)i / (_itemCount - 1);
            float tempDis = Mathf.Abs(tempPos - pos);
            if (tempDis < nearstDis)
            {
                nearstDis = tempDis;
                index = i;
            }
        }
        _centerIndex = _scrollDir == UIDir.Horizontal ? index : _itemCount - 1 - index;
    }

    void SetItemScale()
    {
        for (int i = 0; i < _itemCount; i++)
        {
            if (_centerIndex == i)
            {
                _items[i].transform.localScale = _centerScale;
            }
            else
            {
                _items[i].transform.localScale = Vector3.one;
            }
        }
    }

    void SetItemAndContent()
    {
        _items[_centerIndex].OnCenter();
        _posing = true;
        if (_scrollDir == UIDir.Horizontal)
        {
            _targetPos = (float)_centerIndex / (_itemCount - 1);
        }
        else
        {
            _targetPos = (float)(_itemCount - 1 - _centerIndex) / (_itemCount - 1);
        }
    }
}