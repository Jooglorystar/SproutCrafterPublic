using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public bool IsPlayingTime = false; // 임시


    public bool IsSleep { get; private set; }

    [Header("시간용 필드")]
    private int _inGameTotalDay;
    private int _inGameHour;
    private int _inGameMinute;

    [Header("캐싱용 필드")]
    private int _currentGameYear;
    private ESeason _currentSeason;
    private EWeekday _currentWeekday;
    private int _currentGameDay;

    private int _lastCalculatedDay;
    private ESeason _lastCheckedSeason;
    private int _lastCheckedYear;

    [Header("플레이타임(초)")]
    private float _totalPlayTimeSeconds;

    [Header("인게임 시간 제어용")]
    // 인게임 하루 시간이 얼만큼 흐르는가?
    [SerializeField] private int _inGameMinuteUnit = 5;
    // 위 시간이 흐르기 위해서는 현실에서 얼마의 시간이 흘러야 하는가?
    [SerializeField] private float _realTimeSecondPerInGameMinuteUnit = 5f;

    private float _lastCheckedRealTime = 0f;

    public int InGameYear
    {
        get
        {
            UpdateDate();
            return _currentGameYear;
        }
    }
    public ESeason CurrentSeason
    {
        get
        {
            UpdateDate();
            return _currentSeason;
        }
    }
    public EWeekday CurrentWeekDay
    {
        get
        {
            UpdateDate();
            return _currentWeekday;
        }
    }
    public int InGameDay
    {
        get
        {
            UpdateDate();
            return _currentGameDay;
        }
    }
    public int InGameTotalDay { get { return _inGameTotalDay + 1; } }
    public int InGameHour { get { return _inGameHour; } }
    public int InGameMinute { get { return _inGameMinute; } }

    public float TotalPlayTimeSeconds { get { return _totalPlayTimeSeconds; } }

    public int InGameMunuteUnit { get { return _inGameMinuteUnit; } }
    public float RealTimeSecondPerInGameMinuteUnit { get { return _realTimeSecondPerInGameMinuteUnit; } }

    public event Action OnTimeCheck;
    public event Action OnDayCheck;
    public event Action OnSeasonCheck;
    public event Action OnYearCheck;

    private void Awake()
    {
        GameManager.Instance.TimeM = this;
    }

    private void Start()
    {
        OnTimeCheck += CalculateInGameTime;
    }

    private void Update()
    {
        if (IsPlayingTime)
        {
            CheckPlayTime();
            StartTime();
        }
    }

    /// <summary>
    /// 프로퍼티 갱신용
    /// </summary>
    private void UpdateDate()
    {
        if (_lastCalculatedDay != _inGameTotalDay)
        {
            _currentGameYear = (_inGameTotalDay / 112) + 1;
            _currentSeason = (ESeason)((_inGameTotalDay / 28) % 4);
            _currentWeekday = (EWeekday)(_inGameTotalDay % 7);
            _currentGameDay = (_inGameTotalDay % 28) + 1;

            _lastCalculatedDay = _inGameTotalDay;
        }
    }

    private void UpdateSeason()
    {
        _currentSeason = (ESeason)((_inGameTotalDay / 28) % 4);
    }

    private void UpdateYear()
    {
        _currentGameYear = (_inGameTotalDay / 112) + 1;
    }

    #region 델리게이트 호출용
    private void CallTimeCheck()
    {
        OnTimeCheck?.Invoke();
    }

    private void CallDayCheck()
    {
        OnDayCheck?.Invoke();
    }

    private void CallSeasonCheck()
    {
        OnSeasonCheck?.Invoke();
    }

    private void CallYearCheck()
    {
        OnYearCheck?.Invoke();
    }

    #endregion
    /// <summary>
    /// 인게임 시간을 초기화하는 메서드
    /// </summary>
    public void InitiateInGameTime()
    {
        // 플레이 시간 초기화
        _totalPlayTimeSeconds = 0f;

        // 인게임 시간 초기화
        _lastCheckedRealTime = 0f;
        _inGameTotalDay = 0;
        _lastCalculatedDay = _inGameTotalDay - 1;
        ResetInGameTime(6);
        CallDayCheck();
        CallSeasonCheck();
        CallYearCheck();

        IsPlayingTime = true;
    }

    public void LoadGameTime()
    {
        _lastCalculatedDay = _inGameTotalDay - 1;
        ResetInGameTime(6);
        CallDayCheck();
        CallSeasonCheck();
        CallYearCheck();

        IsPlayingTime = true;
    }

    public void InitTimeManager()
    {
        if (_inGameTotalDay > 0)
        {
            LoadGameTime();
        }
        else
        {
            InitiateInGameTime();
        }
    }

    /// <summary>
    /// 다음 날로 진행하는 메서드, 날짜, 계절 계산을 이 메서드에서 합니다.
    /// </summary>
    /// <param name="p_wakeUpTime">일어날 시각</param>
    public void WakeUpAtMorning(int p_wakeUpTime)
    {
        if (p_wakeUpTime < 6) return;

        _inGameTotalDay++;
        ResetInGameTime(p_wakeUpTime);
        CallDayCheck();
        CheckDelegate();
    }

    private void CheckDelegate()
    {
        if (_lastCalculatedDay != _inGameTotalDay)
        {
            CallDayCheck();
            _lastCalculatedDay = _inGameTotalDay;
        }
        if (_lastCheckedSeason != _currentSeason)
        {
            CallSeasonCheck();
            _lastCheckedSeason = _currentSeason;
        }

        if (_lastCheckedYear != _currentGameYear)
        {
            CallYearCheck();
            _lastCheckedYear = _currentGameYear;
        }
    }

    /// <summary>
    /// 일어나는 시각으로 초기화하는 메서드
    /// </summary>
    /// <param name="p_awakeUpTime">일어날 시간 입력</param>
    private void ResetInGameTime(int p_awakeUpTime)
    {
        _inGameHour = p_awakeUpTime;
        _inGameMinute = 0;

        _lastCheckedRealTime = 0f;
        CallTimeCheck();
    }

    #region 시간 진행관련 메서드
    /// <summary>
    /// 정해진 현실시간 마다 정해진 인게임 시간을 흐르게 합니다.
    /// </summary>
    private void StartTime()
    {
        _lastCheckedRealTime += Time.deltaTime;
        if (_lastCheckedRealTime >= _realTimeSecondPerInGameMinuteUnit)
        {
            _inGameMinute += _inGameMinuteUnit;

            CallTimeCheck();

            _lastCheckedRealTime = 0f;
        }
    }

    /// <summary>
    /// 시간을 계산합니다.
    /// </summary>
    private void CalculateInGameTime()
    {
        // 분->시 계산
        if (_inGameMinute >= 60)
        {
            _inGameHour += (_inGameMinute / 60);
            _inGameMinute %= 60;
        }

        // 00시 01시 02시 계산용
        if (_inGameHour >= 24)
        {
            _inGameHour %= 24;
        }

        // 2시에 시간을 멈추게 함
        if (_inGameHour == 2)
        {
            Sleep(8, Setting.SpawnPoint);
        }
    }

    public void Sleep(int p_wakeUpTime, Vector2 p_spawnPoint)
    {
        GameManager.Instance.StartFade(() => MorningRoutine(p_wakeUpTime, p_spawnPoint));
    }

    private IEnumerator MorningRoutine(int p_wakeUpTime, Vector2 p_spawnPoint)
    {
        IsSleep = true;

        if(GameManager.Instance.CharacterM.CurrentScene != SceneName.Farm)
            yield return GameManager.Instance.SceneControlM.MoveScene(SceneName.Farm, p_spawnPoint);
        else
            GameManager.Instance.CharacterM.transform.position = p_spawnPoint;

        GameManager.Instance.TimeM.WakeUpAtMorning(p_wakeUpTime);
        GameManager.Instance.CropM.UpdateGrowth();
        GameManager.Instance.TilemapM.ResetTileStateAtMorning();
        GameManager.Instance.DynamicPricingSystem.UpdatePrices();
        SaveManager.Instance.SaveGame();
        Managers.Sound.PlayRandomMusic();

        GameManager.Instance.CharacterM.playerCondition.Eat();

        IsSleep = false;
    }

    public string GetTimeToString()
    {
        return $"{_inGameHour.ToString("D2")}:{_inGameMinute.ToString("D2")}";
    }

    #endregion

    #region 현실 플레이타임 기록용
    /// <summary>
    /// 실제 플레이 타임을 기록하는 메서드
    /// </summary>
    private void CheckPlayTime()
    {
        _totalPlayTimeSeconds += Time.deltaTime;
    }

    /// <summary>
    /// 초로 이루어진 플레이타임을 시간과 분으로 HH:MM 형식의 문자열로 반환하는 메서드
    /// </summary>
    /// <param name="p_totalPlayTime"></param>
    /// <returns>HH:MM:SS</returns>
    private string CalculatePlayTime(float p_totalPlayTime)
    {
        int totalPlayMinute = (int)p_totalPlayTime / 60;

        return $"{(totalPlayMinute / 60).ToString("D2")}:{(totalPlayMinute % 60).ToString("D2")}:{((int)p_totalPlayTime % 60).ToString("D2")}";
    }
    #endregion

    #region 디버그용 출력용 메서드
    // 인게임 시간 디버그 출력용
    private void DebugGetInGameTime()
    {
        Debug.Log($"{InGameYear}년차 {CurrentSeason} {InGameDay}일차, {CurrentWeekDay} {_inGameHour.ToString("D2")}:{_inGameMinute.ToString("D2")}");
    }

    // 총 일수 디버그 출력용
    private void DebugGetTotalGameDate()
    {
        Debug.Log($"총 {InGameTotalDay}일");
    }

    // 플레이타임 디버그 출력용
    private void DebugGetPlayTime()
    {
        Debug.Log($"{CalculatePlayTime(_totalPlayTimeSeconds)} 동안 플레이");
    }
    #endregion
    #region 시간 저장 함수
    public void SetInGameDay(int day)
    {
        _inGameTotalDay = day;
    }
    #endregion
}