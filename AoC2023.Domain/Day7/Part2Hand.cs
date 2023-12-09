namespace AoC2023.Domain.Day7;

public partial class Day7Calculator
{
    private class Part2Hand : IComparable<Part2Hand>
    {
        public string cards;
        public long bid;
        public HandRanks HandType;
        public Part2Hand(string cards, long bid)
        {
            this.cards = cards;
            this.bid = bid;
            var groups = cards.GroupBy(x => x).Select(group => new { Card = group.Key, Count = group.Count() }).OrderByDescending(x => x.Count).ToList();
            switch (groups[0].Count)
            {
                case 5: HandType = HandRanks.FiveMatch; break;
                case 4: HandType = groups.Any(x => x.Card == 'J') ? HandRanks.FiveMatch : HandRanks.FourOfAKind; break;
                case 3:
                    if (groups[1].Count == 2)
                    {
                        if (groups[0].Card == 'J' || groups[1].Card == 'J') { HandType = HandRanks.FiveMatch; break; }
                        else { HandType = HandRanks.FullHouse; break; }
                    }
                    else
                    {
                        if (groups.Any(x => x.Card == 'J')) { HandType = HandRanks.FourOfAKind; break; }
                        else { HandType = HandRanks.ThreeOfAKind; break; }
                    }
                case 2:
                    if (groups[1].Count == 2)
                    {
                        if (groups[0].Card == 'J' || groups[1].Card == 'J') { HandType = HandRanks.FourOfAKind; break; }
                        else if (groups[2].Card == 'J') { HandType = HandRanks.FullHouse; break; }
                        else { HandType = HandRanks.TwoPair; break; }
                    }
                    else
                    {
                        if (groups.Any(x => x.Card == 'J')) { HandType = HandRanks.ThreeOfAKind; break; }
                        else { HandType = HandRanks.OnePair; break; }
                    }
                default:
                    HandType = groups.Any(x => x.Card == 'J') ? HandRanks.OnePair : HandRanks.HighCard;
                    break;
            }
        }
        public int CompareTo(Part2Hand other)
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
                    case 'T': return "AKQ".Contains(other.cards[i]) ? -1 : 1;
                    case '9': return "AKQT".Contains(other.cards[i]) ? -1 : 1;
                    case '8': return "AKQT9".Contains(other.cards[i]) ? -1 : 1;
                    case '7': return "AKQT98".Contains(other.cards[i]) ? -1 : 1;
                    case '6': return "AKQT987".Contains(other.cards[i]) ? -1 : 1;
                    case '5': return "AKQT9876".Contains(other.cards[i]) ? -1 : 1;
                    case '4': return "AKQT98765".Contains(other.cards[i]) ? -1 : 1;
                    case '3': return "AKQT987654".Contains(other.cards[i]) ? -1 : 1;
                    case '2': return "AKQT9876543".Contains(other.cards[i]) ? -1 : 1;
                    case 'J': return -1;
                }
            }
            return 0;
        }
    }
}

