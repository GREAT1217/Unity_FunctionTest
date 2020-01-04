using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

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