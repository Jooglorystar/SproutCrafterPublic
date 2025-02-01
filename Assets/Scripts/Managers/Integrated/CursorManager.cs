using UnityEngine;
using UnityEngine.InputSystem;


public enum CursorTypes
{
    Default,
    Interaction,
    FishTool,
    HoeTool,
    AxeTool,
    WateringTool,
    CanPlant,
    CanHarvest,
}


public class CursorManager : MonoBehaviour, IInit
{
    private Camera _camera;
    
    private CursorTexturesSO _cursorTextures;
    
    private float _cursorUpdateTime = 0.2f;
    private float _currentTime = 0f;
    
    private bool _isInteracting = false;
    private bool _isTitleScene = true;
    
    public bool IsInteracting { get => _isInteracting; set => _isInteracting = value; } // IInteractable을 상속받는 것과 상호작용 중일 때 true
    public bool IsTitleScene { get => _isTitleScene; set => _isTitleScene = value; }
    
    
    public void Init()
    {
        _cursorTextures = Resources.Load<CursorTexturesSO>("Cursors/CursorTextures");
        
        EventManager.Subscribe(GameEventType.GameStart,SetCameraAfterSceneChange);
        
        Cursor.SetCursor(_cursorTextures.Cursors[(int)CursorTypes.Default].cursorTexture, _cursorTextures.Cursors[(int)CursorTypes.Default].hotspot, CursorMode.Auto);
    }


    private void Update()
    {
        _currentTime += Time.deltaTime;
        
        if (_isInteracting == false && _currentTime >= _cursorUpdateTime && _isTitleScene == false)
        {
            _currentTime = 0f;
            
            CheckCursorType(Managers.Data.inventorySlots[GameManager.Instance.CharacterM.InputController.FocusedSlotNumber]);
        }
    }

    
    /// <summary>
    /// 마우스 커서 위치의 오브젝트에 따라 바꿔주어야할 커서를 분기
    /// </summary>
    private void CheckCursorType(Slot p_Slot)
    {
        Vector2 mousePos = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        
        if (GameManager.Instance.CharacterM.PlayerInteraction.ObjectInteraction.CheckCanCrop(mousePos))
        {
            SetCursorTexture(CursorTypes.CanHarvest);
            return;
        }
        
        if (p_Slot.itemCount != 0)
        {
            if (p_Slot.itemSo.CanChangeCursor(mousePos))
            {
                SetCursorTexture(p_Slot.itemSo.cursorType);
                return;
            }

            SetCursorTexture(CursorTypes.Default);
        }
        
        SetCursorTexture(CursorTypes.Default);
    }

    
    /// <summary>
    /// 실제 커서를 바꿔주는 기능
    /// </summary>
    public void SetCursorTexture(CursorTypes p_CursorType)
    {
        Cursor.SetCursor(_cursorTextures.Cursors[(int)p_CursorType].cursorTexture, _cursorTextures.Cursors[(int)p_CursorType].hotspot, CursorMode.Auto);
    }


    /// <summary>
    /// 씬이 전환된 후 설정해주어야 할 것들 설정
    /// </summary>
    private void SetCameraAfterSceneChange(object p_args)
    {
        _camera = Camera.main;

        _isTitleScene = false;
    }
}