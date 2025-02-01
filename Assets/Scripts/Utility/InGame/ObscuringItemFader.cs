using System.Collections;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class ObscuringItemFader : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }
    

    IEnumerator FadeOutRoutine()
    {
        float currentAlpha = _spriteRenderer.color.a;
        float targetAlpha = Setting.TargetAlpha;
        float distance = currentAlpha - targetAlpha;

        while (Mathf.Abs(currentAlpha - targetAlpha) > 0.01f)
        {
            currentAlpha -= distance / Setting.FadeInOutSeconds * Time.deltaTime;
            _spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }
        _spriteRenderer.color = new Color(1f, 1f, 1f, targetAlpha);
    }

    
    public void FadeIn()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        StartCoroutine(FadeInRoutine());
    }

    
    IEnumerator FadeInRoutine()
    {
        float currentAlpha = _spriteRenderer.color.a;
        float distance = 1f - currentAlpha;

        while (1f - currentAlpha > 0.01f)
        {
            currentAlpha += distance / Setting.FadeInOutSeconds * Time.deltaTime;
            _spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }
        _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }
}