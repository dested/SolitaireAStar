using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SolitaireAStar
{
    class Program
    {
        static Random random = new Random(4894);

        static void Main(string[] args)
        {

            @do();
        }

        private static void @do()
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
                //                                Console.WriteLine(SolitaireState.ToString());
                //                                Console.ReadLine();
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
            var topCard = SolitaireState.DeckDiscardLast;
            for (int index = 0; index < 7; index++)
            {
                if (cardFits(SolitaireState.PilesLast[index], topCard))
                {
                    var s = new SolitaireState(SolitaireState);

                    s.CopyDiscard();
                    s.CopyPile(index);

                    var top = Utilities.RemoveLast(ref s.DeckDiscard);

                    if (s.DeckDiscard.Length == 0)
                    {
                        s.DeckDiscardLast = Utilities.CardDefault;
                    }
                    else
                    {
                        s.DeckDiscardLast = s.DeckDiscard[s.DeckDiscard.Length - 1];
                    }

                    Utilities.Add(ref s.Piles[index], top);
                    s.PilesLast[index] = top;
                    cache.Add(s);
                }
            }
        }
        private static void tryMoveToTop(SolitaireState SolitaireState, List<SolitaireState> cache)
        {
            byte l;
            for (int index = 0; index < 7; index++)
            {
                var pile = SolitaireState.Piles[index];
                if (pile.Length == 0)
                {
                    continue;
                }
                l = SolitaireState.PilesLast[index];

                var type = l / 13;
                if (cardFitsTop(SolitaireState.TopLast[type], type, l))
                {
                    var s = new SolitaireState(SolitaireState);
                    s.CopyPile(index);
                    s.CopyTop(type);

                    var p = Utilities.RemoveLast(ref s.Piles[index]);
                    var items = s.Piles[index];
                    if (items.Length == 0)
                    {
                        s.PilesLast[index] = Utilities.CardDefault;
                    }
                    else
                    {
                        s.PilesLast[index] = items[items.Length - 1];
                    }

                    if (items.Length > 0)
                    {
                        s.PileFaceDown[index] = (byte)(items.Length - 1);
                    }

                    Utilities.Add(ref s.Top[type], p);
                    s.TopLast[type] = p;

                    cache.Add(s);
                }
            }
            if (SolitaireState.DeckDiscard.Length > 0)
            {
                l = SolitaireState.DeckDiscardLast;
                var type = l / 13;
                if (cardFitsTop(SolitaireState.TopLast[type], type, l))
                {
                    var s = new SolitaireState(SolitaireState);
                    s.CopyDiscard();
                    s.CopyTop(type);

                    var p = Utilities.RemoveLast(ref s.DeckDiscard);
                    if (s.DeckDiscard.Length == 0)
                    {
                        s.DeckDiscardLast = Utilities.CardDefault;
                    }
                    else
                    {
                        s.DeckDiscardLast = s.DeckDiscard[s.DeckDiscard.Length - 1];
                    }
                    Utilities.Add(ref s.Top[type], p);
                    s.TopLast[type] = p;

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
                if (s.Deck.Length == 0)
                {
                    s.DeckLast = Utilities.CardDefault;
                }
                else
                {
                    s.DeckLast = s.Deck[s.Deck.Length - 1];
                }
                Utilities.Add(ref s.DeckDiscard, l);
                s.DeckDiscardLast = l;
                cache.Add(s);
            }
            else
            {
                var s = new SolitaireState(SolitaireState);
                s.CopyDiscard();
                s.CopyDeck();
                s.ResetCount++;
                s.Deck = s.DeckDiscard;
                Array.Resize(ref s.DeckDiscard, 0);
                s.DeckLast = s.Deck[s.Deck.Length - 1];
                s.DeckDiscardLast = Utilities.CardDefault;
                cache.Add(s);
            }
        }

        private static void tryMovePiles(SolitaireState SolitaireState, List<SolitaireState> cache)
        {

            for (int index = 0; index < 7; index++)
            {
                var pile = SolitaireState.Piles[index];

                var faceDownIndex = SolitaireState.PileFaceDown[index];
                if (faceDownIndex > 0)
                {
                    var card = pile[faceDownIndex];
                    for (int pindex = 0; pindex < 7; pindex++)
                    {
                        if (pindex == index) continue;

                        if (cardFits(SolitaireState.PilesLast[pindex], card))
                        {
                            var s = new SolitaireState(SolitaireState);
                            s.CopyPile(index);
                            s.CopyPile(pindex);

                            var cards = s.Piles[index];

                            var l = s.Piles[pindex].Length;
                            var newSize = cards.Length - faceDownIndex;
                            Array.Resize(ref s.Piles[pindex], l + newSize);
                            Array.ConstrainedCopy(cards, faceDownIndex, s.Piles[pindex], l, newSize);
                            Utilities.Resize(ref s.Piles[index], faceDownIndex);
                            if (s.Piles[pindex].Length == 0)
                            {
                                s.PilesLast[pindex] = Utilities.CardDefault;
                            }
                            else
                            {
                                s.PilesLast[pindex] = s.Piles[pindex][s.Piles[pindex].Length - 1];
                            }

                            if (s.Piles[index].Length == 0)
                            {
                                s.PilesLast[index] = Utilities.CardDefault;
                            }
                            else
                            {
                                s.PilesLast[index] = s.Piles[index][s.Piles[index].Length - 1];
                            }

                            cards = s.Piles[index];

                            if (cards.Length > 0)
                            {
                                s.PileFaceDown[index] = (byte)(cards.Length - 1);
                            }

                            cache.Add(s);
                        }
                    }
                }
            }
        }
        private static bool cardFits(byte top, byte bottom)
        {
            if (top == 255)
            {
                return bottom % 13 == 0;
            }
            return top / 26 != bottom / 26 && top % 13 == bottom % 13 + 1;
        }
        private static bool cardFitsTop(byte top, int type, byte bottom)
        {
            if (top == 255)
            {
                return bottom % 13 == 0 && type == bottom / 13;
            }
            return top % 13 == bottom % 13 - 1 && type == bottom / 13;
        }



        private static SolitaireState setupInitial()
        {
            var initialState = new SolitaireState();
            initialState.Deck = NewDeck();
            var deck = NewDeck();
            for (byte a = 0; a < 7; a++)
            {
                for (int i = a; i < 7; i++)
                {
                    var index = deck.Length - 1;
                    var c = deck[index];
                    if (i == a)
                    {
                        initialState.PileFaceDown[i] = a;
                    }
                    Utilities.Add(ref initialState.Piles[i], c);
                    Utilities.RemoveLast(ref deck);
                }
            }
            for (int i = 0; i < 7; i++)
            {
                initialState.PilesLast[i] = initialState.Piles[i][initialState.Piles[i].Length - 1];
            }
            for (int i = 0; i < 4; i++)
            {
                initialState.TopLast[i] = Utilities.CardDefault;
            }
            initialState.Deck = deck.ToArray();
            initialState.DeckDiscardLast = Utilities.CardDefault;
            initialState.DeckLast = initialState.Deck[initialState.Deck.Length - 1];

            return initialState;
        }

        private static byte[] NewDeck()
        {
            byte[] cards = new byte[52];
            for (byte i = 0; i < 4; i++)
            {
                for (byte c = 0; c < 13; c++)
                {
                    cards[c + (i * 13)] = (byte)(c + (i * 13));
                }
            }
            //            cards = cards.OrderBy(a => random.Next()).ToList();
            return cards.ToArray();
        }
    }

    public class SolitaireState
    {
        public byte[] PileFaceDown;
        public SolitaireState()
        {


            Top = new byte[4][];
            Top[0] = new byte[0];
            Top[1] = new byte[0];
            Top[2] = new byte[0];
            Top[3] = new byte[0];

            Deck = new byte[0];
            DeckDiscard = new byte[0];
            Piles = new byte[7][];
            PileFaceDown = new byte[7];


            TopLast = new byte[4];
            PilesLast = new byte[7];

            for (int i = 0; i < 7; i++)
            {
                Piles[i] = new byte[0];
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
                        (21 - FastSum()) * 4;
            return _score;
        }

        public int FastSum()
        {
            int cd = 0;
            for (int index = 0; index < 7; index++)
            {
                cd += PileFaceDown[index];
            }
            return cd;

        }


        public SolitaireState(SolitaireState state)
        {
            Piles = new byte[7][];
            PileFaceDown = new byte[7];
            Top = new byte[4][];

            TopLast = new byte[4];
            PilesLast = new byte[7];

            for (int i = 0; i < 4; i++)
            {
                Top[i] = state.Top[i];
                TopLast[i] = state.TopLast[i];
            }

            Deck = state.Deck;
            DeckLast = state.DeckLast;

            DeckDiscard = state.DeckDiscard;
            DeckDiscardLast = state.DeckDiscardLast;

            for (int i = 0; i < 7; i++)
            {
                Piles[i] = state.Piles[i];
                PilesLast[i] = state.PilesLast[i];

                PileFaceDown[i] = state.PileFaceDown[i];
            }



            copyState = state;
            ResetCount = state.ResetCount;
        }

        public static byte[] Copy(byte[] c)
        {
            byte[] cc = new byte[c.Length];
            Array.Copy(c, cc, c.Length);
            return cc;
        }

        public SolitaireState copyState;

        public byte[][] Top;
        public byte[] Deck;
        public byte[] DeckDiscard;
        public byte[][] Piles;

        public byte DeckDiscardLast;
        public byte DeckLast;
        public byte[] TopLast;
        public byte[] PilesLast;


        public int ResetCount;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Top ♠: " + DumpListUp(Top[CardType.Spade]));
            sb.AppendLine("Top ♣: " + DumpListUp(Top[CardType.Club]));
            sb.AppendLine("Top ♥: " + DumpListUp(Top[CardType.Heart]));
            sb.AppendLine("Top ♦: " + DumpListUp(Top[CardType.Diamond]));
            sb.AppendLine();
            sb.AppendLine("Deck: " + DumpListDown(Deck));
            sb.AppendLine("Discard: " + DumpListUp(DeckDiscard));
            sb.AppendLine("Piles: ");
            var length = Piles.Length;
            for (int index = 0; index < length; index++)
            {
                var pile = Piles[index];
                sb.AppendLine("\tPile: " + DumpList(PileFaceDown[index], pile));
            }

            return sb.ToString();
        }

        static byte[] bytes = new byte[72];

        public override int GetHashCode()
        {
            byte pointer = 0;

            DumpFastList(Top[CardType.Spade], bytes, ref pointer);
            DumpFastList(Top[CardType.Club], bytes, ref pointer);
            DumpFastList(Top[CardType.Heart], bytes, ref pointer);
            DumpFastList(Top[CardType.Diamond], bytes, ref pointer);

            DumpFastList(Deck, bytes, ref pointer);
            DumpFastList(DeckDiscard, bytes, ref pointer);

            for (int i = 0; i < 7; i++)
            {
                DumpFastList(Piles[i], bytes, ref pointer);
                bytes[pointer++] = PileFaceDown[i];
            }
            return ComputeHash(bytes);
        }
        public static int ComputeHash(byte[] data)
        {
            unchecked
            {
                int hash = 17;
                for (int index = 0; index < 72; index++)
                {
                    hash = hash * 23 + data[index];
                }

                return hash;
            }
        }


        private string DumpList(int faceDown, byte[] cards)
        {
            if (cards.Length == 0) return string.Empty;

            StringBuilder sb = new StringBuilder();

            var length = cards.Length;
            for (int index = 0; index < length; index++)
            {
                var card = cards[index];
                if (index < faceDown)
                {
                    sb.Append("__ ");
                }
                else
                {
                    sb.Append(NumToK(card % 13) + "" + NumToT(card / 13) + " ");
                }
            }
            return sb.ToString();
        }
        private string DumpListUp(byte[] cards)
        {
            if (cards.Length == 0) return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (var card in cards)
            {
                sb.Append(NumToK(card % 13) + "" + NumToT(card / 13) + " ");
            }
            return sb.ToString();
        }
        private string DumpListDown(byte[] cards)
        {
            if (cards.Length == 0) return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (var card in cards)
            {
                sb.Append("__ ");
            }
            return sb.ToString();
        }

        public string NumToK(int number)
        {
            switch (number)
            {
                case 0: return "A";
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8: return (number + 1).ToString();
                case 9: return "0";
                case 10: return "J";
                case 11: return "Q";
                case 12: return "K";
            }
            return "";
        }
        public string NumToT(int number)
        {
            switch (number)
            {
                case 0: return "♠";
                case 1: return "♣";
                case 2: return "♥";
                case 3: return "♦";
            }
            return "";
        }

        private void DumpFastList(byte[] cards, byte[] bytes, ref byte pointer)
        {
            var length = cards.Length;
            if (length == 0)
            {
                bytes[pointer++] = (255);
                return;
            }
            var count = cards.Length;
            for (int index = 0; index < count; index++)
            {
                bytes[pointer++] = cards[index];
            }

            bytes[pointer++] = (255);
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
            Top[type] = (Copy(copyState.Top[type]));
        }

        public void CopyDeck()
        {
            Deck = (Copy(copyState.Deck));
        }
    }


    public enum CardColor
    {
        Red,
        Black
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
        public static byte CardDefault = 255;

        public static void Add(ref byte[] items, byte card)
        {
            var newSize = items.Length;
            Array.Resize(ref items, newSize + 1);
            items[newSize] = card;
        }
        public static byte RemoveLast(ref byte[] items)
        {
            var newSize = items.Length - 1;
            var t = items[newSize];
            Array.Resize(ref items, newSize);
            return t;
        }
        public static void Resize(ref byte[] items, int size)
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
