using UnityEngine;


public class BedPopup : PopupUI
{
    /// <summary>
    /// 취침 버튼 클릭시 작동하는 메서드, 하루를 넘김
    /// </summary>
    public void OnClickSleepButton()
    {
        GameManager.Instance.TimeM.Sleep(6, Setting.SpawnPoint);
        Managers.Sound.ControlMusicPlayer(MusicPlayerOption.Stop);
    }


    /// <summary>
    /// '아니오' 버튼 클릭시 작동, BedPopup UI를 꺼줌
    /// </summary>
    public void OnClickCancelSleepButton()
    {
        Managers.UI.CloseAllPopup();
    }
}