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

    public Button _button;//收纳按钮
    public Text _content;//当前节点名
    public Tree _tree;//当前节点
    public float _clickInterval = 0.2f;//双击时间间隔

    private GameObject[] _subObjs;//子对象
    private float _time1, _time2;

    public void InitData(Tree tree)
    {
        _tree = tree;
        InitUI();
    }

    /// <summary>
    /// 初始化UI
    /// </summary>
    private void InitUI()
    {
        _button.transform.localPosition = Vector3.right * (_tree._grade * 20 - 100);
        _content.transform.localPosition = Vector3.right * (_tree._grade * 20);
        _content.text = _tree._curNode.name;
        _button.gameObject.SetActive(_tree._subNodes.Length != 0);
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (_button.transform.localEulerAngles.z == 0)//收纳状态
        {
            _button.transform.localEulerAngles = Vector3.forward * -90;
            ShowSubObjs();
        }
        else
        {
            _button.transform.localRotation = Quaternion.identity;
            HideSubObjs();
        }
    }

    private void ShowSubObjs()
    {
        if (_tree._subNodes.Length <= 0) return;
        if (_subObjs != null)
        {
            for (int i = 0; i < _subObjs.Length; i++)
            {
                _subObjs[i].SetActive(true);
            }
        }
        else
        {
            _subObjs = new GameObject[_tree._subNodes.Length];
            int space = -20;
            for (int i = 0; i < _subObjs.Length; i++)
            {
                _subObjs[i] = UITreeManager.InitNode(transform, _tree._subNodes[i], space);
                space -= 20;
            }
        }
        //加入列表
        UITreeManager.InsertRange(gameObject, _subObjs);
    }

    private void HideSubObjs()
    {
        for (int i = 0; i < _subObjs.Length; i++)
        {
            _subObjs[i].SetActive(false);
        }
        //移除列表
        UITreeManager.RemoveRange(gameObject, _subObjs.Length);
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
