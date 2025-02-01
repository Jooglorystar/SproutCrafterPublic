

public class ChestPopup : PopupUI
{
    private void OnEnable()
    {
        GameManager.Instance.CharacterM.InputController.OnInteractPopupUI += CloseChestPopup;
        Managers.Sound.PlaySfx(SfxEnums.StorageOpen);
    }


    private void OnDisable()
    {
        GameManager.Instance.CharacterM.InputController.OnInteractPopupUI -= CloseChestPopup;
    }


    private bool CloseChestPopup()
    {
        Managers.Sound.PlaySfx(SfxEnums.StorageClose);
        Managers.UI.ClosePopup(this.gameObject);
        
        return true;
    }


    public override void Close()
    {
        base.Close();
        Managers.Sound.PlaySfx(SfxEnums.StorageClose);
    }
}