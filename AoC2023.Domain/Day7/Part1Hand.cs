namespace AoC2023.Domain.Day7;

public partial class Day7Calculator
{
    private class Part1Hand : IComparable<Part1Hand>
    {
        public string cards;
        public long bid;
        public HandRanks HandType;
        public Part1Hand(string cards, long bid)
        {
            this.cards = cards;
            this.bid = bid;
            var groups = cards.GroupBy(x => x).Select(group => new { Card = group.Key, Count = group.Count() }).OrderByDescending(x => x.Count).ToList();
            switch (groups[0].Count)
            {
                case 5: HandType = HandRanks.FiveMatch; break;
                case 4: HandType = HandRanks.FourOfAKind; break;
                case 3: HandType = groups[1].Count == 2 ? HandRanks.FullHouse : HandRanks.ThreeOfAKind; break;
                case 2: HandType = groups[1].Count == 2 ? HandRanks.TwoPair : HandRanks.OnePair; break;
                default: HandType = HandRanks.HighCard; break;
            }
        }
        public int CompareTo(Part1Hand other)
        {
            if (HandType != other.HandType) return HandType - other.HandType;
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] == other.cards[i]) continue;
                switch (cards[i])
                {
                    case 'A': return 1;
                    case 'K': return other.cards[i] == 'A' ? -1 : 1;
                    case 'Q': return "AK".Contains(other.cards[i]) ? -1 : 1;
                    case 'J': return "AKQ".Contains(other.cards[i]) ? -1 : 1;
                    case 'T': return "AKQJ".Contains(other.cards[i]) ? -1 : 1;
                    case '9': return "AKQJT".Contains(other.cards[i]) ? -1 : 1;
                    case '8': return "AKQJT9".Contains(other.cards[i]) ? -1 : 1;
                    case '7': return "AKQJT98".Contains(other.cards[i]) ? -1 : 1;
                    case '6': return "AKQJT987".Contains(other.cards[i]) ? -1 : 1;
                    case '5': return "AKQJT9876".Contains(other.cards[i]) ? -1 : 1;
                    case '4': return "AKQJT98765".Contains(other.cards[i]) ? -1 : 1;
                    case '3': return "AKQJT987654".Contains(other.cards[i]) ? -1 : 1;
                    case '2': return -1;
                }
            }
            return 0;
        }
    }
}

