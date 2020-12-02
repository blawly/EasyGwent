using System.Collections.Generic;

namespace EasyGwent
{
    class Player
    {
        public Player(string name, List<Card> playerDeck)
        {
            Name = name;
            Lives = 2;
            Score = 0;
            Deck = playerDeck.GetRange(5, 10);
            Hand = playerDeck.GetRange(0, 5);
            Played = new List<FieldCard>();
        }
        public string Name { get; }
        public int Lives { get; set; }
        public int Score { get; set; }
        public List<Card> Deck { get; set; }
        public List<Card> Hand { get; set; }
        public List<FieldCard> Played { get; set; }
        public void Draw()
        {
            while (Hand.Count < 5)
            {
                Hand.Add(Deck[0]);
                Deck.RemoveAt(0);
            }
        }
        public void UpdateScore()
        {
            Score = 0;
            for (int i = 0; i < Played.Count; i++)
            {
                Score += Played[i].Value;
            }
        }
    }
}
