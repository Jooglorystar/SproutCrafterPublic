using UnityEngine;


public class ItemInfomation : MonoBehaviour
{
    public ItemSO item;
    public int itemAmount = 5;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    
    private void OnEnable()
    {
        _spriteRenderer.sprite = item.itemSprite;
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.CharacterM.inventory.AcquireItem(item, itemAmount);

            Destroy(gameObject); // 오브젝트 풀로 반환하도록 연구해보기
        }
    }
}