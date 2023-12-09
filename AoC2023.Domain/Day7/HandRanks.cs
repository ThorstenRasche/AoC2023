namespace AoC2023.Domain.Day7;

public partial class Day7Calculator
{
    private enum HandRanks
    {
        HighCard = 1,
        OnePair = 2,
        TwoPair = 3,
        ThreeOfAKind = 4,
        FullHouse = 5,
        FourOfAKind = 6,
        FiveMatch = 7
    }
}

