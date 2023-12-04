namespace AoC23.Presentation;

public static class Extensions
{
    public static bool TryParseToInt(this string input, out int result, bool checkOneOrTwo = false)
    {
        result = 0;
        bool parseSuccess = int.TryParse(input, out result);

        if (checkOneOrTwo)
        {
            return parseSuccess && (result == 1 || result == 2);
        }

        return parseSuccess;
    }
}
