using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
public class GamePlayPanel : MonoBehaviour
{
    [Header("事件广播")]
    public ObjectEventSO playerTurnEndEvent;
    public Button endTurnButton;

    [Header("Buff UI")]
    public GameObject buffUIPanel;
    [Header("伤害计算UI")]
    public DamageUIPanel damageCalculationUI;
    [Header("敌人UI")]
    public EnemyUIController enemyUI;
    [Header("棋盘")]
    //于棋盘创建时进行赋值
    public GameBoardController gameBoardController;

    [Header("棋盘数据")]
    public GameBoardDataSO gameBoardData;

    private void Start()
    {
        buffUIPanel = transform.GetChild(1).gameObject;
    }

    public void OnPlayerTurnStart()
    {
        //检查棋盘上是否有卡牌，如果有，则启动结束回合按钮
        if(gameBoardController.CheckHasSoilderOnBoard())
            endTurnButton.interactable = true;
        else
            endTurnButton.interactable = false;

    }

    public void OnPlayerTurnEnd()
    {
        playerTurnEndEvent.RaisEvent(null, this);
        endTurnButton.interactable = false;
    }

    //计算棋盘生成时，最上方一列的数据
    public void OnGameStartUpdateUI()
    {
        TextMeshProUGUI[] allColumnDataUI = buffUIPanel.transform.GetComponentsInChildren<TextMeshProUGUI>();
        for(int i = 0; i < allColumnDataUI.Length; i++)
        {
            if(i == 0)
            {
                allColumnDataUI[i].text = "Start";
            }
            else
            {
                if(gameBoardData.columnDataList[i-1].isMultiply)
                {
                    allColumnDataUI[i].text = "X" + gameBoardData.columnDataList[i-1].value;
                }
                else
                {
                    allColumnDataUI[i].text = "+" + gameBoardData.columnDataList[i-1].value;
                }
            }
        }
    }


    //UI更新计算
    public void UpdateDamageUI()
    {
        int addValue = 0;
        int multiplyValue = 1;
        int totalValue = 0;

        List<Card> allCards = new List<Card>();
        //获取当前棋盘上所有的卡牌
        for(int i = 0; i < gameBoardController.transform.childCount; i++)
        {
            if(gameBoardController.transform.GetChild(i).GetComponent<SlotController>().currentCard != null)
            {
                allCards.Add(gameBoardController.transform.GetChild(i).GetComponentInChildren<Card>());
            }
        }

        foreach(var card in allCards)
        {
            //Debug.Log("卡牌名称：" + card.cardName.text);
            if(card.isMultiply)
            {
                multiplyValue += int.Parse(card.attackText.text);
            }
            else
            {
                addValue += int.Parse(card.attackText.text);
            }
        }

        //获取当前临时增加的伤害
        GameBoardController gameBoard = FindFirstObjectByType<GameBoardController>();
        int tempAddValue = gameBoard.tempAddValue;
        int tempMultiplyValue = gameBoard.tempMultiplyValue;

        //计算总伤害(包括临时增加的伤害)
        addValue += tempAddValue;
        multiplyValue += tempMultiplyValue;

        totalValue = addValue * multiplyValue;
        //Debug.Log("总伤害：" + totalValue);
        //更新UI
        StartCoroutine(UpdateDamageUIAnimation(damageCalculationUI.addValueText, addValue, 1f));
        StartCoroutine(UpdateDamageUIAnimation(damageCalculationUI.multiplyValueText, multiplyValue, 1f));
        StartCoroutine(UpdateDamageUIAnimation(damageCalculationUI.totalValueText, totalValue, 1f));
        /*
        damageCalculationUI.addValueText.text = addValue.ToString();
        damageCalculationUI.multiplyValueText.text = multiplyValue.ToString();
        damageCalculationUI.totalValueText.text = totalValue.ToString();
        */
    }

    private IEnumerator UpdateDamageUIAnimation(TextMeshProUGUI targetText, int targetValue,float animationTime)
    {
        float animationTimePerStep = animationTime / (targetValue - int.Parse(targetText.text));
        while(int.Parse(targetText.text) < targetValue)
        {
            targetText.text = (int.Parse(targetText.text) + 1).ToString();
            yield return new WaitForSeconds(animationTimePerStep);
        }
        targetText.text = targetValue.ToString();
    }
}
