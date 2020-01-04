using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 树形节点
/// </summary>
public class UITreeNode : MonoBehaviour, IPointerClickHandler
{
    public UnityAction SingleClick;
    public UnityAction DoubleClick;

    private UITreeManager _mgr;
    private bool _isExpand;//是否展开
    public Button _button;//收纳按钮
    public Text _content;//当前节点名
    public FileNode _node;//当前节点
    public float _clickInterval = 0.2f;//双击时间间隔

    private UITreeNode[] _subObjs;//子对象
    private float _time1, _time2;

    public bool IsExpand
    {
        get
        {
            return _isExpand;
        }
        set
        {
            _isExpand = value;
            _button.transform.localEulerAngles = Vector3.forward * (value ? -90 : 0);
        }
    }

    public void InitData(FileNode node, UITreeManager mgr)
    {
        _mgr = mgr;
        _node = node;
        InitUI();
    }

    /// <summary>
    /// 初始化UI
    /// </summary>
    private void InitUI()
    {
        //设置按钮和内容位置
        _button.transform.localPosition = Vector3.right * (_node._grade * 20 - 100);
        _content.transform.localPosition = Vector3.right * (_node._grade * 20);
        _content.text = _node._fileName;
        _button.gameObject.SetActive(_node._hasChild);
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (_button.transform.localEulerAngles.z == 0)//收纳状态
        {
            ShowSubObjs();
            _mgr.InsertRange(this, _subObjs);
            IsExpand = true;
        }
        else
        {
            int length = 0;
            HideSubObjs(this, ref length);
            _mgr.RemoveRange(this, length);
            IsExpand = false;
        }
    }

    private void ShowSubObjs()
    {
        if (_subObjs != null)
        {
            for (int i = 0; i < _subObjs.Length; i++)
            {
                _subObjs[i].gameObject.SetActive(true);
            }
        }
        else
        {
            _node._childNodes = _mgr.GetFile(_node._filePath, _node._grade + 1);
            _subObjs = new UITreeNode[_node._childNodes.Length];
            for (int i = 0; i < _subObjs.Length; i++)
            {
                //_subObjs[i] = _mgr.InitNode(_mgr._root, _node._childNodes[i]);//设置在根目录
                _subObjs[i] = _mgr.InitNode(transform, _node._childNodes[i]);
            }
        }
    }

    private void HideSubObjs(UITreeNode node, ref int length)
    {
        length += node._subObjs.Length;
        for (int i = 0; i < node._subObjs.Length; i++)
        {
            node._subObjs[i].gameObject.SetActive(false);
            if (node._subObjs[i].IsExpand)
            {
                node._subObjs[i].IsExpand = false;
                HideSubObjs(node._subObjs[i], ref length);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SingleClickEvent();
        DoubleClickEvent();
    }

    /// <summary>
    /// 单击事件
    /// </summary>
    private void SingleClickEvent()
    {
        if (SingleClick != null)
        {
            SingleClick();
        }
    }

    /// <summary>
    /// 双击事件
    /// </summary>
    private void DoubleClickEvent()
    {
        _time1 = Time.timeSinceLevelLoad;
        if (_time1 - _time2 < _clickInterval)
        {
            if (DoubleClick != null)
            {
                DoubleClick();
            }
        }
        _time2 = _time1;
    }
}
