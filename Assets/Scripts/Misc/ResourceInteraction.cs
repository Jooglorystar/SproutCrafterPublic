using System.Collections;
using UnityEngine;

public class ResourceInteraction : MonoBehaviour
{
    public int health; // 자원의 체력
    public ResourceSO resourceSO; // 드롭 아이템 정보를 위한 ResourceSO
    private PoolManager poolManager; // PoolManager 참조
    private Vector3 fallDirection;

    //[Header("Effects")]
    //public GameObject dustParticlePrefab;

    private void Start()
    {
        // PoolManager 참조
        poolManager = GameManager.Instance.PoolM;
    }

    // ResourceSO를 설정하고 체력을 초기화하는 메서드
    public void Initialize(ResourceSO resource)
    {
        if (resource == null)
        {
            Debug.LogWarning("ResourceSO가 null입니다. 기본 ResourceSO를 사용합니다.");
            resourceSO = GameManager.Instance.DataBaseM.ItemDatabase.GetDefaultResourceSO();
        }
        else
        {
            resourceSO = resource;
        }

        // ResourceSO에서 체력을 설정
        health = resourceSO.health;

        Debug.Log($"ResourceInteraction 초기화: {resourceSO.itemName}, 체력: {health}");
    }

    // 자원을 타격하는 함수
    public void Hit(int damage)
    {
        if (health <= 0) return; // 이미 자원이 파괴되었으면 아무것도 하지 않음

        Managers.Sound.PlaySfx(SfxEnums.HitWood);

        health -= damage; // 타격을 받으면 자원 체력 감소

        if (health <= 0) // 체력이 0 이하로 떨어지면 자원 채집 완료
        {
            StartCoroutine(FallTree());
        }
    }

    private void OnResourceCollected()
    {
        // 자원 드롭 처리
        DropResource();
        Managers.Sound.PlaySfx(SfxEnums.ItemDrop);
        // 오브젝트를 풀로 반환
        ReturnToPool();
    }

    private void DropResource()
    {
        if (resourceSO != null && resourceSO.dropItemPrefab != null)
        {
            // 드롭 아이템 프리팹 생성
            GameObject dropItem = Instantiate(resourceSO.dropItemPrefab, transform.position, Quaternion.identity);

            // 드롭된 아이템의 정보를 초기화
            ItemInfomation itemInfo = dropItem.GetComponent<ItemInfomation>();
            if (itemInfo != null)
            {
                itemInfo.item = resourceSO; // 드롭된 아이템의 ResourceSO 설정
                itemInfo.itemAmount = resourceSO.dropAmount; // ResourceSO의 dropAmount를 사용
            }

            // Debug.Log($"드롭된 아이템: {resourceSO.itemName}, 수량: {resourceSO.dropAmount}");
        }
        else
        {
            // Debug.LogWarning("드롭할 아이템 정보가 없거나 드롭 프리팹이 설정되지 않았습니다.");
        }
    }


    private void ReturnToPool()
    {
        // 오브젝트를 풀로 반환
        poolManager.ReleaseObject(PoolTag.TreeGrowthPrefab, gameObject);
        // Debug.Log("오브젝트가 풀로 반환되었습니다.");
    }

    private void DetermineFallDirection()
    {
        // 플레이어 위치를 기준으로 쓰러질 방향 계산
        Vector3 playerPosition = GameManager.Instance.CharacterM.transform.position;
        fallDirection = (transform.position - playerPosition).normalized;

        // x축 방향으로만 쓰러질 방향 설정
        fallDirection = new Vector3(fallDirection.x, 0f, 0f).normalized; // z축 회전만 고려
    }

    private IEnumerator FallTree()
    {
        DetermineFallDirection(); // 쓰러질 방향 계산

        float fallDuration = 1.5f; // 쓰러지는 데 걸리는 시간
        float elapsed = 0f;

        // 초기 z축 회전
        float startRotationZ = transform.eulerAngles.z;
        // 목표 z축 회전
        float endRotationZ = startRotationZ + (fallDirection.x > 0 ? -90f : 90f); // x축 방향에 따라 회전 방향 결정

        //나무가 쓰러지기 시작할때 소리
        Managers.Sound.PlaySfx(SfxEnums.HitTreeFall);
        
        while (elapsed < fallDuration)
        {
            // z축 회전을 점진적으로 적용
            float currentRotationZ = Mathf.Lerp(startRotationZ, endRotationZ, elapsed / fallDuration);
            transform.rotation = Quaternion.Euler(0f, 0f, currentRotationZ);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 최종 z축 회전 적용
        transform.rotation = Quaternion.Euler(0f, 0f, endRotationZ);
        // 먼지 효과와 소리 재생
        PlayFallEffects();
        // 쓰러진 후 자원 드롭 및 풀 반환 처리
        OnResourceCollected();
    }

    private IEnumerator FallWithPhysics()
    {
        DetermineFallDirection();

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false; // 물리 효과 적용
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY; // z축 회전만 허용

        // 쓰러지는 방향으로 힘 가하기
        rb.AddTorque(Vector3.forward * (fallDirection.x > 0 ? -10f : 10f), ForceMode.Impulse);

        yield return new WaitForSeconds(1.5f); // 1.5초 후 나무가 정지

        rb.isKinematic = true; // 물리 효과 비활성화
        OnResourceCollected(); // 자원 드롭 및 풀 반환
    }
    private void PlayFallEffects()
    {/*
        if (dustParticlePrefab != null)
        {
            // 먼지 파티클 Prefab 생성
            GameObject dustParticle = Instantiate(dustParticlePrefab, transform.position, Quaternion.identity);

            // 파티클 시스템 자동 파괴
            Destroy(dustParticle, 2f); // 2초 후 파티클 제거
        }
        else
        {
            Debug.LogWarning("DustParticlePrefab이 연결되지 않았습니다.");
        }
        */
        // 나무 쓰러지는 소리 재생
        Managers.Sound.PlaySfx(SfxEnums.HitTreeFall);
    }
}