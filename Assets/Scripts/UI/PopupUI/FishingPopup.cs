using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class FishingPopup : PopupUI 
{
    public static bool IsFishing;
    
    [Header("UI 요소")]
    public GameObject uiFishingPanel; // 낚시 미니게임 전체 UI
    public RectTransform uiTimingBar; // 타이밍 바 UI
    public RectTransform uiTargetArea; // 목표 영역 UI
    public RectTransform uiTimerGauge; // 타이머 게이지 UI
    
    [SerializeField] private Image _startGame;
    [SerializeField] private Image _successGame;
    [SerializeField] private Image _failGame;
    
    [Header("게임 설정")]
    private float _gaugeMaxHeight; // 타이머 게이지의 최대 높이
    
    public float minSpeed; // 타이밍 바 최소 속도
    public float maxSpeed; // 타이밍 바 최대 속도
    private float _timeLimit = 5f; // 제한 시간

    private float _timingBarSpeed;
    private bool _timingBarMovingUp = true;
    private float _gameTimer;
    private bool _isGameStarted;
    
    private readonly int _minItemCode = 1000;
    private readonly int _maxItemCode = 1004;

    
    public override void Init()
    {
        _gaugeMaxHeight = uiTimerGauge.sizeDelta.y; // 초기화 시 타이머 게이지의 최대 높이 저장
    }

    
    private void OnEnable()
    {
        IsFishing = true;
        
        uiFishingPanel.SetActive(false);
        
        _startGame.gameObject.SetActive(false);
        _successGame.gameObject.SetActive(false);
        _failGame.gameObject.SetActive(false);
        
        StartCoroutine(StartFishing());
        
        Managers.Sound.PlaySfx(SfxEnums.FishingSwing);
    }


    private void Update()
    {
        if(_isGameStarted == false) return;
        
        MoveTimingBar();
        UpdateTimer();
    }


    private void OnDisable()
    {
        GameManager.Instance.CharacterM.AnimationController.SetStartFishing(false);
        GameManager.Instance.CharacterM.AnimationController.SetFishing(false);
        GameManager.Instance.CharacterM.AnimationController.SetFailFishing(false);
        GameManager.Instance.CharacterM.AnimationController.SetSuccessFishing(false);
        GameManager.Instance.CharacterM.AnimationController.SetEndFishing(false);

        GameManager.Instance.CharacterM.InputController.OnInteractPopupUI -= CheckSuccess;
        
        IsFishing = false;
    }


    private IEnumerator StartFishing()
    {
        _isGameStarted = false;
        GameManager.Instance.CharacterM.AnimationController.SetStartFishing(true);
        GameManager.Instance.CharacterM.AnimationController.SetFishing(true);

        float waitTime = Random.Range(3f, 10f); // 3~10초 랜덤 대기
        
        yield return new WaitForSeconds(waitTime - 1f); // 시작 느낌표 나오기 1초 전까지 기다림
        
        _startGame.gameObject.SetActive(true);
        Managers.Sound.PlaySfx(SfxEnums.FishingReel);
        yield return new WaitForSeconds(1f); // 시작 느낌표를 1초 동안 표시
        
        _startGame.gameObject.SetActive(false);

        
        StartFishingGame();
        _isGameStarted = true;
    }
    
    
    /// <summary>
    /// 미니 게임 시작
    /// </summary>
    private void StartFishingGame()
    {
        uiFishingPanel.gameObject.SetActive(true);
        GameManager.Instance.CharacterM.InputController.OnInteractPopupUI += CheckSuccess;
        SetupGame();
    }

#region 게임 초기화

    private void SetupGame()
    {
        SetRandomTargetArea();
        SetRandomSpeed();
        ResetTimingBar();
        ResetTimer();
    }
    
    
    /// <summary>
    /// 목표 영역 랜덤 설정
    /// </summary>
    private void SetRandomTargetArea()
    {
        float positionY = Random.Range(-50f, 50f);
        float height = Random.Range(20f, 60f);

        uiTargetArea.anchoredPosition = new Vector2(0, positionY);
        uiTargetArea.sizeDelta = new Vector2(uiTargetArea.sizeDelta.x, height);
    }

    
    /// <summary>
    /// 타이밍 바 랜덤 속도 지정
    /// </summary>
    private void SetRandomSpeed()
    {
        _timingBarSpeed = Random.Range(minSpeed, maxSpeed);
    }
    
    
    /// <summary>
    /// 타이밍 바 초기화 세팅
    /// </summary>
    private void ResetTimingBar()
    {
        uiTimingBar.anchoredPosition = new Vector2(0, -90f);
        _timingBarMovingUp = true;
    }

    
    /// <summary>
    /// 타이머 게이지를 꽉 찬 상태로 설정
    /// </summary>
    private void ResetTimer()
    {
        _gameTimer = _timeLimit;
        
        if (uiTimerGauge != null)
        {
            uiTimerGauge.sizeDelta = new Vector2(uiTimerGauge.sizeDelta.x, _gaugeMaxHeight);
        }
    }

#endregion


#region 게임 진행

    private void MoveTimingBar()
    {
        float moveAmount = _timingBarSpeed * Time.deltaTime;
        float newY = uiTimingBar.anchoredPosition.y + (_timingBarMovingUp ? moveAmount : -moveAmount);

        if (newY > 65f)
        {
            newY = 65f;
            _timingBarMovingUp = false;
        }
        else if(newY < -65f)
        {
            newY = -65f;
            _timingBarMovingUp = true;
        }

        uiTimingBar.anchoredPosition = new Vector2(0, newY);
    }
    
    
    private void UpdateTimer()
    {
        _gameTimer -= Time.deltaTime;

        // 게이지의 높이를 남은 시간에 비례하도록 설정
        if (uiTimerGauge != null)
        {
            float fillAmount = Mathf.Clamp01(_gameTimer / _timeLimit);
            uiTimerGauge.sizeDelta = new Vector2(uiTimerGauge.sizeDelta.x, fillAmount * _gaugeMaxHeight);
        }

        if (_gameTimer <= 0 && _isGameStarted)
        {
            EndGame(false);
        }
    }


    /// <summary>
    /// 타이밍 바가 목표 영역에 들어왔는지 확인
    /// </summary>
    private bool CheckSuccess()
    {
        float barY = uiTimingBar.anchoredPosition.y;
        float targetTop = uiTargetArea.anchoredPosition.y + (uiTargetArea.sizeDelta.y / 2);
        float targetBottom = uiTargetArea.anchoredPosition.y - (uiTargetArea.sizeDelta.y / 2);

        bool success = barY >= targetBottom && barY <= targetTop;
        return EndGame(success);
    }


    private bool EndGame(bool p_success)
    {
        _isGameStarted = false;
        
        GameManager.Instance.CharacterM.AnimationController.SetEndFishing(true);

        if (p_success)
        {
            _successGame.gameObject.SetActive(true);
            uiFishingPanel.gameObject.SetActive(false);

            GameManager.Instance.CharacterM.AnimationController.SetSuccessFishing(true);
            GameManager.Instance.CharacterM.AnimationController.SetStartFishing(false);

            StartCoroutine(OnFishingSuccess());
            return true;
        }

        _failGame.gameObject.SetActive(true);
        uiFishingPanel.gameObject.SetActive(false);

        GameManager.Instance.CharacterM.AnimationController.SetFailFishing(true);
        GameManager.Instance.CharacterM.AnimationController.SetStartFishing(false);

        StartCoroutine(OnFishingFailure());
        return false;
    }

#endregion

    
    /// <summary>
    /// 낚시 성공시 호출
    /// </summary>
    private IEnumerator OnFishingSuccess()
    {
        Managers.Sound.PlaySfx(SfxEnums.FishingCastImpact);
        
        yield return Setting.WaitForOneSecond;
        
        int number = Random.Range(_minItemCode, _maxItemCode);
        
        GameManager.Instance.CharacterM.inventory.AcquireItem(GameManager.Instance.DataBaseM.ItemDatabase.GetByID(number));
        
        Close();
    }


    /// <summary>
    /// 낚시 실패시 호출
    /// </summary>
    private IEnumerator OnFishingFailure()
    {        
        Managers.Sound.PlaySfx(SfxEnums.FishingCast);

        yield return Setting.WaitForOneSecond;
        
        Close();
    }
}