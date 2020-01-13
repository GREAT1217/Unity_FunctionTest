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
    public RectTransform _bg;
    public Text _textDuration;
    public Text _textTime;
    public Button _btnZoom;
    public Sprite _spMax;
    public Sprite _spMin;
    public float _zoomTime = 0.2f;
    //自动隐藏栏
    public bool _autoHideBar;
    public float _autoHideInterval = 5f;//自动隐藏间隔
    public float _fadeTime = 0.2f;

    private UnityAction VideoEnd;//播放结束事件
    private Coroutine _sliderProcessCor;//更新进度条协程
    private float _defultVolume = 0.5f;//默认音量
    private float _curVolume;//静音前记录音量
    private int _videoDuration;//总时长
    private int _videoTime;//播放时间
    private bool _changingProcess;//调整进度中
    private bool _isPause;//暂停开关
    private bool _isMute;//静音开关
    private bool _isMax;//放大开关
    private Vector2 _maxSize;//放大尺寸
    private Vector2 _minSize;//缩小尺寸
    private Vector3 _maxPos;//放大位置
    private Vector3 _minPos;//放大位置
    private float _pointOnTime;//鼠标进入时间
    private bool _isHideBar;//栏是否隐藏
    private bool _playEnd;//播放结束

    public void ShowVideo(string videoPath, string title, UnityAction videoEnd)
    {
        gameObject.SetActive(true);
        VideoEnd = videoEnd;
        _videoPlayer.url = videoPath;
        _textTitle.text = title;

        ChangeVolume(_defultVolume);
        PlayVideo();

        //StartCoroutine(InitVideoDuration());

        _videoDuration = (int)Mathf.Floor(_videoPlayer.frameCount / _videoPlayer.frameRate);
        _textDuration.text = string.Format("{0:D2}:{1:D2}", _videoDuration / 60, _videoDuration % 60);
    }

    void Start()
    {
        _minPos = _bg.localPosition;
        _maxPos = Vector3.zero;
        _minSize = _bg.sizeDelta;
        _maxSize = new Vector2(Screen.width, Screen.height);
        if (_btnClose) _btnClose.onClick.AddListener(CloseVideo);
        _btnVideo.onClick.AddListener(PauseOrPlay);
        _btnAudio.onClick.AddListener(MuteOrNot);
        if (_btnZoom) _btnZoom.onClick.AddListener(MaxOrMinUI);
        UIEventTrigger.Add(_videoGraphic).PointerClick += PauseOrPlay;
        UIEventTrigger.Add(_videoGraphic).PointerClick += ShowBar;
        UIEventTrigger.Add(_videoGraphic).PointerEnter = ShowBar;
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
        UpdateVideoProcess();
        HideBar();
        CheckToFinishVideo();
    }

    /// <summary>
    /// 初始化总时长
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitVideoDuration()
    {
        //unity2108加载视频速度比unity5慢
        yield return new WaitUntil(() => _videoPlayer.frameRate != 0);
        _videoDuration = (int)Mathf.Floor(_videoPlayer.frameCount / _videoPlayer.frameRate);
        _textDuration.text = string.Format("{0:D2}:{1:D2}", _videoDuration / 60, _videoDuration % 60);
    }

    /// <summary>
    /// 检测视频结束
    /// </summary>
    private void CheckToFinishVideo()
    {
        if (_playEnd) return;
        if (_videoPlayer.frame >= (long)_videoPlayer.frameCount && _videoPlayer.isPlaying)
        {
            Debug.Log("=====================视频播放结束============");
            //PauseVideo();
            _sliderProcess.value = 1;//防止结束时遇到进度条刷新间隔而停止刷新
            if (VideoEnd != null)
            {
                VideoEnd();
            }
            _playEnd = true;
        }
        if (_sliderProcess.value == 1)
        {
            Debug.Log("=====================视频播放结束============");
            //PauseVideo();
            if (VideoEnd != null)
            {
                VideoEnd();
            }
            _playEnd = true;
        }
    }

    /// <summary>
    /// 关闭视频
    /// </summary>
    private void CloseVideo()
    {
        if (VideoEnd != null)
        {
            VideoEnd();
        }
        ClearVideoData();
    }

    /// <summary>
    /// 清理数据
    /// </summary>
    private void ClearVideoData()
    {
        VideoEnd = null;
        gameObject.SetActive(false);
        if (_isMax)
        {
            MaxOrMinUI();
        }
    }

    /// <summary>
    /// 更新视频进度
    /// </summary>
    private void UpdateVideoProcess()
    {
        if (_changingProcess)
        {
            ChangeProcess(_sliderProcess.value);
        }
        else
        {
            _videoTime = (int)_videoPlayer.time;
        }
        _textTime.text = string.Format("{0:D2}:{1:D2}", _videoTime / 60, _videoTime % 60);
    }

    /// <summary>
    /// 更新进度条
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateSliderProcess()
    {
        yield return new WaitUntil(() => _videoDuration != 0);
        while (true)
        {
            //减少刷新频率，防止点击抬起时sliderProcess卡顿
            _sliderProcess.value = (float)_videoTime / _videoDuration;
            yield return new WaitForSeconds(0.2f);
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
        _videoPlayer.time = _videoTime = (int)(_videoDuration * value);
    }

    /// <summary>
    /// 暂停视频
    /// </summary>
    private void PauseVideo()
    {
        //if (_videoPlayer.isPlaying)
        {
            _videoPlayer.Pause();
            _pauseState.SetActive(true);
            _btnVideo.image.sprite = _spPlay;
            if (_sliderProcessCor != null)
            {
                StopCoroutine(_sliderProcessCor);
                _sliderProcessCor = null;
            }
            _isPause = true;
        }
    }

    /// <summary>
    /// 播放视频
    /// </summary>
    private void PlayVideo()
    {
        Debug.Log("播放：pause状态" + _isPause);
        // if (!_videoPlayer.isPlaying)
        {
            if (_sliderProcess.value == 1)//重播
            {
                Debug.Log("重播");
                ChangeProcess(0);
            }
            _videoPlayer.Play();
            _pauseState.SetActive(false);
            _btnVideo.image.sprite = _spPause;
            _sliderProcessCor = StartCoroutine(UpdateSliderProcess());
            _isPause = false;
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
    /// 更改音量
    /// </summary>
    /// <param name="value"></param>
    private void ChangeVolume(float value)
    {
        _sliderVolume.value = value;
        _audioPlayer.volume = value;
        _btnAudio.image.sprite = value == 0 ? _spMute : _spUnMute;
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
    /// 放大或缩小UI
    /// </summary>
    private void MaxOrMinUI()
    {
        _isMax = !_isMax;
        if (_isMax)
        {
            _bg.DOLocalMove(_maxPos, _zoomTime);
            _bg.DOSizeDelta(_maxSize, _zoomTime);
            _btnZoom.image.sprite = _spMin;
        }
        else
        {
            _bg.DOLocalMove(_minPos, _zoomTime);
            _bg.DOSizeDelta(_minSize, _zoomTime);
            _btnZoom.image.sprite = _spMax;
        }
    }

    /// <summary>
    /// 检测隐藏栏
    /// </summary>
    private void HideBar()
    {
        if (!_autoHideBar) return;
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
    private void ShowBar()
    {
        if (!_autoHideBar) return;
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