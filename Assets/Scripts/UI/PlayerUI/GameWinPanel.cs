using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameWinPanel : MonoBehaviour
{
    public Button SelectCardButton;

    [Header("事件广播")]
    public ObjectEventSO loadMapEvent;
    
    public void OnEnable()
    {
        SelectCardButton.interactable = true;
    }

    public void OnBackToMapButtonClicked()
    {
        loadMapEvent.RaisEvent(null, this);
        gameObject.SetActive(false);
        GameManager.Instance.DisableAllUI();
    }
}
