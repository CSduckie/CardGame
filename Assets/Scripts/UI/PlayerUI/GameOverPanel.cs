using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [Header("事件广播")]
    public ObjectEventSO loadMenuEvent;
    public void OnBackToStartButtonClicked()
    {
        loadMenuEvent.RaisEvent(null, this);
        GameManager.Instance.isGameFailed = true;
    }
}
