using System.Collections;
using UnityEngine;

public class LocalTeleporter : Teleporter
{
    [SerializeField] private bool _isFade;

    protected override void Teleport()
    {
        if (_isFade)
        {
            GameManager.Instance.StartFade(() => MovePosition());
        }
        else
        {
            StartCoroutine(MovePosition());
        }

        if (_teleportType == TeleportType.SceneToBuilding)
        {
            Managers.Sound.PlaySfx(SfxEnums.DoorOpen);
        }

        if (_teleportType == TeleportType.BuildingToScene)
        {
            Managers.Sound.PlaySfx(SfxEnums.DoorClose);
        }
    }

    private IEnumerator MovePosition()
    {
        GameManager.Instance.CharacterM.gameObject.transform.position = _targetScenePosition;
        yield return null;
    }
}