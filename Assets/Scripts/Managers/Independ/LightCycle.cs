using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightCycle : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField] private Light2D _globalLight;

    [Header("Lights Color")]
    [SerializeField] private Color _noonLightColor;
    [SerializeField] private Color _nightLightColor;

    [Header("Animation Curve")]
    [SerializeField] private AnimationCurve _globalColorAnimationCurve;

    private void Start()
    {
        GameManager.Instance.TimeM.OnTimeCheck += LerpLight;

        LerpLight();
    }

    /// <summary>
    /// 다음 조명색으로 부드럽게 전환하기 위한 메서드
    /// </summary>
    private void LerpLight()
    {
        _globalLight.color = Color.Lerp(_nightLightColor, _noonLightColor, _globalColorAnimationCurve.Evaluate((GameManager.Instance.TimeM.InGameHour + (GameManager.Instance.TimeM.InGameMinute / 60f))));

    }
}