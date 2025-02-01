[System.Flags] // 플래그로 여러 계절 조합 가능
public enum Season
{
    None = 0,
    Spring = 1,
    Summer = 2,
    Fall = 4,
    Winter = 8,
    All = Spring | Summer | Fall | Winter // 모든 계절
}


public enum EWeekday
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday
}


public enum ESeason
{
    Spring,
    Summer,
    Fall,
    Winter,
    All
}