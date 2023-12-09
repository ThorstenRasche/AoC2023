namespace AoC2023.Domain.Day7;

public partial class Day7Calculator
{
    private class CardComparer : IComparer<char>
    {
        public int Compare(char x, char y) => "A23456789TJQK".IndexOf(y).CompareTo("A23456789TJQK".IndexOf(x));
    }
}

