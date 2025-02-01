using UnityEngine;


public class AnimationController : MonoBehaviour
{
    [Header("기본 동작")]
    private static readonly int _walk = Animator.StringToHash("Walk");
    private static readonly int _blackOutHash = Animator.StringToHash("BlackOut");


    [Header("도구 사용")]
    private static readonly int _hoe = Animator.StringToHash("Hoe");
    private static readonly int _watering = Animator.StringToHash("Watering");
    private static readonly int _axe = Animator.StringToHash("Axe");


    [Header("낚시")]
    private static readonly int _startFishing = Animator.StringToHash("StartFishing");
    private static readonly int _fishing = Animator.StringToHash("Fishing");
    private static readonly int _endFishing = Animator.StringToHash("EndFishing");
    private static readonly int _successFishing = Animator.StringToHash("SuccessFishing");
    private static readonly int _failFishing = Animator.StringToHash("FailFishing");


    private Animator _playerAnimator;
    private bool _isAnimating = false;


    private void Awake()
    {
        _playerAnimator = GetComponent<Animator>();
    }


    private void Start()
    {
        GameManager.Instance.CharacterM.AnimationController = this;
    }


    public void SetMove(bool p_Flag)
    {
        _playerAnimator.SetBool(_walk, p_Flag);
    }


    public void SetWatering()
    {
        if (_isAnimating == false)
        {
            _isAnimating = true;
            _playerAnimator.SetTrigger(_watering);
        }
    }


    public void SetAxe()
    {
        if (_isAnimating == false)
        {
            _isAnimating = true;
            _playerAnimator.SetTrigger(_axe);
        }
    }


    public void SetHoe()
    {
        if (_isAnimating == false)
        {
            _isAnimating = true;
            _playerAnimator.SetTrigger(_hoe);
        }
    }

    public void SetBlackOut(bool p_bool)
    {
        _playerAnimator.SetBool(_blackOutHash, p_bool);
    }


    #region 낚시

    public void SetStartFishing(bool p_Flag)
    {
        if (_isAnimating == false)
        {
            _isAnimating = true;
            _playerAnimator.SetBool(_startFishing, p_Flag);
        }
    }


    public void SetFishing(bool p_Flag)
    {
        _playerAnimator.SetBool(_fishing, p_Flag);
    }


    public void SetEndFishing(bool p_Flag)
    {
        _playerAnimator.SetBool(_endFishing, p_Flag);
    }


    public void SetSuccessFishing(bool p_Flag)
    {
        _playerAnimator.SetBool(_successFishing, p_Flag);

        _isAnimating = false;
    }


    public void SetFailFishing(bool p_Flag)
    {
        _playerAnimator.SetBool(_failFishing, p_Flag);

        _isAnimating = false;
    }

    #endregion


    #region 이벤트 제어

    public void StartMove()
    {
        GameManager.Instance.CharacterM.PlayerMovement.SubscribeMoveEvent();

        _isAnimating = false;
    }


    public void StopMove()
    {
        GameManager.Instance.CharacterM.PlayerMovement.UnSubscribeMoveEvent();
    }


    public bool IsAnimating()
    {
        return _isAnimating;
    }

    #endregion

}