using UnityEngine;


public class PopupUI : MonoBehaviour
{
    public virtual void Init()
    {
        
    }


    public virtual void Close()
    {
        Managers.UI.ClosePopup(this.gameObject);
    }
    
    
    public virtual void CloseAll()
    {
        Managers.UI.CloseAllPopup();
    }
}