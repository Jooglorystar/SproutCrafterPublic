using UnityEngine;
using UnityEngine.SceneManagement;


public class QuitGamePopup : PopupUI
{
    private enum QuitGameType
    {
        None,
        GoTitle,
        Quit
    }

    [SerializeField] private GameObject _conrfirmationPanel;

    private QuitGameType _quitGameType = QuitGameType.None;

    public void OnEnable()
    {
        _conrfirmationPanel.SetActive(false);
    }

    public void OnClickBackTitleGame()
    {
        _quitGameType = QuitGameType.GoTitle;
        _conrfirmationPanel.SetActive(true);
    }


    public void OnClickQuitGame()
    {
        _quitGameType = QuitGameType.Quit;
        _conrfirmationPanel.SetActive(true);
    }

    public void OnClickConfirmQuit()
    {
        _conrfirmationPanel.SetActive(false);
        switch (_quitGameType)
        {
            case QuitGameType.GoTitle:
                SceneManager.LoadScene(0);
                break;
            case QuitGameType.Quit:
                Application.Quit();
                break;
            default:
                break;
        }
    }
}