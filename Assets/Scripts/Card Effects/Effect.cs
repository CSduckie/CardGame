using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public int value;
    public EffectTargetType targetType;
    //卡牌具体执行效果的抽象函数
    public abstract void Execute(Card from);
}
