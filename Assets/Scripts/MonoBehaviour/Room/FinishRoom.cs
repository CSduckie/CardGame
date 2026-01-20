using UnityEngine;

public class FinishRoom : MonoBehaviour
{
    //返回地图
    public ObjectEventSO loadMapEvent;

    private void OnMouseDown() 
    {
        loadMapEvent.RaisEvent(null, this);
    }
}
