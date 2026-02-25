using UnityEngine;

[CreateAssetMenu(fileName = "IgnoreModifierEffect", menuName = "Card Effects/IgnoreModifierEffect")]
public class IgnoreModifierEffect : Effect
{
    public override void Execute(Card from)
    {
        from.cardAttackModifier = 0;
        from.isIgnoreModifier = true;
    }

    public override void ExecuteOnTurnEnd(Card from){}
    public override void ExecuteOnDestroy(Card from){}
    public override void ExecuteOnOtherCardsDie(Card from){}
}
