using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardDataSO", menuName = "CardDataSO")]
public class CardDataSO : ScriptableObject
{
    public string cardName;
    public Sprite cardImage;
    public CardType cardType;
    public bool isMultiply;
    public int Attack, health, cost;
    
    //卡牌效果列表
    public List<Effect> effects;
    public string description;
}
