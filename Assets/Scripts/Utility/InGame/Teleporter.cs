using UnityEngine;

public abstract class Teleporter : MonoBehaviour
{
    [SerializeField] protected Vector3 _targetScenePosition = new Vector3();
    [SerializeField] private int _openTime = 0;
    [SerializeField] private int _closeTime = 24;
    [SerializeField] protected TeleportType _teleportType;

    protected abstract void Teleport();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && IsOpen())
        {
            Teleport();
        }
    }

    private bool IsOpen()
    {
        return GameManager.Instance.TimeM.InGameHour >= _openTime && GameManager.Instance.TimeM.InGameHour <= _closeTime;
    }
}