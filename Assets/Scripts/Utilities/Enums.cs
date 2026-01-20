using System;


[Flags]
public enum RoomType
{
    //根据2的N次方设置，可以允许在Inspector中多选
    MinorEnemy = 1,
    EliteEnemy = 4,
    Boss = 8,
    Shop = 16,
    Reward = 32,
}

public enum RoomState
{
    Active,
    Locked,
    Entered,
}

public enum CardType
{
    Soldier,
    Skill
}

public enum EffectTargetType 
{
    Self,
    Enemy,
    All,
}