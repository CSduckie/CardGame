using UnityEngine;

[CreateAssetMenu(fileName = "MoveChangeValueEffect", menuName = "Card Effects/MoveChangeValueEffect")]
public class MoveChangeValueEffect : Effect
{
    /// <summary>
    /// 应用此卡片的效果的卡牌有：
    /// /// 武士 每一格 +1
    /// /// 重甲士兵 每一格 -1
    /// </summary>
    /// <param name="from"></param>
    public override void Execute(Card from){}
    public override void ExecuteOnTurnEnd(Card from)
    {
        if(from.cardData.cardType != CardType.Soldier) return;

        //更改当前modifier
        from.cardAttackModifier += value;

    }
    public override void ExecuteOnDestroy(Card from){}
}
