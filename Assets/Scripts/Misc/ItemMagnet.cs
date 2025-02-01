using UnityEngine;

public class ItemMagnet : MonoBehaviour
{
    public float magnetRange = 5f;  // 자석 범위 (플레이어와 아이템 사이의 거리)
    public float magnetSpeed = 5f;  // 자석 끌어당기는 속도

    private Transform player;  // 플레이어의 Transform
    private Inventory playerInventory;  // 플레이어의 Inventory

    private void Start()
    {
        // 플레이어 오브젝트 찾기
        player = GameObject.FindWithTag("Player").transform;
        playerInventory = player.GetComponent<Inventory>();  // 플레이어의 Inventory 스크립트 참조
    }

    private void Update()
    {
        MagnetizeItem();
    }

    private void MagnetizeItem()
    {
        // 플레이어와 아이템 간의 거리 계산
        float distance = Vector3.Distance(transform.position, player.position);

        // 만약 아이템이 자석 범위 안에 있으면, 플레이어로 끌어당기기
        if (distance <= magnetRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * magnetSpeed * Time.deltaTime;
        }
    }
}
