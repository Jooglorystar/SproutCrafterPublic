using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LabUI : MonoBehaviour
{
    [Header("슬롯")]
    public UISlot slotA;
    public UISlot slotB;
    public Button breedButton;

    [Header("연구 슬롯")]
    public Image researchSilhouette;
    public Image resultImage;
    public Slider timerBar;
    public Button collectButton;

    [Header("디버그")]
    public TextMeshProUGUI debugLogText;

    private LabSystem labSystem;
    private bool researchComplete;
    private ItemSO researchResult;

    private void Start()
    {
        labSystem = FindObjectOfType<LabSystem>();
        Init();
    }

    #region Initialization (초기화)

    /// UI 초기화: 버튼 클릭 이벤트 및 슬롯 변경 이벤트 설정
    public void Init()
    {
        ResetResearchSlot();

        // 슬롯 초기화
        slotA.TurnOffFocusImage();
        slotB.TurnOffFocusImage();

        // 버튼 이벤트 등록
        breedButton.onClick.AddListener(StartBreeding);
        collectButton.onClick.AddListener(CollectResult);

        // 디버그 초기화
        UpdateDebugLog("조합 준비 완료. 슬롯에 작물을 넣어주세요.");
    }
    #endregion

    #region Breeding Process (조합 프로세스)

    /// 조합 시작 버튼 클릭 시 호출
    private void StartBreeding()
    {
        GameManager.Instance.DataBaseM.ItemDatabase.Init();

        // 슬롯이 비어있는지 확인
        if (slotA == null || slotB == null)
        {
            UpdateDebugLog("슬롯이 비어있습니다. 작물을 추가해주세요.");
            return;
        }

        // LabSystem을 통해 조합 실행
        researchResult = labSystem.CombineCrops();

        if (researchResult == null)
        {
            UpdateDebugLog("조합 결과가 없습니다. 조합 가능한 작물을 추가하세요.");
            return;
        }

        // 연구 진행 상태 업데이트
        researchComplete = false;
        researchSilhouette.gameObject.SetActive(true);
        resultImage.gameObject.SetActive(false);

       

        // 타이머 시작
        StartCoroutine(ResearchTimer(5f));
    }

    /// 연구 타이머 실행
    private IEnumerator ResearchTimer(float duration)
    {
        float elapsed = 0;

        // 타이머 진행
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            timerBar.value = elapsed / duration;
            yield return null;
        }

        // 연구 완료 처리
        CompleteResearch();
    }

    /// 연구 완료 시 호출
    private void CompleteResearch()
    {
        researchComplete = true;
        researchSilhouette.gameObject.SetActive(false);
        resultImage.gameObject.SetActive(true);
        resultImage.sprite = researchResult.itemSprite;

        UpdateDebugLog($"조합 완료! 결과: {researchResult.itemName}");
    }

    #endregion

    #region Result Collection (결과 수집)
    /// 연구 결과 수집 버튼 클릭 시 호출
    private void CollectResult()
    {
        if (!researchComplete)
        {
            UpdateDebugLog("연구가 아직 완료되지 않았습니다.");
            return;
        }

        UpdateDebugLog($"연구 결과 아이템 획득: {researchResult.itemName}");
        ResetResearchSlot();
    }

    /// 연구 슬롯을 초기 상태로 리셋
    /// 연구 슬롯을 초기 상태로 리셋
    private void ResetResearchSlot()
    {
        researchComplete = false;
        researchResult = null;

        researchSilhouette.gameObject.SetActive(false);
        resultImage.gameObject.SetActive(false);
        timerBar.value = 0;

        // 슬롯 상태 초기화
        slotA.ClearUI();
        slotB.ClearUI();

        // 슬롯 데이터 초기화
        Managers.Data.combinationSlots[0].itemSo = null;
        Managers.Data.combinationSlots[0].itemCount = 0;

        Managers.Data.combinationSlots[1].itemSo = null;
        Managers.Data.combinationSlots[1].itemCount = 0;

        UpdateDebugLog("연구 슬롯 및 데이터 초기화됨.");
    }


    #endregion

    #region Debugging (디버그)

    /// 디버그 로그를 업데이트합니다.
    private void UpdateDebugLog(string message)
    {
        if (debugLogText != null)
        {
            debugLogText.text = message;
        }
        else
        {
            Debug.LogWarning("DebugLogText가 설정되지 않았습니다.");
        }
    }
    #endregion
}
