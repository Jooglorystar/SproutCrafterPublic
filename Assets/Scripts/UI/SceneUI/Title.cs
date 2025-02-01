using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;


public class Title : SceneUI
{
    private float _doFadeDuration = 0.2f;
    
    [SerializeField] private GameObject[] _buttons;
    
    
    public override void Init()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {           
            _buttons[i].gameObject.transform.localScale = Vector3.one * 0.05f;
            _buttons[i].SetActive(false);
        }
        
        base.Init();
        DOTween.Init();
        
        StartCoroutine(FadeUI());
        
        Managers.Sound.PlayMusic(MusicEnums.Title);
    }


    private IEnumerator FadeUI()
    {
        yield return new WaitForSeconds(0.5f);
        
        for (int i = 0; i < _buttons.Length; i++)
        {           
            _buttons[i].SetActive(true);
            _buttons[i].gameObject.transform.DOScale(1f, _doFadeDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() => _buttons[i].gameObject.transform.localScale = Vector3.one);
            
            yield return new WaitForSeconds(0.1f); // 캐싱?
        }
    }


    public void OnClickStartGameButton()
    {
        Managers.UI.OnEnableUI<TitleStartGame>();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
    }


    public void OnClickOptionButton()
    {
        Managers.UI.OnEnableUI<TitleOption>();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
    }

    public void OnClickManualButton()
    {
        Managers.UI.OnEnableUI<TitleManual>();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
    }

    public void OnClickQuitGameButton()
    {
        Application.Quit();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
    }


    public void OnClickCreditsButton()
    {
        Managers.UI.OnEnableUI<CreditsPopup>();
    }


    public void OnPointerEnter(Image p_Image)
    {
        Color color = new Color(0.5f, 0.5f, 0.5f, 1);
        
        p_Image.DOColor(color, _doFadeDuration);
    }


    public void OnPointerExit(Image p_Image)
    {       
        Color color = new Color(1f, 1f, 1f, 1);
        
        p_Image.DOColor(color, _doFadeDuration);
    }


    public void OnPointerClick(Image p_Image) // 클릭을 하고 마우스가 본인 오브젝트에서  Up 되었을 경우
    {
        Color color = new Color(1f, 1f, 1f, 1);
        
        p_Image.DOColor(color, _doFadeDuration);
    }
}