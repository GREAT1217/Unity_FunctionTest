using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IUIGuide
{
    /// <summary>
    /// 目标
    /// </summary>
    RectTransform Target { get; }

    /// <summary>
    /// 画布
    /// </summary>
    Canvas CurrentCanvas { get; }

    /// <summary>
    /// 绑定的对象
    /// </summary>
    GameObject gameObject { get; }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="target"></param>
    /// <param name="canvas"></param>
    void SetTarget(RectTransform target, Canvas canvas);
}

/// <summary>
/// 引导管理器
/// </summary>
public class GuideManager : MonoBehaviour
{
    public Canvas _guideCanvas;//
    public GuideUICircle _circleUIGuide;//圆形UI引导
    public GuideUIRect _rectUIGuide;//矩形UI引导
    public RectTransform _guideArrow;//引导箭头
    public Text _guideText;//引导文本
    public GuidePathLine _guideLine;//引导线
    public Transform _player;//引导对象
    [HideInInspector]
    public List<IGuideData> _guideDatas;//引导数据列表

    private EGuideType _curGuideType;//当前引导类型
    private int _curGuideIndex = -1;//当前引导索引
    private IUIGuide _curUIGuide;//当前UI引导
    private KeyCode _curGuideKey;//当前引导按键类型
    private int _curMouseKey = -1;//当前引导鼠标按键值
    private float _curMouseValue = -1;//当前引导鼠标滑轮值
    private string _curMouseDir;//当前引导鼠标移动方向
    private GameObject _curGuideObj;//当前引导对象
    private Button _curGuideBtn;//当前引导按钮对象
    private GuideTrigger _tempTrigger;//临时触发检测组件
    private float _curGuideTimer = -1;//当前引导计时
    private float _tempTimer = 0;//临时计时数据
    private RaycastHit _hit;//
    private Ray _ray;//

    void Start()
    {
        _tempTrigger = _player.gameObject.AddComponent<GuideTrigger>();//不需要触发检测不加
        InitGuideData();
        if (_guideDatas != null) StartGuide(0);//开始引导
    }

    private void InitGuideData()
    {
        _guideDatas = new List<IGuideData>();//GIndex暂时没用到，直接按列表索引来的
        _guideDatas.Add(new GuideUI("GuideObjects/GuideCanvas/RectButton", EGuideShape.Rect) { GInfo = "点击矩形按钮" });
        _guideDatas.Add(new GuideUI("GuideObjects/GuideCanvas/CircleImage", EGuideShape.Circle) { GInfo = "点击圆形图片" });
        _guideDatas.Add(new GuideIDown(KeyCode.Space) { GInfo = "按下空格键" });
        _guideDatas.Add(new GuideMDown(0, 2, "GuideObjects/GreenCube") { GInfo = "左键长按绿色方块两秒" });
        _guideDatas.Add(new GuideMDrag(0, "Mouse Y") { GInfo = "左键按下拖拽鼠标前后移动" });
        _guideDatas.Add(new GuideMWheel() { GInfo = "滑动鼠标滚轮" });
        _guideDatas.Add(new GuidePath("GuideObjects/BlueSphere") { GInfo = "WASD移动到蓝色球" });
        _guideDatas.Add(new GuideTimer(2, "GuideObjects/BlueSphere") { GInfo = "在蓝色球等两秒" });
        _guideDatas.Add(new GuideEvent("GuideObjects/GreenCube", EEventGuideType.DoubleClick) { GInfo = "双击绿色方块" });
    }

    void Update()
    {
        switch (_curGuideType)
        {
            //case EGuideType.None:
            //case EGuideType.Path:
            //case EGuideType.UI:
            //case EGuideType.EventListen:
            //    break;
            case EGuideType.InputDown:
                JudgeKey();
                break;
            case EGuideType.MouseDown:
                JudgeMouseDown();
                break;
            case EGuideType.MouseDrag:
                JudgeMouseDrag();
                break;
            case EGuideType.MouseWheel:
                JudgeMouseWheel();
                break;
            case EGuideType.Timer:
                JudgeTimer();
                break;
        }
    }

    /// <summary>
    /// 开始引导
    /// </summary>
    /// <param name="guideIndex"></param>
    private void StartGuide(int index)
    {
        _curGuideIndex = index;
        IGuideData guide = _guideDatas[index];
        switch (guide.GType)
        {
            case EGuideType.InputDown:
                SetKeyGuide(((GuideIDown)guide).gInput, guide.GInfo);
                break;
            case EGuideType.Path:
                SetPathGuide(((GuidePath)guide).gPath, guide.GInfo);
                break;
            case EGuideType.MouseDrag:
                GuideMDrag drag = (GuideMDrag)guide;
                SetMouseDragGuide(drag.gKey, drag.gDir, drag.gValue, guide.GInfo);
                break;
            case EGuideType.MouseDown:
                GuideMDown down = (GuideMDown)guide;
                SetMouseDownGuide(down.gKey, down.gTime, down.gPath, guide.GInfo);
                break;
            case EGuideType.MouseWheel:
                SetMouseWheelGuide(((GuideMWheel)guide).gValue, guide.GInfo);
                break;
            case EGuideType.Timer:
                GuideTimer timer = (GuideTimer)guide;
                SetTimerGuide(timer.gTime, timer.gPath, guide.GInfo);
                break;
            case EGuideType.UI:
                GuideUI ui = (GuideUI)guide;
                SetUIGuide(ui.gPath, ui.gShape, guide.GInfo);
                break;
            case EGuideType.EventListen:
                GuideEvent even = (GuideEvent)guide;
                SetEventGuide(even.gPath, even.gEvent, guide.GInfo);
                break;
        }
    }

    /// <summary>
    /// 结束引导
    /// </summary>
    /// <param name="index"></param>
    public void EndGuide()
    {
        HideTip();
        _curGuideIndex++;
        if (_curGuideIndex < _guideDatas.Count)
        {
            StartGuide(_curGuideIndex);
        }
        else
        {
            ShowTip("完美");
        }
    }

    #region 引导内容
    /// <summary>
    /// 按键引导
    /// </summary>
    /// <param name="key"></param>
    /// <param name="info"></param>
    private void SetKeyGuide(KeyCode key, string info)
    {
        ShowTip(info);
        _curGuideKey = key;
        _curGuideType = EGuideType.InputDown;
    }
    /// <summary>
    /// 检查按键
    /// </summary>
    private void JudgeKey()
    {
        if (_curGuideKey != KeyCode.None && Input.GetKeyDown(_curGuideKey))
        {
            _curGuideType = EGuideType.None;
            _curGuideKey = KeyCode.None;
            EndGuide();
        }
    }

    /// <summary>
    /// 路径引导
    /// </summary>
    /// <param name="objPath"></param>
    /// <param name="info"></param>
    private void SetPathGuide(string objPath, string info)
    {
        ShowTip(info);
        _curGuideObj = GameObject.Find(objPath);
        _curGuideType = EGuideType.Path;
        _tempTrigger.TriggerEnter = EndPathGuide;//使用触发检测终点
        if (_curGuideObj != null )
        {
            if (!_curGuideObj.activeInHierarchy)
            {
                _curGuideObj.gameObject.SetActive(true);
            }
            _curGuideObj.GetComponent<MeshRenderer>().enabled = true;
            _guideLine.ShowLine(_player.gameObject, _curGuideObj);
        }
    }
    /// <summary>
    /// 检查触发终点
    /// </summary>
    /// <param name="other"></param>
    private void EndPathGuide(Collider other)
    {
        if (_curGuideObj == null) return;
        string name = _curGuideObj.name;
        if (other.name == name)
        {
            _curGuideType = EGuideType.None;
            _curGuideObj = null;
            _guideLine.HideLine();
            _tempTrigger.TriggerEnter = null;
            EndGuide();
        }
    }

    /// <summary>
    /// 鼠标拖拽引导
    /// </summary>
    /// <param name="mouseKey"></param>
    /// <param name="info"></param>
    private void SetMouseDragGuide(int mouseKey, string mouseDir, float dragValue, string info)
    {
        ShowTip(info);
        _curMouseKey = mouseKey;
        _curMouseDir = mouseDir;
        _curMouseValue = dragValue;
        _curGuideType = EGuideType.MouseDrag;
    }
    /// <summary>
    /// 检查鼠标拖拽
    /// </summary>
    private void JudgeMouseDrag()
    {
        if (_curMouseDir != "")
        {
            if (Input.GetMouseButton(_curMouseKey))
            {
                if (Mathf.Abs(Input.GetAxis(_curMouseDir)) >= _curMouseValue)
                {
                    EndMouseDragGuide();
                }
            }
        }
        else
        {
            if (Mathf.Abs(Input.GetAxis(_curMouseDir)) >= _curMouseValue)
            {
                EndMouseDragGuide();
            }
        }
    }
    /// <summary>
    /// 结束鼠标拖拽引导
    /// </summary>
    private void EndMouseDragGuide()
    {
        _curGuideType = EGuideType.None;
        _curMouseKey = -1;
        _curMouseDir = null;
        _curMouseValue = -1;
        EndGuide();
    }

    /// <summary>
    /// 鼠标按键引导
    /// </summary>
    /// <param name="mouseKey"></param>
    /// <param name="info"></param>
    /// <param name="objPath"></param>
    /// <param name="time"></param>
    private void SetMouseDownGuide(int mouseKey, float time, string objPath, string info)
    {
        ShowTip(info);
        _curMouseKey = mouseKey;
        _curGuideObj = GameObject.Find(objPath);
        _curGuideTimer = time;
        _curGuideType = EGuideType.MouseDown;
    }
    /// <summary>
    /// 检查鼠标按键
    /// </summary>
    private void JudgeMouseDown()
    {
        if (_curGuideObj != null)
        {
            if (Input.GetMouseButton(_curMouseKey))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out _hit))
                {
                    if (_hit.transform.name == _curGuideObj.name)
                    {
                        Debug.Log(_tempTimer);
                        _tempTimer += Time.deltaTime;
                        if (_tempTimer >= _curGuideTimer)
                        {
                            EndMouseDownGuide();
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(_curMouseKey))
            {
                _tempTimer += Time.deltaTime;
                if (_tempTimer > _curGuideTimer)
                {
                    EndMouseDownGuide();
                }
            }
        }
    }
    /// <summary>
    /// 结束按键引导
    /// </summary>
    private void EndMouseDownGuide()
    {
        _curGuideObj = null;
        _curGuideType = EGuideType.None;
        _curMouseKey = -1;
        _tempTimer = 0;
        _curGuideTimer = -1;
        EndGuide();
    }

    /// <summary>
    /// 鼠标滚轮引导
    /// </summary>
    /// <param name="wheelValue"></param>
    /// <param name="info"></param>
    private void SetMouseWheelGuide(float wheelValue, string info)
    {
        ShowTip(info);
        _curMouseValue = wheelValue;
        _curGuideType = EGuideType.MouseWheel;
    }
    /// <summary>
    /// 检查鼠标滚轮
    /// </summary>
    private void JudgeMouseWheel()
    {
        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > _curMouseValue)
        {
            EndMouseWheelGuide();
        }
    }
    /// <summary>
    /// 结束鼠标滚轮引导
    /// </summary>
    private void EndMouseWheelGuide()
    {
        _curMouseValue = -1;
        _curGuideType = EGuideType.None;
        EndGuide();
    }

    /// <summary>
    /// 计时引导
    /// </summary>
    /// <param name="time"></param>
    /// <param name="info"></param>
    private void SetTimerGuide(float time, string objPath, string info)
    {
        ShowTip(info);
        _curGuideObj = GameObject.Find(objPath);
        if (_curGuideObj != null) _tempTrigger.TriggerStay = JudgeTimer;
        _curGuideTimer = time;
        _curGuideType = EGuideType.Timer;
    }
    /// <summary>
    /// 检测计时
    /// 单纯的计时
    /// </summary>
    private void JudgeTimer()
    {
        if (_curGuideObj != null) return;
        _tempTimer += Time.deltaTime;
        if (_tempTimer >= _curGuideTimer)
        {
            EndTimerGuide();
        }
    }
    /// <summary>
    /// 检测计时
    /// 需要触发
    /// </summary>
    /// <param name="other"></param>
    private void JudgeTimer(Collider other, float time)
    {
        if (_curGuideObj == null) return;
        string name = _curGuideObj.name;
        if (other.name != name) return;
        if (time >= _curGuideTimer)
        {
            EndTimerGuide();
        }
    }
    /// <summary>
    /// 结束计时引导
    /// </summary>
    private void EndTimerGuide()
    {
        _curGuideType = EGuideType.None;
        _tempTimer = 0;
        _curGuideTimer = 0;
        _curGuideObj = null;
        _tempTrigger.TriggerStay = null;
        EndGuide();
    }

    /// <summary>
    /// UI引导
    /// </summary>
    /// <param name="uiPath"></param>
    /// <param name="shape"></param>
    /// <param name="info"></param>
    private void SetUIGuide(string uiPath, EGuideShape shape, string info)
    {
        ShowTip(info);
        StartCoroutine(WaitLoad(uiPath, shape));
        _curGuideType = EGuideType.UI;
    }
    /// <summary>
    /// 等待加载UI
    /// 避免动态加载的报错
    /// </summary>
    /// <param name="ui"></param>
    /// <param name="shape"></param>
    /// <returns></returns>
    private IEnumerator WaitLoad(string uiPath, EGuideShape shape)
    {
        //while (_curGuideObj == null)
        //{
        //    yield return null;
        //    _curGuideObj = GameObject.Find(uiPath);
        //}
        yield return new WaitUntil(() => (_curGuideObj = GameObject.Find(uiPath)) != null);
        RectTransform target = _curGuideObj.GetComponent<RectTransform>();
        SetGuideMask(target, shape);
        SetGuideArrow(target);
        _curGuideBtn = _curGuideObj.GetComponent<Button>();
        if (_curGuideBtn)
        {
            _curGuideBtn.onClick.AddListener(EndUIGuide);
        }
        else
        {
            _curGuideObj.gameObject.AddComponent<GuideEvents>().SingleClick += EndUIGuide;
        }
    }
    /// <summary>
    /// 设置引导遮罩
    /// </summary>
    /// <param name="target"></param>
    /// <param name="maskShape"></param>
    private void SetGuideMask(RectTransform target, EGuideShape maskShape)
    {
        switch (maskShape)
        {
            case EGuideShape.Circle:
                _curUIGuide = _circleUIGuide;
                break;
            case EGuideShape.Rect:
                _curUIGuide = _rectUIGuide;
                break;
        }
        _curUIGuide.SetTarget(target, _guideCanvas);
        _curUIGuide.gameObject.SetActive(true);
    }
    /// <summary>
    /// 设置引导箭头
    /// </summary>
    /// <param name="target"></param>
    private void SetGuideArrow(RectTransform target)
    {
        _guideArrow.gameObject.SetActive(true);
        float posX = target.position.x;
        float posY = target.position.y;
        Vector3 offset = Vector3.zero;
        if (posX >= Screen.width / 2)
        {
            if (posY >= Screen.height / 2)
                offset = new Vector3(-_guideArrow.rect.width / 2, -_guideArrow.rect.height / 2, 0);
            else
                offset = new Vector3(-_guideArrow.rect.width / 2, _guideArrow.rect.height / 2, 0);
        }
        else
        {
            if (posY >= 540)
                offset = new Vector3(_guideArrow.rect.width / 2, -_guideArrow.rect.height / 2, 0);
            else
                offset = new Vector3(_guideArrow.rect.width / 2, _guideArrow.rect.height / 2, 0);
        }
        _guideArrow.transform.position = target.position + offset;
        Vector3 direction = target.position - _guideArrow.transform.position;
        _guideArrow.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction.normalized);
    }
    /// <summary>
    /// 结束UI遮罩引导
    /// </summary>
    private void EndUIGuide()
    {
        _curGuideType = EGuideType.None;
        _curUIGuide.gameObject.SetActive(false);//关闭形状遮罩
        _guideArrow.gameObject.SetActive(false);//关闭箭头
        if (_curGuideBtn) _curGuideBtn.onClick.RemoveListener(EndUIGuide);
        else Destroy(_curGuideObj.GetComponent<GuideEvents>());
        _curGuideObj = null;
        _curGuideBtn = null;
        EndGuide();
    }

    /// <summary>
    /// 事件引导
    /// </summary>
    /// <param name="info"></param>
    private void SetEventGuide(string objPath, EEventGuideType gEvent, string info)
    {
        ShowTip(info);
        _curGuideType = EGuideType.EventListen;
        if (objPath != null && objPath != "")
        {
            _curGuideObj = GameObject.Find(objPath);
            switch (gEvent)
            {
                case EEventGuideType.SingleClick:
                    _curGuideObj.AddComponent<GuideEvents>().SingleClick += EndEventGuide;
                    break;
                case EEventGuideType.DoubleClick:
                    _curGuideObj.AddComponent<GuideEvents>().DoubleClick += EndEventGuide;
                    break;
                case EEventGuideType.GREAT:

                    break;
            }
        }
    }
    /// <summary>
    /// 结束事件引导
    /// </summary>
    private void EndEventGuide()
    {
        Destroy(_curGuideObj.GetComponent<GuideEvents>());
        _curGuideType = EGuideType.None;
        _curGuideObj = null;
        EndGuide();
    }
    #endregion

    #region 提示信息
    /// <summary>
    /// 显示Tip
    /// </summary>
    /// <param name="info"></param>
    private void ShowTip(string info)
    {
        _guideText.GetComponentInChildren<Text>().text = info;
    }
    /// <summary>
    /// 隐藏Tip
    /// </summary>
    private void HideTip()
    {
        _guideText.GetComponentInChildren<Text>().text = "";
    }
    private IEnumerator DoScale(RectTransform trans, Vector3 targetScale, float time)
    {
        Vector3 target = targetScale - trans.localScale;
        while ((trans.localScale - targetScale).magnitude >= 0.01f)
        {
            trans.localScale += target * (Time.deltaTime / time);
            yield return null;
        }
        trans.localScale = targetScale;
    }
    #endregion
}