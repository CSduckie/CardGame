using UnityEngine;

[CreateAssetMenu(fileName = "ChangeIsMultiplyEffect", menuName = "Card Effects/ChangeIsMultiplyEffect")]
public class ChangeIsMultiplyEffect : Effect
{
    public override void Execute(Card from)
    {
        Debug.Log("ChangeIsMultiplyEffect");
        //目前的设计是，检测卡牌右侧是否有别的卡牌，如果有那么就改变卡牌的isMultiply状态
        GameBoardController gameBoard = FindFirstObjectByType<GameBoardController>();

        bool isRightHaveSolder = false;
        int myRow = from.transform.parent.GetComponent<SlotController>().Raw;
        int myColumn = from.transform.parent.GetComponent<SlotController>().Column;


        var targetSlot = gameBoard.transform.GetChild((myRow-1) * gameBoard.column + myColumn).GetComponent<SlotController>();

        if(targetSlot.currentCard != null)
        {
            isRightHaveSolder = true;
        }
        else
        {
            isRightHaveSolder = false;
        }

        from.isMultiply = isRightHaveSolder;

        //更新卡牌UI
        from.multiplyText.text = from.isMultiply ? "X" : "+";
        //更新计算UI
        GameManager.Instance.gamePlayPanel.UpdateDamageUI();
        gameBoard.UpdateEnemyPredictHealth();
    }

    public override void ExecuteOnTurnEnd(Card from){}
    public override void ExecuteOnDestroy(Card from){}
}
