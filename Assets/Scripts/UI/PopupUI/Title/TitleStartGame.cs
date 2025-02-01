

public class TitleStartGame : PopupUI
{
    public void OnClickReturnGameButton()
    {
        Managers.UI.CloseAllPopup();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
    }
}