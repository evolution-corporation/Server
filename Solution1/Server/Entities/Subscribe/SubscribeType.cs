namespace Server.Entities;

public enum SubscribeType
{
    Week,
    Month,
    Month6
}

public static class SubscribeTypeConverter
{
    public static int GetSubscribeTime(SubscribeType type)
    {
        return type switch
        {
            SubscribeType.Week => 7,
            SubscribeType.Month => 30,
            SubscribeType.Month6 => 180,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    
    public static int GetSubscribePrice(SubscribeType type)
    {
        return type switch
        {
            SubscribeType.Week => 100,
            SubscribeType.Month => 29900,
            SubscribeType.Month6 => 124900,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}