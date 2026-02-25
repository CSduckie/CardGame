using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CleanSpecialGridEffect", menuName = "Card Effects/CleanSpecialGridEffect")]
public class CleanSpecialGridEffect : Effect
{
    //目前的设计是，在place使用中，遍历，如果当前使用卡牌是skill卡牌，则随机删除一个特殊地格的效果
    public override void Execute(Card from)
    {
        if(from.cardData.cardType != CardType.Skill) return;

        //创建一个空列表存储特殊地格
        List<SlotController> specialGridList = new List<SlotController>();
        //遍历棋盘上的所有特殊地格，随机删除一个特殊地格的效果
        foreach(var slot in GameManager.Instance.gameBoardController.transform.GetComponentsInChildren<SlotController>())
        {
            if(slot.isSpecial)
            {
                specialGridList.Add(slot);
            }
        }
        //随机删除一个特殊地格的效果
        if(specialGridList.Count > 0)
        {
            int randomIndex = Random.Range(0, specialGridList.Count);
            specialGridList[randomIndex].isSpecial = false;
            specialGridList[randomIndex].specialGridType = SpecialGridType.None;
            specialGridList[randomIndex].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }

    //在回合结束时，清除当前所在的特殊地格
    //当前的设计是，每当Sapper移动到下一个格子时，清除当前所在的特殊地格，同时自身变为Multiply状态
    public override void ExecuteOnTurnEnd(Card from)
    {
        if(from.cardData.cardType != CardType.Soldier) return;
        //清除当前所在的特殊地格
        //解除当前的负面状态
        from.isFreeze = false;
        from.isPoison = false;
        from.onTower = false;
        SlotController currentSlot = from.transform.parent.GetComponent<SlotController>();
        if(currentSlot.isSpecial)
        {
            currentSlot.isSpecial = false;
            currentSlot.specialGridType = SpecialGridType.None;
            currentSlot.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            from.isMultiply = true;
            from.multiplyText.text = "X";
        }
        else
        {
            Debug.Log("当前所在的特殊地格不是特殊地格");
            from.isMultiply = false;
            from.multiplyText.text = "+";
        }
    }
    public override void ExecuteOnDestroy(Card from){}
    public override void ExecuteOnOtherCardsDie(Card from){}
}
