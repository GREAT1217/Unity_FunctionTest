using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI引导形状
/// </summary>
public enum EGuideShape
{
    /// <summary>
    /// 圆形
    /// </summary>
    Circle,
    /// <summary>
    /// 矩形
    /// </summary>
    Rect,
}

/// <summary>
/// 引导类型
/// </summary>
public enum EGuideType
{
    /// <summary>
    /// 无
    /// </summary>
    None,
    /// <summary>
    /// 按键
    /// </summary>
    InputDown,
    /// <summary>
    /// 鼠标按下
    /// </summary>
    MouseDown,
    /// <summary>
    /// 鼠标拖拽
    /// </summary>
    MouseDrag,
    /// <summary>
    /// 鼠标滚轮
    /// </summary>
    MouseWheel,
    /// <summary>
    /// 触发
    /// </summary>
    Path,
    /// <summary>
    /// 计时
    /// </summary>
    Timer,
    /// <summary>
    /// UI
    /// </summary>
    UI,
    /// <summary>
    /// 事件监听
    /// </summary>
    EventListen,
}

/// <summary>
/// 事件引导类型
/// </summary>
public enum EEventGuideType
{
    None,
    SingleClick,
    DoubleClick,
    GREAT,//可扩充，在GuideEvents中添加事件检测，在GuideManager中绑定回调方法
}

/// <summary>
/// 引导数据基类接口
/// </summary>
public interface IGuideData
{
    EGuideType GType { get; }
    int GIndex { get; set; }
    string GInfo { get; set; }
}

/// <summary>
/// 键盘按键引导
/// </summary>
public struct GuideIDown : IGuideData
{
    public EGuideType GType { get { return EGuideType.InputDown; } }
    public int GIndex { get; set; }
    public string GInfo { get; set; }

    /// <summary>
    /// 按键
    /// </summary>
    public KeyCode gInput;

    public GuideIDown(KeyCode gInput) : this()
    {
        this.gInput = gInput;
    }
}

/// <summary>
/// 鼠标按键引导
/// </summary>
public struct GuideMDown : IGuideData
{
    public EGuideType GType { get { return EGuideType.MouseDown; } }
    public int GIndex { get; set; }
    public string GInfo { get; set; }

    /// <summary>
    /// 鼠标按键
    /// </summary>
    public int gKey;
    /// <summary>
    /// 长按时长
    /// </summary>
    public float gTime;
    /// <summary>
    /// 对象路径
    /// </summary>
    public string gPath;

    public GuideMDown(int gKey = 0, float gTime = 0, string gPath = null) : this()
    {
        this.gKey = gKey;
        this.gTime = gTime;
        this.gPath = gPath;
    }
}

/// <summary>
/// 鼠标拖拽引导
/// </summary>
public struct GuideMDrag : IGuideData
{
    public EGuideType GType { get { return EGuideType.MouseDrag; } }
    public int GIndex { get; set; }
    public string GInfo { get; set; }

    /// <summary>
    /// 鼠标按键
    /// </summary>
    public int gKey;
    /// <summary>
    /// 拖拽方向
    /// </summary>
    public string gDir;
    /// <summary>
    /// 拖拽阈值
    /// </summary>
    public float gValue;

    public GuideMDrag(int gKey = 0, string gDir = "Mouse X", float gValue = 0.5f) : this()
    {
        this.gKey = gKey;
        this.gDir = gDir;
        this.gValue = gValue;
    }
}

/// <summary>
/// 鼠标滚轮引导
/// </summary>
public struct GuideMWheel : IGuideData
{
    public EGuideType GType { get { return EGuideType.MouseWheel; } }
    public int GIndex { get; set; }
    public string GInfo { get; set; }

    /// <summary>
    /// 滑动阈值
    /// </summary>
    public float gValue;

    public GuideMWheel(float gValue = 1f) : this()
    {
        this.gValue = gValue;
    }
}

/// <summary>
/// 路径引导
/// </summary>
public struct GuidePath : IGuideData
{
    public EGuideType GType { get { return EGuideType.Path; } }
    public int GIndex { get; set; }
    public string GInfo { get; set; }

    /// <summary>
    /// 对象路径
    /// </summary>
    public string gPath;

    public GuidePath(string gPath) : this()
    {
        this.gPath = gPath;
    }
}

/// <summary>
/// 计时引导
/// </summary>
public struct GuideTimer : IGuideData
{
    public EGuideType GType { get { return EGuideType.Timer; } }
    public int GIndex { get; set; }
    public string GInfo { get; set; }

    /// <summary>
    /// 时长
    /// </summary>
    public float gTime;
    /// <summary>
    /// 对象路径
    /// </summary>
    public string gPath;

    public GuideTimer(float gTime, string gPath = null) : this()
    {
        this.gTime = gTime;
        this.gPath = gPath;
    }
}

/// <summary>
/// UI引导
/// </summary>
public struct GuideUI : IGuideData
{
    public EGuideType GType { get { return EGuideType.UI; } }
    public int GIndex { get; set; }
    public string GInfo { get; set; }

    /// <summary>
    /// 对象路径
    /// </summary>
    public string gPath;
    /// <summary>
    /// 镂空形状
    /// </summary>
    public EGuideShape gShape;

    public GuideUI(string gPath, EGuideShape gShape) : this()
    {
        this.gPath = gPath;
        this.gShape = gShape;
    }
}

/// <summary>
/// 事件引导
/// </summary>
public struct GuideEvent : IGuideData
{
    public EGuideType GType { get { return EGuideType.EventListen; } }
    public int GIndex { get; set; }
    public string GInfo { get; set; }

    /// <summary>
    /// 事件对象
    /// </summary>
    public string gPath;
    /// <summary>
    /// 事件类型
    /// </summary>
    public EEventGuideType gEvent;

    public GuideEvent(string gPath, EEventGuideType gEvent) : this()
    {
        this.gPath = gPath;
        this.gEvent = gEvent;
    }
}
