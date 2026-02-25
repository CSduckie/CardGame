using UnityEngine;

[CreateAssetMenu(fileName = "OtherCardsDieAddValueEffect", menuName = "Card Effects/OtherCardsDieAddValueEffect")]
public class OtherCardsDieAddValueEffect : Effect
{

    public override void Execute(Card from){}
    public override void ExecuteOnTurnEnd(Card from){}
    public override void ExecuteOnDestroy(Card from){}

    public override void ExecuteOnOtherCardsDie(Card from)
    {
        //如果其他卡牌死亡，则增加当前卡牌的攻击力
        from.cardAttackModifier += value;
        //Debug.Log("OtherCardsDieAddValueEffect: " + value);
    }
}
