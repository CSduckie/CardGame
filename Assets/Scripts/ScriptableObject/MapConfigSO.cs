using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "MapConfig", menuName = "Map/MapConfigSO")]
public class MapConfigSO : ScriptableObject
{
    public List<RoomBluePrint> roomBluePrints;
}

/// <summary>
/// 房间蓝图
/// </summary>
[System.Serializable]
public class RoomBluePrint
{
    public int min,max;
    public RoomType roomType;
}
