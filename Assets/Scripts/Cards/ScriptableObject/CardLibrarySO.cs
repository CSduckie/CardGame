using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="CardLibrarySO",menuName ="Card/CardLibrarySO")]
public class CardLibrarySO : ScriptableObject
{
    public List<CardLibraryEntry> cardLibraryList;
}


//卡牌结构体，内部包含卡片数据和卡牌数量
[System.Serializable]
public struct CardLibraryEntry
{
    public CardDataSO cardData;
    public int amount;
}
