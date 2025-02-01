using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    private InputController _inputController;
    private AnimationController _animationController;
    
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    
    [Header("플레이어 이동")]
    [SerializeField] private float _moveSpeed;
    private Vector2 _preMovementInput;
    private Vector2 _currentMovementInput;


    public void Awake()
    {
        _animationController = GetComponentInChildren<AnimationController>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        _rigidbody = GetComponent<Rigidbody2D>();
    }


    private void Start()
    {
        _inputController = GetComponent<InputController>();

        SubscribeMoveEvent();
    }


    private void FixedUpdate()
    {
        ApplyMovement(_currentMovementInput);
    }


    /// <summary>
    /// InputController의 Action에서 실행됨
    /// </summary>
    private void SetMoveInput(Vector2 p_input)
    {
        _currentMovementInput = p_input;
    }


    /// <summary>
    /// 실제로 오브젝트를 움직이는 역할
    /// </summary>
    private void ApplyMovement(Vector2 p_input)
    {
        _rigidbody.velocity = p_input * _moveSpeed;

        _animationController.SetMove(_rigidbody.velocity.magnitude > 0.1f ? true : false);

        SetSpriteFlip(p_input);
    }


    /// <summary>
    /// X 인풋에 따라서 스프라이트를 뒤집어줌
    /// </summary>
    private void SetSpriteFlip(Vector2 p_input)
    {
        if (p_input.x != 0)
        {
            _spriteRenderer.flipX = p_input.x > 0;
        }
    }

    /// <summary>
    /// 입력한 마우스 위치에 따라 스프라이트를 뒤집음 도구 사용시 호출
    /// </summary>
    /// <param name="p_mousePos"></param>
    public void FlipSprite(Vector2 p_mousePos)
    {
        bool isRight = p_mousePos.x > transform.position.x;

        _spriteRenderer.flipX = isRight;
    }


    public void SubscribeMoveEvent()
    {
        _inputController.OnMoveAction = SetMoveInput;
    }


    public void UnSubscribeMoveEvent()
    {
        _inputController.OnMoveAction -= SetMoveInput;

        _currentMovementInput = Vector2.zero;
    }
}