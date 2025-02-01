using DG.Tweening;
using System;
using System.Collections;
using UnityEngine.UI;


public class GameManager : ConvertToSingleton<GameManager>
{
    public PoolManager PoolM { get; set; }
    public SceneControlManager SceneControlM { get; set; }
    public TimeManager TimeM { get; set; }
    public CharacterManager CharacterM { get; set; }
    public DataBaseManager DataBaseM { get; set; }
    public CropManager CropM { get; set; }
    public TreeManager TreeM { get; set; }
    public BuildingManager BuildingM { get; set; }
    public TilemapManager TilemapM { get; set; }
    public PlayerSkillManager PlayerSkillM { get; set; }
    public NPCManager NpcM { get; set; }
    public QuestManager QuestM { get; set; }
    

    public DynamicPricingSystem DynamicPricingSystem { get; private set; }


    private bool _isFading;
    public Image fadeImage;

    private float _targetAlpha;
    private float _fadeDuration = 1f;


    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        DynamicPricingSystem = new DynamicPricingSystem(DataBaseM.ItemDatabase);
        SaveManager.Instance.LoadGame();
        DataBaseM.ItemDatabase.Init();
        TimeM.InitTimeManager();
        EventManager.Dispatch(GameEventType.GameStart, null);
        Managers.Sound.OnFadeOutAudio();
    }


    private void OnDisable()
    {
        Managers.Cursor.IsTitleScene = true;
    }

    public void StartFade(Func<IEnumerator> coroutine)
    {
        if (!_isFading)
        {
            Managers.Cursor.IsInteracting = true;
            Managers.Cursor.SetCursorTexture(CursorTypes.Default);
            
            CharacterM.AnimationController.StopMove();
            CharacterM.inventory.StopScrollSlot();
            TimeM.IsPlayingTime = false;
            _targetAlpha = 1f;
            fadeImage.gameObject.SetActive(true);

            fadeImage.DOFade(_targetAlpha, _fadeDuration)
                .OnComplete(() => StartCoroutine(DuringFade(coroutine)));
        }
    }

    private IEnumerator DuringFade(Func<IEnumerator> coroutine)
    {
        yield return StartCoroutine(coroutine());

        EndFadeTreat();
    }

    private void EndFadeTreat()
    {
        _targetAlpha = 0f;

        fadeImage.DOFade(_targetAlpha, _fadeDuration)
            .SetDelay(0.5f)
            .SetEase(Ease.InQuart)
            .OnComplete(() => EndFadeOut());
    }

    private void EndFadeOut()
    {
        Managers.Cursor.IsInteracting = false;
        fadeImage.gameObject.SetActive(false);
        TimeM.IsPlayingTime = true;
        CharacterM.AnimationController.StartMove();
        CharacterM.inventory.StartScrollSlot();
    }
}