namespace Server.Helpers;

public static class ContentFilter
{
    private static List<string> AbsentWord = new() {"блядь", "хуй","пизда","ебать"};

    public static bool ContainsAbsentWord(IEnumerable<string> text)
    {
        return text.Any(word => AbsentWord.Contains(word));
    }
}