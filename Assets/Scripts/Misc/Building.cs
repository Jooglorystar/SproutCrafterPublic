using UnityEngine;
using UnityEngine.U2D.Animation;

public class Building : MonoBehaviour
{
    public bool placed { get; private set; }
    public bool hited { get; private set; }
    public BoundsInt area;

    public BuildingSO buildingData;

    private BoxCollider2D boxCollider;

    private SpriteResolver[] spriteResolvers;
    private SpriteResolver backSpriteResolver;
    private SpriteResolver frontSpriteResolver;

    private Vector3 backSpritePosition;
    private Vector3 frontSpritePosition;

    private string _back = "Back";
    private string _front = "Front";

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteResolvers = GetComponentsInChildren<SpriteResolver>();

        backSpriteResolver = spriteResolvers[0];
        frontSpriteResolver = spriteResolvers[1];
    }

    /// <summary>
    /// SO에 맞춰 설치물의 정보를 업데이트 하는 메서드
    /// </summary>
    /// <param name="p_buildingData">설치물 SO</param>
    public void SetBuilingData(BuildingSO p_buildingData)
    {
        buildingData = p_buildingData;

        area.size = buildingData.buildingAreaSize;

        if (area.size.y <= 1)
        {
            // 플레이어보다 아래에 있는 적용
            backSpritePosition = new Vector3((float)area.size.x / 2f, ((float)area.size.y / 2f), 0);
            backSpriteResolver.gameObject.transform.position = backSpritePosition;

            // 플레이어보다 위에 있는 적용
            frontSpritePosition = new Vector3((float)area.size.x / 2f, ((float)area.size.y), 0);
            frontSpriteResolver.gameObject.transform.position = frontSpritePosition;

            boxCollider.offset = new Vector2((float)area.size.x / 2f, ((float)area.size.y / 2f));
            boxCollider.size = new Vector2(area.size.x, area.size.y);
        }
        else
        {
            // 플레이어보다 아래에 있는 적용
            backSpritePosition = new Vector3((float)area.size.x / 2f, ((float)area.size.y / 2f) + 1, 0);
            backSpriteResolver.gameObject.transform.position = backSpritePosition;

            // 플레이어보다 위에 있는 적용
            frontSpritePosition = new Vector3((float)area.size.x / 2f, ((float)area.size.y) + 0.5f, 0);
            frontSpriteResolver.gameObject.transform.position = frontSpritePosition;

            boxCollider.offset = new Vector2((float)area.size.x / 2f, ((float)area.size.y / 2f) + 0.5f);
            boxCollider.size = new Vector2(area.size.x, area.size.y - 1);
        }
        backSpriteResolver.SetCategoryAndLabel(buildingData.spriteCategory, _back);
        frontSpriteResolver.SetCategoryAndLabel(buildingData.spriteCategory, _front);

        boxCollider.isTrigger = true;
    }

    public void ResetPostion()
    {
        transform.position = Vector3.zero;
        area.position = Vector3Int.zero;
        backSpriteResolver.gameObject.transform.position = Vector3.zero;
        frontSpriteResolver.gameObject.transform.position = Vector3.zero;
    }

    public bool CanBePlaced()
    {
        Vector3Int positionInt = GameManager.Instance.BuildingM.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;

        if (GameManager.Instance.BuildingM.CanTakeArea(areaTemp))
        {
            return true;
        }

        return false;
    }

    public void Place()
    {
        Vector3Int positionInt = GameManager.Instance.BuildingM.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;
        placed = true;
        boxCollider.isTrigger = false;
        GameManager.Instance.BuildingM.TakeArea(areaTemp);
        buildingData.Place(this);
    }

    public void Remove()
    {
        placed = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hited = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hited = false;
    }
}