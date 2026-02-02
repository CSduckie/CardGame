using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    [Header("事件广播")]
    public ObjectEventSO newGameEvent;

    public void OnNewGameButtonClicked()
    {
        newGameEvent.RaisEvent(null, this);
        GameManager.Instance.isGameFailed = false;
        GameManager.Instance.isFirstTurn = true;
        GameManager.Instance.DisableAllUI();
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
