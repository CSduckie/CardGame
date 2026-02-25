using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FarmerLegionEffect", menuName = "Card Effects/FarmerLegionEffect")]
public class FarmerLegionEffect : Effect
{
    //目前的设计是：检查场上的农民卡牌数量，然后增加攻击力1
    //农民攻击力 = 农民基础攻击力 + slot 修正 + 农民数量修正
    // 第一张，+1， 第二张，+2， 第三张，+3， 以此类推,
    public override void Execute(Card from)
    {
        //创建一个空列表，存储所有的农民卡牌
        List<Card> farmerCards = new List<Card>();

        //遍历所有的卡牌，如果卡牌是农民，则添加到列表中
        foreach(var card in GameManager.Instance.gameBoardController.transform.GetComponentsInChildren<Card>())
        {
            if(card.cardData.cardName == "Farmer")
            {
                farmerCards.Add(card);
            }
        }

        int sameCardCount = farmerCards.Count - 1;

        //赋值所有的农民卡牌
        foreach(var card in farmerCards)
        {
            int cardNewAttack = 0;
            card.cardAttackModifier = sameCardCount;

            //使用GameBoard中的相同方法进行重新计算
            //先计算基础攻击+slot修正 + 农民数量修正
            int cardBaseAttack = card.cardData.Attack + card.cardAttackModifier;
            SlotController targetSlot = card.transform.parent.GetComponent<SlotController>();
            if(targetSlot.isMultiply)
            {
                cardNewAttack = cardBaseAttack * targetSlot.value;
            }
            else
            {
                cardNewAttack = cardBaseAttack + targetSlot.value;
            }
            //最后计算地格修正
            if(targetSlot.isSpecial)
            {
                card.CardOnSpecialGridEffect(targetSlot.specialGridType);
            }

            //更新UI
            card.attackText.text = cardNewAttack.ToString();
        }

        //清空列表
        farmerCards.Clear();
        //更新UI
        GameManager.Instance.gamePlayPanel.UpdateDamageUI();
    }

    public override void ExecuteOnTurnEnd(Card from){}
    public override void ExecuteOnDestroy(Card from){}
    public override void ExecuteOnOtherCardsDie(Card from){}
}
