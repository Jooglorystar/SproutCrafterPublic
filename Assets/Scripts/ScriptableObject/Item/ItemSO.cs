using UnityEngine;


[CreateAssetMenu(menuName = "Item/BaseItem", fileName = "ItemBase_")]
public abstract class ItemSO : ScriptableObject, IDatabaseEntry
{
    [Header("필수 정보")]
    public int itemCode;                  // 아이템 고유 코드
    public string itemName;               // 아이템 이름
    public string itemDescription;        // 아이템 설명
    public Sprite itemSprite;             // 아이템 스프라이트
    public int buyPrice;                  // 구매 가격
    public int sellPrice;                 // 판매 가격

    [Space(10)]
    [Header("가변 정보")]
    public bool isCanStack;               // 스택 가능 여부
    public int maxStack;                  // 최대 스택 가능 수
    public GameObject dropItemPrefab;     // 드롭 아이템 프리팹
    public ItemType itemType;             // 아이템 종류 Enum
    public CursorTypes cursorType = CursorTypes.Default;

    public int Key => itemCode;

    public abstract bool CanChangeCursor(Vector2 p_MousePos);
    public abstract bool CanUse(Vector2 p_MousePos);
    public abstract void Use(Vector2 p_MousePos, int p_SlotNumber);

#if UNITY_EDITOR
    private void OnValidate()
    {
        ValidateHelper.ValidateCheckItemCode(this, nameof(itemCode), itemCode);
    }
#endif
}