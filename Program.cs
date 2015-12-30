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
                            var ll = s.Piles[index].LastOrDefault();
                            if (ll != null)
                            {
                                s.Piles[index][s.Piles[index].Count - 1] = ll.GetFaced(CardFace.Up);
                            }
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
                            var ll = s.Piles[index].LastOrDefault();
                            if (ll != null)
                            {
                                s.Piles[index][s.Piles[index].Count - 1] = ll.GetFaced(CardFace.Up);
                            }
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
                            var ll = s.Piles[index].LastOrDefault();
                            if (ll != null)
                            {
                                s.Piles[index][s.Piles[index].Count - 1] = ll.GetFaced(CardFace.Up);
                            }
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
                            var ll = s.Piles[index].LastOrDefault();
                            if (ll != null)
                            {
                                s.Piles[index][s.Piles[index].Count - 1] = ll.GetFaced(CardFace.Up);
                            }
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

                s.Deck.Remove(l);
                s.DeckDiscard.Add(l.GetFaced(CardFace.Up));
                states.Add(s);
            }
            else
            {
                var s = new SolitareState(solitareState);
                s.ResetCount++;
                for (int index = 0, c = s.DeckDiscard.Count; index < c; index++)
                {
                    s.DeckDiscard[index] = s.DeckDiscard[index].GetFaced(CardFace.Down);
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

                for (int i = 1, count = pile.Count; i < count; i++)
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
                                
                                for (int j = i, c = s.Piles[index].Count; j < c; j++)
                                {
                                    s.Piles[pindex].Add(s.Piles[index][j]);
                                }

                                for (int j = s.Piles[index].Count - 1; j >= i; j--)
                                {
                                    s.Piles[index].RemoveAt(j);
                                }


                                var ll = s.Piles[index].LastOrDefault();
                                if (ll != null)
                                {
                                    s.Piles[index][s.Piles[index].Count - 1] = ll.GetFaced(CardFace.Up);
                                }

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
                    cards.Add(Card.Find(CardFace.Down, (CardType)i, c));
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
            TopHearts = new List<Card>(Copy(state.TopHearts));
            TopDiamonds = new List<Card>(Copy(state.TopDiamonds));
            TopClubs = new List<Card>(Copy(state.TopClubs));
            TopSpades = new List<Card>(Copy(state.TopSpades));

            Deck = new List<Card>(Copy(state.Deck));
            DeckDiscard = new List<Card>(Copy(state.DeckDiscard));
            Piles = new List<Card>[7];

            Piles[0] = new List<Card>(Copy(state.Piles[0]));
            Piles[1] = new List<Card>(Copy(state.Piles[1]));
            Piles[2] = new List<Card>(Copy(state.Piles[2]));
            Piles[3] = new List<Card>(Copy(state.Piles[3]));
            Piles[4] = new List<Card>(Copy(state.Piles[4]));
            Piles[5] = new List<Card>(Copy(state.Piles[5]));
            Piles[6] = new List<Card>(Copy(state.Piles[6]));

            ResetCount = state.ResetCount;
        }

        public static List<Card> Copy(List<Card> c)
        {
            List<Card> cc = new List<Card>(c.Count);
            for (int i = 0, count = c.Count; i < count; i++)
            {
                cc.Add(Card.Find(c[i].RealValue));
            }
            return cc;
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

            sb.AppendLine("Top H: " + DumpList(TopHearts));
            sb.AppendLine("Top D: " + DumpList(TopDiamonds));
            sb.AppendLine("Top S: " + DumpList(TopSpades));
            sb.AppendLine("Top C: " + DumpList(TopClubs));
            sb.AppendLine();
            sb.AppendLine("Deck: " + DumpList(Deck));
            sb.AppendLine("Discard: " + DumpList(DeckDiscard));
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


        private string DumpList(List<Card> cards)
        {
            if (cards.Count == 0) return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (var card in cards)
            {
                if (card.Face == CardFace.Down)
                {
                    sb.Append("__ ");
                }
                else
                {
                    sb.Append(NumToK(card.Number) + "" + card.Type.ToString()[0] + " ");
                }
            }
            return sb.ToString();
        }

        public string NumToK(int number)
        {
            switch (number)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    return number.ToString();
                case 11: return "J";
                case 12: return "Q";
                case 13: return "K";
            }
            return "";
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
                bytes[bInd++] = cards[index].Value;
            }
            bytes[bInd++] = (255);
        }
    }

    public class Card
    {
        public byte Value;
        public byte RealValue;
        static Card()
        {
            cards = new Card[4 * 13 * 2 + 1];
            for (int i = 0; i < 4; i++)
            {
                for (int c = 1; c <= 13; c++)
                {
                    var card = new Card(CardFace.Up, (CardType)i, c);
                    cards[card.RealValue] = card;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int c = 1; c <= 13; c++)
                {
                    var card = new Card(CardFace.Down, (CardType)i, c);
                    cards[card.RealValue] = card;
                }
            }
        }

        private static Card[] cards;

        public static Card Find(CardFace face, CardType cardType, int number)
        {
            return cards[(byte)((number + ((int)cardType * 13)) + (face == CardFace.Down ? 52 : 0))];
        }
        public static Card Find(byte val)
        {
            return cards[val];
        }

        public Card(CardFace face, CardType cardType, int number)
        {
            Face = face;
            Color = (cardType == CardType.Club || cardType == CardType.Spade) ? CardColor.Black : CardColor.Red;
            Number = number;
            Type = cardType;
            Value = (byte)(Number + ((int)Type * 13));
            if (face == CardFace.Down)
            {
                RealValue = (byte)(Value + 52);
                Value = 0;
            }
            else
            {
                RealValue = Value;
            }
        }

        public CardFace Face { get; set; }
        public CardColor Color { get; set; }
        public int Number { get; set; }
        public CardType Type { get; set; }

        public override string ToString()
        {
            return $"Face: {Face},  Number: {Number}, Type: {Type}";
        }

        public Card GetFaced(CardFace face)
        {
            return Find(face, Type, Number);
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
