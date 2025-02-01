using UnityEngine;
using UnityEngine.UI;

public class TitleManual : PopupUI
{
    public override void Init()
    {
        base.Init();
    }

    public override void Close()
    {
        base.Close();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
    }
}