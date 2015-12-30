using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitareAStar
{
    class Program
    {
        static Random random = new Random(16);

        static void Main(string[] args)
        {
            var start = DateTime.Now;
            var initialState = setupInitial();

            HashSet<int> closedSet = new HashSet<int>();

            List<SolitareState> newStates = new List<SolitareState>();
            newStates.Add(initialState);
            int collision = 0;
            int iterations = 0;
            while (newStates.Count > 0)
            {
                var solitareState = newStates[0];

                if (solitareState.GetScore() == int.MaxValue)
                {
                    Console.WriteLine("WON " + (DateTime.Now - start));
                    Console.WriteLine("Iterations per second:" + (iterations / (DateTime.Now - start).TotalMilliseconds) * 1000);
                    Console.WriteLine(newStates.Count + " " + closedSet.Count + " " + collision);
                    Console.WriteLine(solitareState.ToString());
                    Console.ReadLine();
                    return;
                }


                iterations++;
                if (iterations % 5000 == 0)
                {
                    Console.WriteLine(newStates.Count + " " + iterations + " " + closedSet.Count + " " + collision);

                    Console.WriteLine("Iterations per second:" + (iterations / (DateTime.Now - start).TotalMilliseconds) * 1000);
                    Console.WriteLine(solitareState.ToString());

                }
                newStates.Remove(solitareState);

                var solitareStates = oneTick(solitareState);
                var count = solitareStates.Count;

                for (int index = 0; index < count; index++)
                {
                    var state = solitareStates[index];
                    var item = state.GetHashCode();
                    if (!closedSet.Contains(item))
                    {
                        closedSet.Add(item);
                        newStates.Add(state);
                    }
                    else
                    {
                        collision++;
                    }
                }
                newStates = newStates.OrderByDescending(a => a.GetScore()).ToList();
            }
            Console.WriteLine("LOST " + (DateTime.Now - start));
            Console.WriteLine("Iterations per second:" + (iterations / (DateTime.Now - start).TotalMilliseconds) * 1000);
            Console.WriteLine(newStates.Count + " " + iterations + " " + closedSet.Count + " " + collision);
            Console.ReadLine();
        }

        private static List<SolitareState> oneTick(SolitareState solitareState)
        {
            List<SolitareState> s = new List<SolitareState>();

            s.AddRange(tryMoveFromDiscard(solitareState));
            s.AddRange(tryMoveToTop(solitareState));
            s.AddRange(tryPopDeck(solitareState));
            s.AddRange(tryMovePiles(solitareState));

            return s;
        }

        private static List<SolitareState> tryMoveFromDiscard(SolitareState solitareState)
        {

            List<SolitareState> states = new List<SolitareState>();
            if (solitareState.DeckDiscard.Count == 0) return states;
            var topCard = solitareState.DeckDiscard.Last();
            for (int index = 0; index < 7; index++)
            {
                var pile = solitareState.Piles[index];
                if (cardFits(pile.LastOrDefault(), topCard))
                {
                    var s = new SolitareState(solitareState);
                    var top = s.DeckDiscard[s.DeckDiscard.Count - 1];
                    s.DeckDiscard.Remove(top);
                    s.Piles[index].Add(top);
                    states.Add(s);
                }
            }
            return states;
        }

        private static bool cardFits(Card top, Card bottom)
        {
            if (top == null)
            {
                return bottom.Number == 13;
            }
            return top.Number == bottom.Number + 1 && top.Color != bottom.Color;
        }
        private static bool cardFitsTop(Card top, CardType type, Card bottom)
        {
            if (top == null)
            {
                return bottom.Number == 1 && type == bottom.Type;
            }
            return top.Number == bottom.Number - 1 && type == bottom.Type;
        }

        private static List<SolitareState> tryMoveToTop(SolitareState solitareState)
        {
            List<SolitareState> states = new List<SolitareState>();
            Card l;
            for (int index = 0; index < 7; index++)
            {
                var pile = solitareState.Piles[index];
                if (pile.Count == 0)
                {
                    continue;
                }
                l = pile.Last();
                switch (l.Type)
                {
                    case CardType.Spade:
                        if (cardFitsTop(solitareState.TopSpades.LastOrDefault(), CardType.Spade, l))
                        {
                            var s = new SolitareState(solitareState);
                            var p = s.Piles[index][pile.Count - 1];
                            s.Piles[index].Remove(p);
                            s.Piles[index].LastOrDefault()?.SetFace(CardFace.Up);
                            s.TopSpades.Add(p);
                            states.Add(s);
                        }
                        break;
                    case CardType.Club:
                        if (cardFitsTop(solitareState.TopClubs.LastOrDefault(), CardType.Club, l))
                        {
                            var s = new SolitareState(solitareState);
                            var p = s.Piles[index][pile.Count - 1];
                            s.Piles[index].Remove(p);
                            s.Piles[index].LastOrDefault()?.SetFace(CardFace.Up);
                            s.TopClubs.Add(p);
                            states.Add(s);
                        }
                        break;
                    case CardType.Heart:
                        if (cardFitsTop(solitareState.TopHearts.LastOrDefault(), CardType.Heart, l))
                        {
                            var s = new SolitareState(solitareState);
                            var p = s.Piles[index][pile.Count - 1];
                            s.Piles[index].Remove(p);
                            s.Piles[index].LastOrDefault()?.SetFace(CardFace.Up);
                            s.TopHearts.Add(p);
                            states.Add(s);
                        }
                        break;
                    case CardType.Diamond:
                        if (cardFitsTop(solitareState.TopDiamonds.LastOrDefault(), CardType.Diamond, l))
                        {
                            var s = new SolitareState(solitareState);
                            var p = s.Piles[index][pile.Count - 1];
                            s.Piles[index].Remove(p);
                            s.Piles[index].LastOrDefault()?.SetFace(CardFace.Up);
                            s.TopDiamonds.Add(p);
                            states.Add(s);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (solitareState.DeckDiscard.Count > 0)
            {
                l = solitareState.DeckDiscard.Last();
                switch (l.Type)
                {
                    case CardType.Spade:
                        if (cardFitsTop(solitareState.TopSpades.LastOrDefault(), CardType.Spade, l))
                        {
                            var s = new SolitareState(solitareState);
                            var p = s.DeckDiscard[s.DeckDiscard.Count - 1];
                            s.DeckDiscard.Remove(p);
                            s.TopSpades.Add(p);
                            states.Add(s);
                        }
                        break;
                    case CardType.Club:
                        if (cardFitsTop(solitareState.TopClubs.LastOrDefault(), CardType.Club, l))
                        {
                            var s = new SolitareState(solitareState);
                            var p = s.DeckDiscard[s.DeckDiscard.Count - 1];
                            s.DeckDiscard.Remove(p);
                            s.TopClubs.Add(p);
                            states.Add(s);
                        }
                        break;
                    case CardType.Heart:
                        if (cardFitsTop(solitareState.TopHearts.LastOrDefault(), CardType.Heart, l))
                        {
                            var s = new SolitareState(solitareState);
                            var p = s.DeckDiscard[s.DeckDiscard.Count - 1];
                            s.DeckDiscard.Remove(p);
                            s.TopHearts.Add(p);
                            states.Add(s);
                        }
                        break;
                    case CardType.Diamond:
                        if (cardFitsTop(solitareState.TopDiamonds.LastOrDefault(), CardType.Diamond, l))
                        {
                            var s = new SolitareState(solitareState);
                            var p = s.DeckDiscard[s.DeckDiscard.Count - 1];
                            s.DeckDiscard.Remove(p);
                            s.TopDiamonds.Add(p);
                            states.Add(s);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return states;
        }

        private static List<SolitareState> tryPopDeck(SolitareState solitareState)
        {
            List<SolitareState> states = new List<SolitareState>();
            if (solitareState.Deck.Count > 0)
            {
                var s = new SolitareState(solitareState);
                var l = s.Deck.Last();
                l.SetFace(CardFace.Up);
                s.Deck.Remove(l);
                s.DeckDiscard.Add(l);
                states.Add(s);
            }
            else
            {
                var s = new SolitareState(solitareState);
                s.ResetCount++;
                for (int index = 0, c = s.DeckDiscard.Count; index < c; index++)
                {
                    var card = s.DeckDiscard[index];
                    card.SetFace(CardFace.Down);
                }
                s.Deck.AddRange(s.DeckDiscard);
                s.DeckDiscard.Clear();
                states.Add(s);
            }
            return states;
        }

        private static List<SolitareState> tryMovePiles(SolitareState solitareState)
        {
            List<SolitareState> states = new List<SolitareState>();

            for (int index = 0; index < 7; index++)
            {
                var pile = solitareState.Piles[index];
                for (int i = 1; i < pile.Count; i++)
                {
                    var card = pile[i];
                    if (card.Face == CardFace.Up)
                    {
                        for (int pindex = 0; pindex < 7; pindex++)
                        {
                            if (pindex == index) continue;

                            var innerPile = solitareState.Piles[pindex];
                            if (cardFits(innerPile.LastOrDefault(), card))
                            {
                                var s = new SolitareState(solitareState);
                                for (int j = i; j < s.Piles[index].Count; j++)
                                {
                                    s.Piles[pindex].Add(s.Piles[index][j]);
                                }

                                for (int j = s.Piles[index].Count - 1; j >= i; j--)
                                {
                                    s.Piles[index].RemoveAt(j);
                                }

                                s.Piles[index].LastOrDefault()?.SetFace(CardFace.Up);

                                states.Add(s);
                            }
                        }


                        break;
                    }
                }
            }
            return states;
        }

        private static SolitareState setupInitial()
        {
            var initialState = new SolitareState();
            initialState.Deck = NewDeck();

            for (int a = 0; a < 7; a++)
            {
                for (int i = a; i < 7; i++)
                {
                    var index = initialState.Deck.Count - 1;
                    var c = initialState.Deck[index];
                    if (i == a)
                    {
                        c.Face = CardFace.Up;
                    }
                    initialState.Piles[i].Add(c);
                    initialState.Deck.RemoveAt(index);
                }
            }
            return initialState;
        }

        private static List<Card> NewDeck()
        {
            List<Card> cards = new List<Card>();
            for (int i = 0; i < 4; i++)
            {
                for (int c = 1; c <= 13; c++)
                {
                    cards.Add(new Card(CardFace.Down, (CardType)i, c));
                }
            }

            //            cards = cards.OrderBy(a => random.Next()).ToList();
            return cards;
        }
    }

    public class SolitareState
    {
        public SolitareState()
        {
            TopHearts = new List<Card>();
            TopDiamonds = new List<Card>();
            TopClubs = new List<Card>();
            TopSpades = new List<Card>();

            Deck = new List<Card>();
            DeckDiscard = new List<Card>();
            Piles = new List<Card>[7];

            for (int i = 0; i < 7; i++)
            {
                Piles[i] = new List<Card>();
            }
        }

        private int _score = -1;
        public int GetScore()
        {
            if (_score > -1)
            {
                return _score;
            }
            var c = TopHearts.Count +
                    TopSpades.Count +
                    TopClubs.Count +
                    TopDiamonds.Count;

            if (c == 52)
            {
                return int.MaxValue;
            }
            _score = (c * 2) +
                        (24 - (Deck.Count + DeckDiscard.Count)) +
                        (21 - Piles.Sum(a => a.Count(b => b.Face == CardFace.Down)));
            return _score;
        }

        public SolitareState(SolitareState state)
        {
            TopHearts = new List<Card>(state.TopHearts.Select(a => new Card(a.Face, a.Type, a.Number)));
            TopDiamonds = new List<Card>(state.TopDiamonds.Select(a => new Card(a.Face, a.Type, a.Number)));
            TopClubs = new List<Card>(state.TopClubs.Select(a => new Card(a.Face, a.Type, a.Number)));
            TopSpades = new List<Card>(state.TopSpades.Select(a => new Card(a.Face, a.Type, a.Number)));

            Deck = new List<Card>(state.Deck.Select(a => new Card(a.Face, a.Type, a.Number)));
            DeckDiscard = new List<Card>(state.DeckDiscard.Select(a => new Card(a.Face, a.Type, a.Number)));
            Piles = new List<Card>[7];
            for (int i = 0; i < 7; i++)
            {
                Piles[i] = new List<Card>(state.Piles[i].Select(a => new Card(a.Face, a.Type, a.Number)));
            }

            ResetCount = state.ResetCount;
        }

        public List<Card> TopHearts { get; set; }
        public List<Card> TopDiamonds { get; set; }
        public List<Card> TopClubs { get; set; }
        public List<Card> TopSpades { get; set; }
        public List<Card> Deck { get; set; }
        public List<Card> DeckDiscard { get; set; }
        public List<Card>[] Piles { get; set; }
        public int ResetCount { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Top Hearts: " + DumpList(TopHearts));
            sb.AppendLine("Top Diamonds: " + DumpList(TopDiamonds));
            sb.AppendLine("Top Spades: " + DumpList(TopSpades));
            sb.AppendLine("Top Clubs: " + DumpList(TopClubs));
            sb.AppendLine();
            sb.AppendLine("Deck: " + DumpList(Deck));
            sb.AppendLine("DeckDiscard: " + DumpList(DeckDiscard));
            sb.AppendLine("Piles: ");
            foreach (var pile in Piles)
            {
                sb.AppendLine("\tPile: " + DumpList(pile));
            }

            return sb.ToString();
        }

        public static int ComputeHash(byte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                var count = data.Length;
                for (int i = 0; i < count; i++)
                    hash = (hash ^ data[i]) * p;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }

        public override int GetHashCode()
        {

            byte[] bytes = new byte[65];
            int bInd = 0;
            DumpFastList(TopHearts, bytes, ref bInd);
            DumpFastList(TopDiamonds, bytes, ref bInd);
            DumpFastList(TopSpades, bytes, ref bInd);
            DumpFastList(TopClubs, bytes, ref bInd);
            DumpFastList(Deck, bytes, ref bInd);
            DumpFastList(DeckDiscard, bytes, ref bInd);
            for (int index = 0; index < 7; index++)
            {
                DumpFastList(Piles[index], bytes, ref bInd);
            }

            return ComputeHash(bytes);
        }
        static Dictionary<int, char> c = new Dictionary<int, char>()
        {
        };

        static SolitareState()
        {
            var ind = 0;
            for (int i = 0; i < 10; i++)
            {
                c[ind] = i.ToString()[0];
                ind++;
            }
            for (int i = 0; i < 26; i++)
            {
                c[ind] = (char)(((int)'a') + i);
                ind++;
            }
            for (int i = 0; i < 26; i++)
            {
                c[ind] = (char)(((int)'A') + i);
                ind++;
            }
            for (int i = 0; i < 10; i++)
            {
                c[ind] = (char)(((int)'!') + i);
                ind++;
            }
        }
        private string DumpList(List<Card> cards)
        {
            if (cards.Count == 0) return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (var card in cards)
            {
                if (card.Face == CardFace.Down)
                {
                    sb.Append("Down, ");
                }
                else
                {
                    sb.Append(card.Number + " " + card.Type.ToString() + ", ");
                }
            }
            var s = sb.ToString();
            return s.Substring(0, s.Length - 2);
        }

        private void DumpFastList(List<Card> cards, byte[] bytes, ref int bInd)
        {
            if (cards.Count == 0)
            {
                bytes[bInd++] = (255);
                return;
            }
            var count = cards.Count;
            for (int index = 0; index < count; index++)
            {
                var card = cards[index];
                if (card.Face == CardFace.Down)
                {
                    bytes[bInd++] = (0);
                }
                else
                {
                    byte item = (byte)(card.Number + ((int)card.Type * 13));
                    bytes[bInd++] = (item);
                }
            }
            bytes[bInd++] = (255);
        }
    }

    public class Card
    {
        public Card(CardFace face, CardType cardType, int number)
        {
            Face = face;
            Color = (cardType == CardType.Club || cardType == CardType.Spade) ? CardColor.Black : CardColor.Red;
            Number = number;
            Type = cardType;
        }

        public CardFace Face { get; set; }
        public CardColor Color { get; set; }
        public int Number { get; set; }
        public CardType Type { get; set; }

        public void SetFace(CardFace cardFace)
        {
            Face = cardFace;
        }

        public override string ToString()
        {
            return $"Face: {Face},  Number: {Number}, Type: {Type}";
        }
    }

    public enum CardColor
    {
        Red,
        Black
    }

    public enum CardFace
    {
        Up,
        Down
    }

    public enum CardType
    {
        Spade,
        Club,
        Heart,
        Diamond
    }
}
