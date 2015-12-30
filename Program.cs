using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolitaireAStar
{
    class Program
    {
        static Random random = new Random(4894);

        static void Main(string[] args)
        {
            var initialState = setupInitial();
            var start = DateTime.Now;

            HashSet<int> closedSet = new HashSet<int>();

            SortedList<int, SolitaireState> newStates = new SortedList<int, SolitaireState>(new DuplicateKeyComparer<int>());

            newStates.Add(initialState.GetScore(), initialState);
            int collision = 0;
            int iterations = 0;
            List<SolitaireState> cachedStates = new List<SolitaireState>(50);

            while (newStates.Count > 0)
            {
                var SolitaireState = newStates.Last().Value;

                if (SolitaireState.GetScore() == int.MaxValue)
                {
                    Console.WriteLine("WON " + (DateTime.Now - start));
                    Console.WriteLine("Iterations per second:" + (iterations / (DateTime.Now - start).TotalMilliseconds) * 1000);
                    Console.WriteLine(newStates.Count + " " + closedSet.Count + " " + collision);

                    rebuild(SolitaireState);
                    Console.ReadLine();
                    return;
                }


                iterations++;
                if (iterations % 50000 == 0)
                {
                    Console.WriteLine(newStates.Count + " " + iterations + " " + closedSet.Count + " " + collision);

                    Console.WriteLine("Iterations per second:" + (iterations / (DateTime.Now - start).TotalMilliseconds) * 1000);
                    //                    Console.WriteLine(SolitaireState.ToString());
                }
                newStates.RemoveAt(newStates.Count - 1);

                cachedStates.Clear();
                oneTick(SolitaireState, cachedStates);
                var count = cachedStates.Count;

                for (int index = 0; index < count; index++)
                {
                    var state = cachedStates[index];
                    var item = state.GetHashCode();
                    if (!closedSet.Contains(item))
                    {
                        closedSet.Add(item);
                        newStates.Add(state.GetScore(), state);
                    }
                    else
                    {
                        collision++;
                    }
                }
            }
            Console.WriteLine("LOST " + (DateTime.Now - start));
            Console.WriteLine("Iterations per second:" + (iterations / (DateTime.Now - start).TotalMilliseconds) * 1000);
            Console.WriteLine(newStates.Count + " " + iterations + " " + closedSet.Count + " " + collision);
            Console.ReadLine();
        }

        private static void rebuild(SolitaireState SolitaireState)
        {
            List<SolitaireState> states = new List<SolitaireState>();
            states.Add(SolitaireState);
            while (SolitaireState.copyState != null)
            {
                states.Insert(0, SolitaireState.copyState);
                SolitaireState = SolitaireState.copyState;
            }
            foreach (var state in states)
            {
                Console.WriteLine(state);
                Console.ReadLine();
            }
        }

        private static void oneTick(SolitaireState SolitaireState, List<SolitaireState> cache)
        {
            tryMoveFromDiscard(SolitaireState, cache);
            tryMoveToTop(SolitaireState, cache);
            tryPopDeck(SolitaireState, cache);
            tryMovePiles(SolitaireState, cache);


        }

        private static void tryMoveFromDiscard(SolitaireState SolitaireState, List<SolitaireState> cache)
        {

            if (SolitaireState.DeckDiscard.Length == 0) return;
            var topCard = SolitaireState.DeckDiscard.FastLast();
            for (int index = 0; index < 7; index++)
            {
                var pile = SolitaireState.Piles[index];
                if (cardFits(pile.FastLastOrDefault(), topCard))
                {
                    var s = new SolitaireState(SolitaireState);

                    s.CopyDiscard();
                    s.CopyPile(index);

                    var top = Utilities.RemoveLast(ref s.DeckDiscard);
                    Utilities.Add(ref s.Piles[index], top);
                    cache.Add(s);
                }
            }
        }

        private static bool cardFits(Card top, Card bottom)
        {
            if (top == null)
            {
                return bottom.Number == 13;
            }
            return top.Color != bottom.Color && top.Number == bottom.Number + 1;
        }
        private static bool cardFitsTop(Card top, int type, Card bottom)
        {
            if (top == null)
            {
                return bottom.Number == 1 && type == bottom.Type;
            }
            return top.Number == bottom.Number - 1 && type == bottom.Type;
        }

        private static void tryMoveToTop(SolitaireState SolitaireState, List<SolitaireState> cache)
        {
            Card l;
            for (int index = 0; index < 7; index++)
            {
                var pile = SolitaireState.Piles[index];
                if (pile.Length == 0)
                {
                    continue;
                }
                l = pile.FastLast();

                var type = l.Type;
                if (cardFitsTop(SolitaireState.Top[type].FastLastOrDefault(), type, l))
                {
                    var s = new SolitaireState(SolitaireState);
                    s.CopyPile(index);
                    s.CopyTop(type);

                    var p = Utilities.RemoveLast(ref s.Piles[index]);
                    var items = s.Piles[index];
                    if (items.Length > 0)
                    {
                        var ll = items.FastLast();
                        items[items.Length - 1] = ll.GetFaced(CardFace.Up);
                    }
                    Utilities.Add(ref s.Top[type], p);
                    cache.Add(s);
                }
             }
            if (SolitaireState.DeckDiscard.Length > 0)
            {
                l = SolitaireState.DeckDiscard.FastLast();
                var type = l.Type;
                if (cardFitsTop(SolitaireState.Top[type].FastLastOrDefault(), type, l))
                {
                    var s = new SolitaireState(SolitaireState);
                    s.CopyDiscard();
                    s.CopyTop(type);

                    var p = Utilities.RemoveLast(ref s.DeckDiscard);
                    Utilities.Add(ref s.Top[type], p);
                    cache.Add(s);
                }
             }

        }

        private static void tryPopDeck(SolitaireState SolitaireState, List<SolitaireState> cache)
        {
            if (SolitaireState.Deck.Length > 0)
            {
                var s = new SolitaireState(SolitaireState);
                s.CopyDiscard();
                s.CopyDeck();
                var l = Utilities.RemoveLast(ref s.Deck);
                Utilities.Add(ref s.DeckDiscard, l.GetFaced(CardFace.Up));
                cache.Add(s);
            }
            else
            {
                var s = new SolitaireState(SolitaireState);
                s.CopyDiscard();
                s.CopyDeck();

                s.ResetCount++;
                for (int index = 0, c = s.DeckDiscard.Length; index < c; index++)
                {
                    s.DeckDiscard[index] = s.DeckDiscard[index].GetFaced(CardFace.Down);
                }
                s.Deck = s.DeckDiscard;
                s.DeckDiscard = new Card[0];
                cache.Add(s);
            }
        }

        private static void tryMovePiles(SolitaireState SolitaireState, List<SolitaireState> cache)
        {

            for (int index = 0; index < 7; index++)
            {
                var pile = SolitaireState.Piles[index];

                for (int i = 1, count = pile.Length; i < count; i++)
                {
                    var card = pile[i];
                    if (card.Face == CardFace.Up)
                    {
                        for (int pindex = 0; pindex < 7; pindex++)
                        {
                            if (pindex == index) continue;

                            var innerPile = SolitaireState.Piles[pindex];
                            var f = innerPile.FastLastOrDefault();
                            if (cardFits(f, card))
                            {
                                var s = new SolitaireState(SolitaireState);
                                s.CopyPile(index);
                                s.CopyPile(pindex);

                                var cards = s.Piles[index];

                                var l = s.Piles[pindex].Length;
                                var newSize = cards.Length - i;
                                Array.Resize(ref s.Piles[pindex], l + newSize);
                                Array.ConstrainedCopy(cards, i, s.Piles[pindex], l, newSize);
                                Utilities.Resize(ref s.Piles[index], i);
                                cards = s.Piles[index];

                                if (cards.Length > 0)
                                {
                                    var ll = cards.FastLast();
                                    cards[cards.Length - 1] = ll.GetFaced(CardFace.Up);
                                }

                                cache.Add(s);
                            }
                        }


                        break;
                    }
                }
            }
        }

        private static SolitaireState setupInitial()
        {
            var initialState = new SolitaireState();
            initialState.Deck = NewDeck();
            var deck = new List<Card>(NewDeck());
            for (int a = 0; a < 7; a++)
            {
                for (int i = a; i < 7; i++)
                {
                    var index = deck.Count - 1;
                    var c = deck[index];
                    if (i == a)
                    {
                        c = c.GetFaced(CardFace.Up);
                        deck[index] = c;
                    }
                    Utilities.Add(ref initialState.Piles[i], c);
                    deck.RemoveAt(index);
                }
            }
            initialState.Deck = deck.ToArray();
            return initialState;
        }

        private static Card[] NewDeck()
        {
            List<Card> cards = new List<Card>();
            for (int i = 0; i < 4; i++)
            {
                for (int c = 1; c <= 13; c++)
                {
                    cards.Add(Card.Find(CardFace.Down, i, c));
                }
            }
            //            cards = cards.OrderBy(a => random.Next()).ToList();
            return cards.ToArray();
        }
    }

    public class SolitaireState
    {
        public int[] PileFaceDown;
        public SolitaireState()
        {
            Top=new Card[4][];
            Top[0]=new Card[0];
            Top[1]=new Card[0];
            Top[2]=new Card[0];
            Top[3]=new Card[0];

            Deck = new Card[0];
            DeckDiscard = new Card[0];
            Piles = new Card[7][];
            PileFaceDown = new int[7];

            for (int i = 0; i < 7; i++)
            {
                Piles[i] = new Card[0];
            }

        }

        private int _score = -1;
        public int GetScore()
        {
            if (_score > -1)
            {
                return _score;
            }
            var c = Top[0].Length +
                    Top[1].Length +
                    Top[2].Length +
                    Top[3].Length;

            if (c == 52)
            {
                return int.MaxValue;
            }
            _score = (c * 6) +
                        (24 - (Deck.Length + DeckDiscard.Length)) * 2 +
                        (21 - Piles.FastSum()) * 4;
            return _score;
        }


        public SolitaireState(SolitaireState state)
        {
            Piles = new Card[7][];
            PileFaceDown = new int[7];
            Top=new Card[4][];

            Top[(int)CardType.Heart]= state.Top[(int)CardType.Heart];
            Top[(int)CardType.Diamond]= state.Top[(int)CardType.Diamond];
            Top[(int)CardType.Club]= state.Top[(int)CardType.Club];
            Top[(int)CardType.Spade]= state.Top[(int)CardType.Spade];

            Deck = state.Deck;
            DeckDiscard = state.DeckDiscard;

            Piles[0] = state.Piles[0];
            Piles[1] = state.Piles[1];
            Piles[2] = state.Piles[2];
            Piles[3] = state.Piles[3];
            Piles[4] = state.Piles[4];
            Piles[5] = state.Piles[5];
            Piles[6] = state.Piles[6];


            copyState = state;
            ResetCount = state.ResetCount;
        }

        public static Card[] Copy(Card[] c)
        {
            Card[] cc = new Card[c.Length];
            Array.Copy(c, cc, c.Length);
            return cc;
        }

        public SolitaireState copyState;

        public Card[][] Top;
        public Card[] Deck;
        public Card[] DeckDiscard;
        public Card[][] Piles;
        public int ResetCount;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Top H: " + DumpList(Top[CardType.Heart]));
            sb.AppendLine("Top D: " + DumpList(Top[CardType.Diamond]));
            sb.AppendLine("Top S: " + DumpList(Top[CardType.Spade]));
            sb.AppendLine("Top C: " + DumpList(Top[CardType.Club]));
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
        static byte[] bytes = new byte[65];

        public unsafe override int GetHashCode()
        {
            fixed (byte* bt = &bytes[0])
            {
                var bts = bt;


                DumpFastList(Top[CardType.Heart], ref bts);
                DumpFastList(Top[CardType.Diamond], ref bts);
                DumpFastList(Top[CardType.Spade], ref bts);
                DumpFastList(Top[CardType.Club], ref bts);
                DumpFastList(Deck, ref bts);
                DumpFastList(DeckDiscard, ref bts);

                DumpFastList(Piles[0], ref bts);
                DumpFastList(Piles[1], ref bts);
                DumpFastList(Piles[2], ref bts);
                DumpFastList(Piles[3], ref bts);
                DumpFastList(Piles[4], ref bts);
                DumpFastList(Piles[5], ref bts);
                DumpFastList(Piles[6], ref bts);
            }

            return ComputeHash(bytes);
        }


        private string DumpList(Card[] cards)
        {
            if (cards.Length == 0) return string.Empty;

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

        private unsafe void DumpFastList(Card[] cards, ref byte* bytes)
        {
            if (cards.Length == 0)
            {
                *(bytes++) = (255);
                return;
            }
            var count = cards.Length;
            for (int index = 0; index < count; index++)
            {
                var value = cards[index].Value;
                *(bytes++) = value;
            }
            *(bytes++) = (255);
        }

        public void CopyDiscard()
        {
            DeckDiscard = (Copy(copyState.DeckDiscard));
        }
        public void CopyPile(int index)
        {
            Piles[index] = (Copy(copyState.Piles[index]));
        }

        public void CopyTop(int type)
        {
            Top[type ]= (Copy(copyState.Top[type]));
        } 

        public void CopyDeck()
        {
            Deck = (Copy(copyState.Deck));
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
                    var card = new Card(CardFace.Up, i, c);
                    cards[card.RealValue] = card;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int c = 1; c <= 13; c++)
                {
                    var card = new Card(CardFace.Down, i, c);
                    cards[card.RealValue] = card;
                }
            }
        }

        public static Card[] cards;

        public static Card Find(CardFace face, int cardType, int number)
        {
            return cards[(byte)((number + (cardType * 13)) + (face == CardFace.Down ? 52 : 0))];
        }


        public Card(CardFace face, int cardType, int number)
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

        public readonly CardFace Face;
        public readonly CardColor Color;
        public readonly int Number;
        public readonly int Type;

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

    public static class CardType
    {
        public const int Spade = 0;
        public const int Club = 1;
        public const int Heart = 2;
        public const int Diamond = 3;
    }

    public static class Utilities
    {
        public static Card FastLastOrDefault(this Card[] items)
        {
            var count = items.Length;
            if (count == 0) return null;
            return items[count - 1];
        }
        public static Card FastLast(this Card[] items)
        {
            var count = items.Length;
            if (count == 0) return null;
            return items[count - 1];
        }
        public static int FastSum(this Card[][] items)
        {
            int cd = 0;
            var count = items.Length;
            for (int index = 0; index < count; index++)
            {
                var items2 = items[index];
                var count2 = items2.Length;
                for (int index2 = 0; index2 < count2; index2++)
                {
                    if (items2[index2].Face == CardFace.Down)
                        cd++;
                }

            }
            return cd;

        }
        public static void Add(ref Card[] items, Card card)
        {
            var newSize = items.Length;
            Array.Resize(ref items, newSize + 1);
            items[newSize] = card;
        }
        public static Card RemoveLast(ref Card[] items)
        {
            var newSize = items.Length - 1;
            var t = items[newSize];
            Array.Resize(ref items, newSize);
            return t;
        }
        public static void Resize(ref Card[] items, int size)
        {
            Array.Resize(ref items, size);
        }
    }
    public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
    {
        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);
            if (result == 0)
                return 1;   // Handle equality as beeing greater
            return result;
        }
    }

}


/*cache lasts
  array of pileLasts[pileIndex]
remove face?? 
  only do it in tostring
  try move piles issue
    a pile should know what its lowest shown card is
*/
