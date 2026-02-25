using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public int value;
    public EffectTargetType targetType;
    //卡牌具体执行效果的抽象函数
    public abstract void Execute(Card from);
    
    public abstract void ExecuteOnTurnEnd(Card from);
    //卡牌自身死亡事件
    public abstract void ExecuteOnDestroy(Card from);
    //其他卡牌死亡事件
    public abstract void ExecuteOnOtherCardsDie(Card from);
}
