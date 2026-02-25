using UnityEngine;

[CreateAssetMenu(fileName = "IgnoreSloteffectEffect", menuName = "Card Effects/IgnoreSloteffectEffect")]
public class IgnoreSloteffectEffect : Effect
{
    public override void Execute(Card from)
    {
        from.isIgnoreSlotEffect = true;
    }
    public override void ExecuteOnTurnEnd(Card from){}
    public override void ExecuteOnDestroy(Card from){}
    public override void ExecuteOnOtherCardsDie(Card from){}
}
