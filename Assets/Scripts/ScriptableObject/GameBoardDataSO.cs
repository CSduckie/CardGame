using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameBoardData", menuName = "GameBoard/GameBoardDataSO")]
public class GameBoardDataSO : ScriptableObject
{
    public List<columnData> columnDataList = new();
}

[System.Serializable]
public class columnData
{
    public int value;
    public bool isMultiply = false;
}
