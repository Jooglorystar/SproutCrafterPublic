using UnityEngine;
using UnityEngine.UI;


public class DragSlot : ConvertToSingleton<DragSlot>
{
    [Header("이동시키고자하는 아이템의 정보")]
    [HideInInspector] public UISlot uiSlot;
    [HideInInspector] public Slot draggedSlot;
    [HideInInspector] public SlotType draggedSlotType;
    [HideInInspector] public int draggedSlotIndex;
    
    [Header("DragSlot의 요소")]
    [SerializeField] private Image _itemImage;


    protected override void Awake()
    {
        base.Awake();
        
        DragSlotSetColor(0);
    }


    /// <summary>
    /// 드래그가 시작될때 이미지를 설정하여 시각적으로 이동중임을 확인할 수 있도록 하는 기능
    /// </summary>
    /// <param name="p_itemImage"></param>
    public void DragSlotSetImage(Sprite p_itemImage)
    {
        this._itemImage.sprite = p_itemImage;
        DragSlotSetColor(1);
    }
    
    
    /// <summary>
    /// 드래그가 끝났을때 초기화하는 기능
    /// </summary>
    public void ClearDragSlot()
    {
        DragSlotSetColor(0);
        draggedSlot.itemSo = null;
        draggedSlot.itemCount = 0;
        draggedSlotIndex = 0;
    }


    /// <summary>
    /// 아이템 여부에 따라 이미지 조정
    /// </summary>
    private void DragSlotSetColor(float p_Alpha)
    {
        Color color = _itemImage.color;
        color.a = p_Alpha;
        _itemImage.color = color;
    }
}