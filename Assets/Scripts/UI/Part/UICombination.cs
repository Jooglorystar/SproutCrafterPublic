using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UICombination : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    [SerializeField] private CraftingSystem _craftingSystem;
    [SerializeField] private Button _button;
    [SerializeField] private int _index;
    
    
    private void Start()
    {
        if (GameManager.Instance.DataBaseM.craftRecipesDatabase[_index].resultItem != null)
        {
            _button.image.sprite = GameManager.Instance.DataBaseM.craftRecipesDatabase[_index].resultItem.itemSprite;
        }
    }


    public void OnClickMakeItemButton()
    {
        _craftingSystem.CreateItem(GameManager.Instance.DataBaseM.craftRecipesDatabase[_index]);
    }
    

    /// <summary>
    /// 마우스를 올리면 조합에 필요한 아이템과 타겟 아이템의 상세 정보 표시
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }
    

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}