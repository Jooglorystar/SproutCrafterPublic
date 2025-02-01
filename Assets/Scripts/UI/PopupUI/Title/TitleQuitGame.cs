using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleQuitGame : PopupUI
{
    public override void Close()
    {
        base.Close();
    }


    public void OnClickQuitGame()
    {
        Application.Quit();
    }
}