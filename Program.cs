using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitareAStar
{
    class Program
    {
        static void Main(string[] args)
        {
            var initialState = setupInitial();

            List<SolitareState> states = new List<SolitareState>();
            states.Add(initialState);


            while (states.Count > 0)
            {
                List<SolitareState> newStates = new List<SolitareState>();
                foreach (var solitareState in states)
                {
                    Console.WriteLine(solitareState.ToString());

                    newStates.AddRange(oneTick(solitareState));
                }
                states = newStates;
            }

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
            for (int index = 0; index < solitareState.Piles.Count; index++)
            {
                var pile = solitareState.Piles[index];
                if (cardFits(pile.Last(), topCard))
                {
                    var s = new SolitareState(solitareState);
                    s.DeckDiscard.Remove(topCard);
                    s.Piles[index].Add(topCard);
                    states.Add(s);
                }
            }
            return states;
        }

        private static bool cardFits(Card top, Card bottom)
        {
            return top.Number == bottom.Number - 1 && top.Color != bottom.Color;
        }
        private static bool cardFitsTop(Card top, CardType type, Card bottom)
        {
            if (top.Number==0)
            {
                return bottom.Number == 1 && type == bottom.Type;
            }
            return top.Number == bottom.Number + 1 && type == bottom.Type;
        }

        private static List<SolitareState> tryMoveToTop(SolitareState solitareState)
        {
            List<SolitareState> states = new List<SolitareState>();
            for (int index = 0; index < solitareState.Piles.Count; index++)
            {
                var pile = solitareState.Piles[index];
                if (pile.Count == 0)
                {
                    continue;
                }
                var l = pile.Last();
                switch (l.Type)
                {
                    case CardType.Spade:
                        if (cardFitsTop(solitareState.TopSpades.LastOrDefault(), CardType.Spade, l))
                        {
                            var s = new SolitareState(solitareState);
                            var p = s.Piles[index][pile.Count - 1];
                            s.Piles[index].Remove(p);
                            s.Piles[index].Last().SetFace(CardFace.Up);
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
                            s.Piles[index].Last().SetFace(CardFace.Up);
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
                            s.Piles[index].Last().SetFace(CardFace.Up);
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
                            s.Piles[index].Last().SetFace(CardFace.Up);
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
                foreach (var card in s.DeckDiscard)
                {
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
            Piles = new List<List<Card>>();

            for (int i = 0; i < 7; i++)
            {
                Piles.Add(new List<Card>());
            }
        }

        public SolitareState(SolitareState state)
        {
            TopHearts = new List<Card>(state.TopHearts);
            TopDiamonds = new List<Card>(state.TopDiamonds);
            TopClubs = new List<Card>(state.TopClubs);
            TopSpades = new List<Card>(state.TopSpades);

            Deck = new List<Card>(state.Deck);
            DeckDiscard = new List<Card>(state.DeckDiscard);
            Piles = state.Piles.Select(a => new List<Card>(a)).ToList();
            ResetCount = state.ResetCount;
        }

        public List<Card> TopHearts { get; set; }
        public List<Card> TopDiamonds { get; set; }
        public List<Card> TopClubs { get; set; }
        public List<Card> TopSpades { get; set; }
        public List<Card> Deck { get; set; }
        public List<Card> DeckDiscard { get; set; }
        public List<List<Card>> Piles { get; set; }
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
    }

    public struct Card
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
