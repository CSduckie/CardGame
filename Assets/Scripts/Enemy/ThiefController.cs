using UnityEngine;
using System.Collections.Generic;

public class ThiefController : EnemyController
{
    public void TakeAction()
    {
        //目前的设计是精英敌人会随机让一个卡牌的AttackValue为0
        GameBoardController gameBoard = FindFirstObjectByType<GameBoardController>();
        var slotCount = gameBoard.transform.childCount;
        List<Card> tempCardList = new();
        for(int i = 0; i < slotCount; i++)
        {
            var slot = gameBoard.transform.GetChild(i).GetComponent<SlotController>();
            if(slot.currentCard != null)
            {
                tempCardList.Add(slot.currentCard);
            }
        }
        if(tempCardList.Count > 0)
        {
            var randomCard = tempCardList[Random.Range(0, tempCardList.Count)];
            randomCard.attackText.text = "0";
            //更新UI
            GameManager.Instance.gamePlayPanel.UpdateDamageUI();
            gameBoard.UpdateEnemyPredictHealth();
        }
    }
}
