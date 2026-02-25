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

public enum EnemyType
{
    MinorEnemy,
    EliteEnemy,
    Boss,
}


[Flags]
public enum SpecialGridType
{
    None,//普通格子
    Cold,//冻结，不能移动
    Tower,//在塔上，伤害+2
    Posion,//中毒，攻击力/2
    Trap,//陷阱，清除卡牌，然后地格变为None。
}