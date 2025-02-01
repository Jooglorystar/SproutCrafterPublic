using UnityEngine;


public class SceneTeleporter : Teleporter
{
    [SerializeField] private SceneName _targetSceneName = SceneName.Farm;

    protected override void Teleport()
    {
        GameManager.Instance.SceneControlM.MoveSceneWithFade(_targetSceneName, _targetScenePosition);

        if (_teleportType == TeleportType.SceneToBuilding)
        {
            Managers.Sound.PlaySfx(SfxEnums.DoorOpen);
        }

        if (_teleportType == TeleportType.BuildingToScene)
        {
            Managers.Sound.PlaySfx(SfxEnums.DoorClose);
        }
    }
}