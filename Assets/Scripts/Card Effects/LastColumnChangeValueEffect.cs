using UnityEngine;

[CreateAssetMenu(fileName = "LastColumnChangeValueEffect", menuName = "Card Effects/LastColumnChangeValueEffect")]
public class LastColumnChangeValueEffect : Effect
{
    public override void Execute(Card from)
    {
        from.isIgnoreModifier = true;
        from.isIgnoreSlotEffect = true;
    }

    public override void ExecuteOnTurnEnd(Card from)
    {
        if(from.cardData.cardType != CardType.Soldier) return;
        //检查是否是最后一列
        if(from.transform.parent.GetComponent<SlotController>().Column == GameManager.Instance.gameBoardController.column)
        {
            //是最后一格，则随机概率，60%概率触发,生效则在5-value之间随机一个值，不生效则卡牌摧毁
            if(Random.value < 0.6f)
            {
                from.attackText.text = (int.Parse(from.attackText.text) + Random.Range(5,value)).ToString();
            }
            else
            {
                var currentSlot = from.transform.parent.GetComponent<SlotController>();
                currentSlot.currentCard = null;
                currentSlot.isEmpty = true;
                Destroy(from.gameObject);
            }
        }
    }
    public override void ExecuteOnDestroy(Card from){}
}
