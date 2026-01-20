using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapLayoutSO", menuName = "Map/MapLayoutSO")]
public class MapLayoutSO : ScriptableObject
{
    public List<MapRoomData> mapRoomDataList = new();
    public List<LinePosition> linePositionsList = new();
}

[System.Serializable]
public class MapRoomData
{
    public float posX, posY;
    public int colum, line;
    public RoomDataSO roomData;
    public RoomState roomState;
    public List<Vector2Int> linkTo;
}

[System.Serializable]
public class LinePosition
{
    public SerializeVector3 startPos, endPos;
}