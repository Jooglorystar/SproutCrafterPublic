using UnityEngine;


public class PlayerInteraction : MonoBehaviour // 스테미너 사용 로직 추가
{
    private InputController _inputController;
    private ItemInteraction _itemInteraction;
    private ObjectInteraction _objectInteraction;
    
    
    public ItemInteraction ItemInteraction {get {return _itemInteraction;}}
    public ObjectInteraction ObjectInteraction {get {return _objectInteraction;}}

    private Slot[] _slots => Managers.Data.inventorySlots;
    

    private void Awake()
    {
        _inputController = GetComponent<InputController>();
        _itemInteraction = GetComponent<ItemInteraction>();
        _objectInteraction = GetComponent<ObjectInteraction>();
    }


    private void OnEnable()
    {
        _inputController.OnUseItemAction += OnUseItem;
        _inputController.OnInteractAction += OnInteract;
    }


    /// <summary>
    /// 좌클릭을 이용한 동작 수행
    /// </summary>
    private void OnUseItem(Vector2 p_MousePos, int p_SlotNumber)
    {
        if(_slots[p_SlotNumber].itemSo == null) return;
        
        if (_slots[p_SlotNumber].itemSo.CanUse(p_MousePos))
        {
            GameManager.Instance.CharacterM.playerMovement.FlipSprite(p_MousePos);
            _slots[p_SlotNumber].itemSo.Use(p_MousePos, p_SlotNumber);
        }
    }


    /// <summary>
    /// 우클릭을 이용한 동작 수행
    /// </summary>
    private void OnInteract(Vector2 p_MousePos, int p_SlotNumber)
    {
        if (_objectInteraction.OnInteraction(p_MousePos) == false)
        {
            _objectInteraction.OnHarvest(p_MousePos);
        }
    }
}