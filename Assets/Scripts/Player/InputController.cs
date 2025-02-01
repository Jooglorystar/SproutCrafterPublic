using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputController : MonoBehaviour
{
    public Action<Vector2> OnMoveAction;
    
    public Action<int> OnSelectSlotAction;
    
    public Action<Vector2, int> OnUseItemAction;
    public Action<Vector2, int> OnInteractAction;

    public Func<bool> OnInteractPopupUI;
    
    private Camera _camera;
    private int _focusedSlotNumber = 0;
    
    public int FocusedSlotNumber { get { return _focusedSlotNumber; } }


    private void Awake()
    {
        _camera = Camera.main;
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            OnMoveAction?.Invoke(context.ReadValue<Vector2>());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            OnMoveAction?.Invoke(Vector2.zero);
        }
    }


    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if(OnInteractPopupUI?.Invoke() == true) return;
            
            EventManager.Dispatch(GameEventType.OnInventory, null);
        }
    }


    public void OnSelectSpecificSlot(InputAction.CallbackContext context)
    {
        if (InGame.isUIOpened || Managers.UI.isUIPopupOpen) return;

        if (context.phase == InputActionPhase.Started)
        {
            if (int.TryParse(context.control.name, out int keyNumber))
            {
                _focusedSlotNumber = keyNumber == 0 ? 9 : keyNumber - 1;
            
                OnSelectSlotAction?.Invoke(_focusedSlotNumber);
            }
        }
    }
    
    
    public void OnScrollMouse(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !InGame.isUIOpened && !Managers.UI.isUIPopupOpen)
        {
            float scrollValue = context.ReadValue<Vector2>().y;

            _focusedSlotNumber = GetScrolledSlotNumber(scrollValue);
            
            OnSelectSlotAction?.Invoke(_focusedSlotNumber);
        }
    }


    public void OnUseItem(InputAction.CallbackContext context) // 도구 사용, 즉발형 아이템 사용
    {
        if (InGame.isUIOpened) return;

        if (context.phase == InputActionPhase.Started)
        {
            Vector2 mousePos = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            OnUseItemAction?.Invoke(mousePos, _focusedSlotNumber);
        }
    }


    public void OnInteract(InputAction.CallbackContext context) // IInteraction을 상속받는 것, 상호작용이 필요한 아이템 사용
    {
        if (InGame.isUIOpened) return;
            
        if (context.phase == InputActionPhase.Performed)
        {
            Vector2 mousePos = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            OnInteractAction?.Invoke(mousePos, _focusedSlotNumber);
        }
    }


    public void OnInteraction_F(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnInteractPopupUI?.Invoke();
        }
    }


    public void OnInteraction_Space(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnInteractPopupUI?.Invoke();
        }
    }
    
    
    private int GetScrolledSlotNumber(float scrollValue)
    {
        if (scrollValue > 0)
        {
            return (_focusedSlotNumber - 1 + Setting.SlotCount) % Setting.SlotCount;
        }

        return (_focusedSlotNumber + 1) % Setting.SlotCount;
    }
}