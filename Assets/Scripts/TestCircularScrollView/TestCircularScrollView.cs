using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TestCircularScrollView : MonoBehaviour
{
    public UICircularScrollView _circularH;
    public UICircularScrollView _circularV;
    public UICircularFlodScrollView _flodCircularH;
    public UICircularFlodScrollView _flodCircularV;

    void Start()
    {
        _circularH.Init(CellCallBack);
        _circularH.ShowList(1000);
        _circularV.Init(CellCallBack);
        _circularV.ShowList(1000);
        _flodCircularH.Init(FlodBtnCallback, FlodCellCallback);
        _flodCircularH.ShowList(2, 3, 4, 5, 6, 7, 8);
        _flodCircularV.Init(FlodBtnCallback, FlodCellCallback);
        _flodCircularV.ShowList(2, 3, 4, 5, 6, 7, 8);
    }

    private void CellCallBack(GameObject cell, int index)
    {
        cell.name = index.ToString();
        cell.transform.Find("Text").GetComponent<Text>().text = index.ToString();
    }

    void FlodCellCallback(GameObject cell, int flodIndex, int cellIndex)
    {
        cell.transform.Find("Text").GetComponent<Text>().text = flodIndex + "-" + cellIndex;
    }

    void FlodBtnCallback(GameObject flod, int index)
    {
        flod.GetComponentInChildren<Text>().text = "折叠按钮" + index.ToString();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _circularH.ShowList(50);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            _circularH.ShowList(120);
        }
    }

}

public class ExpandData
{
    public int index;
    public System.DateTime date;
    public MsgData[] datas;
}

public class ExpandEntity : MonoBehaviour
{
    public ExpandData data;
    public Text date;
    public Text dataCount;
    public Button button;
    public UnityAction<int> ExpandClick;

    void Start()
    {
        button.onClick.AddListener(() => ExpandClick(data.index));
    }

    public void Show(ExpandData data)
    {
        this.data = data;
        date.text = data.date.TimeOfDay.ToString();
        dataCount.text = data.datas.Length.ToString();
    }
}

public class MsgData
{
    public int index;
    public System.DateTime date;
    public string msg;
    public string user;
}

public class MsgEntity : MonoBehaviour
{
    public Text date;
    public Text msg;
    public Text user;
    public void Show(MsgData data)
    {
        date.text = data.date.ToString();
        msg.text = data.msg;
        user.text = data.user;
    }
}