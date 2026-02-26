using UnityEngine;
using DG.Tweening;
public class GameBoardController : MonoBehaviour
{
    [Header("棋盘信息")]
    public int row, column;
    
    [Header("UI")]
    private GamePlayPanel gamePlayPanel;

    [Header("临时增加的伤害")]
    public int tempAddValue = 0;
    public int tempMultiplyValue = 0;

    private void Start()
    {
        GameManager.Instance.gameBoardController = this;
        gamePlayPanel = FindFirstObjectByType<GamePlayPanel>();
        gamePlayPanel.gameBoardController = this;
    }

    //移动卡片到右边一格
    public void MoveCardToRight(int _row, int _column, Card _card)
    {
        //获取移动的目标slot
        var targetSlot = transform.GetChild((_row-1) * column + _column ).GetComponent<SlotController>();
        //移动前，先更新目标slot的数据
        targetSlot.currentCard = _card;
        targetSlot.isEmpty = false;
        _card.isPlaced = true;

        var currentSlot = transform.GetChild((_row-1) * column + _column - 1).GetComponent<SlotController>();
        currentSlot.currentCard = null;
        currentSlot.isEmpty = true;

#region 动画序列
        //创建移动动画序列
        Sequence cardFlipAnim = DOTween.Sequence();
        Sequence cardMoveAnim = DOTween.Sequence();
        //获取卡背
        var cardBack = _card.transform.GetChild(1).gameObject;
        //获取卡面
        var cardEntry = _card.transform.GetChild(0).gameObject;

        //首先执行卡片ENTRY的缩放动画，然后执行卡片BACK的缩放动画
        //TODO: 这里需要优化，如果动画时间过长，会出现报空
        cardFlipAnim.Append(cardEntry.transform.DOScale(new Vector3(0, 1, 1), 0.25f));
        cardFlipAnim.Append(cardBack.transform.DOScale(new Vector3(-1, 1, 1), 0.25f));

        //初始化移动动画，
        cardMoveAnim.Append(_card.transform.DOMove(targetSlot.transform.position, 0.5f));

        cardMoveAnim.Append(cardBack.transform.DOScale(new Vector3(0, 1, 1), 0.25f));
        cardMoveAnim.Append(cardEntry.transform.DOScale(new Vector3(1, 1, 1), 0.25f));



        cardFlipAnim.Play();
        //设置动画的播放延迟
        cardMoveAnim.SetDelay(0.5f);
        cardFlipAnim.onComplete = () =>
        {
            //在卡片翻面后更新UI，防止穿帮
            UpdateCardUI(_card, targetSlot);
            cardFlipAnim.Kill();
        };

        cardMoveAnim.onComplete = () =>
        {
            //在卡片翻面后更新UI，防止穿帮
            //更新卡片UI
            _card.transform.SetParent(targetSlot.transform);
            _card.transform.localPosition = Vector3.zero;
            _card.transform.localRotation = Quaternion.identity;
            _card.transform.localScale = Vector3.one * 1.1f;

            //在正确更新了父子关系后，执行卡牌的每个Effect的OnTurnEnd效果
            _card.CardOnTurnEndEffect(_card);
            cardMoveAnim.Kill();
        };

#endregion
    }


    //更新卡片UI
    private void UpdateCardUI(Card _card, SlotController _cardSlot)
    {
        //更新卡片UI
        //新的伤害值
        int newCardValue = 0;
        //判断当前卡牌和新slot是否都是Multiply
        int cardBaseAttack = _card.cardData.Attack + _card.cardAttackModifier;
        //Debug.Log("cardBaseAttack: " + cardBaseAttack);
        //Debug.Log("_card.cardAttackModifier: " + _card.cardAttackModifier);
        if (_cardSlot.isMultiply)
        {
            newCardValue = cardBaseAttack * _cardSlot.value;
        }
        else
        {
            newCardValue = cardBaseAttack + _cardSlot.value;
        }

        //检查卡牌是否处于不受任何modifier影响状态
        if (!_card.GetComponent<Card>().isIgnoreModifier)
        {
            _card.GetComponent<Card>().attackText.text = newCardValue.ToString();
        }

        //检查是否是特殊格子
        if (_cardSlot.isSpecial)
        {
            //传入TargetSlot
            _card.GetComponent<Card>().CardOnSpecialGridEffect(_cardSlot);
        }
    }

    public bool isRightHaveObject(int _row, int _column)
    {
        if(_column == column) return true;
        var targetSlot = transform.GetChild((_row-1) * column + _column).GetComponent<SlotController>();
        //Debug.Log("TargetSlot索引" + ((_row-1) * column + _column));
        //Debug.Log("TargetSlot是否有东西" + (targetSlot.currentCard != null));
        if(targetSlot.currentCard != null && targetSlot.currentCard.isFreeze) 
        {
            //Debug.Log("TargetSlot是冻结的");
            return true;
        }
        return false;
    }

    //造成伤害计算
    //于回合结束时调用
    public void CalculateDamage()
    {
        var slotCount = transform.childCount;
        int totalDamage = 0;
        int addValue = 0;
        int multiplyValue = 1;
        for(int i = 0; i < slotCount; i++)
        {
            if(transform.GetChild(i).GetComponent<SlotController>().currentCard != null)
            {
                Card currentCard = transform.GetChild(i).GetComponentInChildren<Card>();
                SlotController currentSlot = transform.GetChild(i).GetComponent<SlotController>();

                if(currentCard.isMultiply)  
                {
                    multiplyValue += int.Parse(currentCard.attackText.text);
                }
                else
                {
                    addValue += int.Parse(currentCard.attackText.text);
                }

            }
        }
        //全部执行完成后，计算总伤害
        totalDamage = (addValue+tempAddValue) * (multiplyValue+tempMultiplyValue);

        //更新敌人UI
        //通知敌人
        EnemyController enemy = FindFirstObjectByType<EnemyController>();
        enemy.TakeDamage(totalDamage);

        //清空临时增加的伤害
        tempAddValue = 0;
        tempMultiplyValue = 0;

        //Debug.Log("当前伤害：" + totalDamage);
    }

    //伤害预测计算
    //更新临时增加的伤害
    //主要用于技能卡牌或者卡牌放置效果，临时的增加伤害
    public void UpdateTempDamage(bool _isMultiply,int _value)
    {
        if(_isMultiply)
        {
            tempMultiplyValue += _value;
        }
        else
        {
            tempAddValue += _value;
        }
    }

    public void UpdateEnemyPredictHealth()
    {
        var slotCount = transform.childCount;
        int totalDamage = 0;
        int addValue = int.Parse(gamePlayPanel.damageCalculationUI.addValueText.text);
        int multiplyValue = int.Parse(gamePlayPanel.damageCalculationUI.multiplyValueText.text);

        /*
        for(int i = 0; i < slotCount; i++)
        {
            if(transform.GetChild(i).GetComponent<SlotController>().currentCard != null)
            {
                Card currentCard = transform.GetChild(i).GetComponentInChildren<Card>();
                SlotController currentSlot = transform.GetChild(i).GetComponent<SlotController>();

                if(currentCard.cardData.isMultiply)
                {
                    multiplyValue += int.Parse(currentCard.attackText.text);
                }
                else
                {
                    addValue += int.Parse(currentCard.attackText.text);
                }

            }
        }
        */
        //全部执行完成后，计算总伤害
        totalDamage = addValue * multiplyValue;
        gamePlayPanel.enemyUI.UpdateEnemyPredictHealth(totalDamage);
    }

    //检查棋盘是否还有士兵
    public bool CheckHasSoilderOnBoard()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<SlotController>().currentCard != null)
            {
                if(transform.GetChild(i).GetComponent<SlotController>().currentCard.cardType == CardType.Soldier)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
