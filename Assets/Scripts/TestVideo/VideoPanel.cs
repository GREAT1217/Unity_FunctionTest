using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

public class VideoPanel : MonoBehaviour
{
    /*
     * 视频：点击暂停、继续
     * 标题栏：自动隐藏、标题、关闭
     * 功能栏：自动隐藏、播放暂停按钮、点击和拖拽调整进度、静音按钮、点击和拖拽调整音量、放大缩小按钮
         */
    //标题栏
    public CanvasGroup _titleBar;
    public Text _textTitle;
    public Button _btnClose;
    //视频
    public RawImage _videoGraphic;
    public VideoPlayer _videoPlayer;
    public AudioSource _audioPlayer;
    public GameObject _pauseState;
    //功能栏
    public CanvasGroup _functionBar;
    //视频操作
    public Button _btnVideo;
    public Sprite _spPause;
    public Sprite _spPlay;
    public Slider _sliderProcess;
    //音量操作
    public Button _btnAudio;
    public Sprite _spMute;
    public Sprite _spUnMute;
    public Slider _sliderVolume;
    //UI操作
    public Text _textDuration;
    public Text _textTime;
    public Button _btnZoom;
    public Sprite _spMax;
    public Sprite _spMin;
    public float _zoomTime = 0.2f;
    //自动隐藏栏
    public float _autoHideInterval = 5f;//自动隐藏间隔
    public float _fadeTime = 0.2f;
    //UI引用
    private RectTransform _videoTransform;
    private Image _videoState;
    private Image _audioState;
    private Image _zoomState;

    private UnityAction VideoEnd;//播放结束事件
    private Coroutine _sliderProcessCor;//更新进度条协程
    private float _defultVolume = 0.5f;//默认音量
    private float _curVolume;//静音前记录音量
    private float _videoDuration;//总时长
    private float _videoTime;//播放时间
    private bool _changingProcess;//调整进度中
    private bool _isPause;//暂停开关
    private bool _isMute;//静音开关
    private bool _isMax;//放大开关
    private Vector2 _maxSize;//放大尺寸
    private Vector2 _minSize;//缩小尺寸
    private float _pointOnTime;//鼠标进入时间
    private bool _isHideBar;//栏是否隐藏

    public void ShowVideo(string videoPath, string title, UnityAction videoEnd)
    {
        gameObject.SetActive(true);
        VideoEnd = videoEnd;
        _videoPlayer.url = videoPath;
        _textTitle.text = title;

        ChangeVolume(_defultVolume);
        PlayVideo();

        _videoDuration = _videoPlayer.frameCount / _videoPlayer.frameRate;
        _textDuration.text = string.Format(" / {0}:{1}", (_videoDuration / 60).ToString("F0").PadLeft(2, '0'), (_videoDuration % 60).ToString("F0").PadLeft(2, '0'));
    }

    void Start()
    {
        _videoState = _btnVideo.GetComponent<Image>();
        _audioState = _btnAudio.GetComponent<Image>();
        _zoomState = _btnZoom.GetComponent<Image>();
        _videoTransform = _videoGraphic.GetComponent<RectTransform>();
        _minSize = _videoTransform.sizeDelta;
        _maxSize = new Vector2(Screen.width, Screen.height);
        _btnClose.onClick.AddListener(CloseVideo);
        _btnVideo.onClick.AddListener(PauseOrPlay);
        _btnAudio.onClick.AddListener(MuteOrNot);
        _btnZoom.onClick.AddListener(MaxOrMin);
        UIEventTrigger.Add(_videoGraphic).PointerClick += PauseOrPlay;
        UIEventTrigger.Add(_videoGraphic).PointerClick += ToShowBar;
        UIEventTrigger.Add(_videoGraphic).PointerEnter = ToShowBar;
        UIEventTrigger.Add(_sliderProcess).PointerDown = () => { ChangingProcess(true); };
        UIEventTrigger.Add(_sliderProcess).PointerUp = () => { ChangingProcess(false); };
        UIEventTrigger.Add(_sliderProcess).Drag = () => { ChangeProcess(_sliderProcess.value); };
        UIEventTrigger.Add(_sliderVolume).Drag = () => { ChangeVolume(_sliderVolume.value); };

        ShowVideo(Application.streamingAssetsPath + "/Videos/changan.mp4", "长安", null);
    }

    void FixedUpdate()
    {
        if (_videoPlayer.texture != null)
        {
            _videoGraphic.texture = _videoPlayer.texture;
        }
        UpdateProcess();
        CheckToHideBar();
        CheckToFinishVideo();
    }

    /// <summary>
    /// 检测视频结束
    /// </summary>
    private void CheckToFinishVideo()
    {
        if (_videoPlayer.frame >= (long)_videoPlayer.frameCount && _videoPlayer.isPlaying)
        {
            PauseVideo();
            if (VideoEnd != null)
            {
                VideoEnd();
            }
        }
    }

    /// <summary>
    /// 关闭视频
    /// </summary>
    private void CloseVideo()
    {
        if (VideoEnd != null) VideoEnd();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 更新进度
    /// </summary>
    private void UpdateProcess()
    {
        if (_changingProcess)
        {
            ChangeProcess(_sliderProcess.value);
        }
        else
        {
            _videoTime = (float)_videoPlayer.time;
        }
        _textTime.text = string.Format("{0}:{1}", (_videoTime / 60).ToString("F0").PadLeft(2, '0'), (_videoTime % 60).ToString("F0").PadLeft(2, '0'));
    }

    /// <summary>
    /// 更新进度条
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateSliderProcess()
    {
        while (true)
        {
            //减少刷新频率，防止点击抬起时sliderProcess卡顿
            yield return new WaitForSeconds(0.2f);
            _sliderProcess.value = _videoTime / _videoDuration;
        }
    }

    /// <summary>
    /// 更改进度中
    /// </summary>
    /// <param name="value"></param>
    private void ChangingProcess(bool value)
    {
        if (value)
        {
            //PauseVideo();
        }
        else
        {
            ChangeProcess(_sliderProcess.value);
            //PlayVideo();
        }
        _changingProcess = value;
    }

    /// <summary>
    /// 更改进度
    /// </summary>
    /// <param name="value"></param>
    private void ChangeProcess(float value)
    {
        if (_videoPlayer.isPrepared)
        {
            _videoPlayer.time = _videoTime = _videoDuration * value;
        }
    }

    /// <summary>
    /// 暂停视频
    /// </summary>
    private void PauseVideo()
    {
        if (_videoPlayer.isPlaying)
        {
            _videoPlayer.Pause();
            _pauseState.SetActive(true);
            _videoState.sprite = _spPlay;
            if (_sliderProcessCor != null)
            {
                StopCoroutine(_sliderProcessCor);
            }
        }
    }

    /// <summary>
    /// 播放视频
    /// </summary>
    private void PlayVideo()
    {
        if (!_videoPlayer.isPlaying)
        {
            _videoPlayer.Play();
            _pauseState.SetActive(false);
            _videoState.sprite = _spPause;
            _sliderProcessCor = StartCoroutine(UpdateSliderProcess());
        }
    }

    /// <summary>
    /// 暂停或继续
    /// </summary>
    private void PauseOrPlay()
    {
        _isPause = !_isPause;
        if (_isPause)
        {
            PauseVideo();
        }
        else
        {
            PlayVideo();
        }
    }

    /// <summary>
    /// 是否静音
    /// </summary>
    private void MuteOrNot()
    {
        _isMute = !_isMute;
        if (_isMute)
        {
            _curVolume = _audioPlayer.volume;
            ChangeVolume(0);
        }
        else
        {
            if (_curVolume == 0) _curVolume = _defultVolume;
            ChangeVolume(_curVolume);
        }
    }

    /// <summary>
    /// 更改音量
    /// </summary>
    /// <param name="value"></param>
    private void ChangeVolume(float value)
    {
        _sliderVolume.value = value;
        _audioPlayer.volume = value;
        _audioState.sprite = value == 0 ? _spMute : _spUnMute;
    }

    /// <summary>
    /// 放大或缩小UI
    /// </summary>
    private void MaxOrMin()
    {
        _isMax = !_isMax;
        if (_isMax)
        {
            _videoTransform.DOSizeDelta(_maxSize, _zoomTime);
            _zoomState.sprite = _spMin;
        }
        else
        {
            _videoTransform.DOSizeDelta(_minSize, _zoomTime);
            _zoomState.sprite = _spMax;
        }
    }

    /// <summary>
    /// 检测隐藏栏
    /// </summary>
    private void CheckToHideBar()
    {
        if (_isHideBar) return;
        _pointOnTime += Time.deltaTime;
        if (_pointOnTime > _autoHideInterval)
        {
            ShowOrHideBar(false);
            _isHideBar = true;
        }
    }

    /// <summary>
    /// 显示栏
    /// </summary>
    private void ToShowBar()
    {
        ShowOrHideBar(true);
        _pointOnTime = 0;
        _isHideBar = false;
    }

    /// <summary>
    /// 显示或隐藏栏
    /// </summary>
    /// <param name="value"></param>
    private void ShowOrHideBar(bool value)
    {
        if (value)
        {
            _titleBar.DOFade(1, _fadeTime);
            _functionBar.DOFade(1, _fadeTime);
        }
        else
        {
            _titleBar.DOFade(0, _fadeTime);
            _functionBar.DOFade(0, _fadeTime);
        }
    }

}
