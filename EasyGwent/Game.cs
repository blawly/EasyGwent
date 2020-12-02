using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyGwent
{
    class Game
    {
        int round;
        int turn;
        readonly Player[] players = new Player[2];
        public bool Over { get; private set; }
        public Game(string player0Name, string player1Name, Random rnd)
        {
            round = 1;
            turn = 1;
            Over = false;
            List<Card> deck = GenerateDeck(rnd);
            int[] player0Subset = Player0Subset(rnd);
            List<Card> player0Deck = SplitDeck(deck, player0Subset, true);
            List<Card> player1Deck = SplitDeck(deck, player0Subset, false);
            players[0] = new Player(player0Name, player0Deck);
            players[1] = new Player(player1Name, player1Deck);
        }
        public void DisplayTurn()
        {
            ref Player currentPlayer = ref players[(turn + 1) % 2];
            ref Player otherPlayer = ref players[turn % 2];
            Console.WriteLine($"Round {round} : Turn {turn}");
            Console.WriteLine($"{currentPlayer.Name}'s turn");
            Console.WriteLine($"\n{otherPlayer.Name}'s played cards:");
            for (int i = 0; i < otherPlayer.Played.Count; i++)
            {
                Console.WriteLine($"\t{otherPlayer.Played[i]}");
            }
            Console.WriteLine($"\nYour played cards:");
            for (int i = 0; i < currentPlayer.Played.Count; i++)
            {
                Console.WriteLine($"\t{currentPlayer.Played[i]}");
            }
            Console.WriteLine($"\nCards in your hand");
            for (int i = 0; i < currentPlayer.Hand.Count; i++)
            {
                Console.WriteLine($"\t{i + 1}. {currentPlayer.Hand[i]}");
            }
        }
        public int ReadChoice()
        {
            ref Player currentPlayer = ref players[(turn + 1) % 2];
            ref Player otherPlayer = ref players[turn % 2];
            Console.WriteLine("\nChoose which card to play, to skip choose 0!");
            int choice = int.Parse(Console.ReadLine());
            while (choice > currentPlayer.Hand.Count || choice < 0)
            {
                Console.WriteLine($"\nThere is no card #{choice}. Choose another, to skip choose 0!");
                choice = int.Parse(Console.ReadLine());
            }
            while (choice != 0 && !currentPlayer.Hand[choice - 1].Playable(currentPlayer, otherPlayer))
            {
                Console.WriteLine($"\nCard #{choice} cannot be played. Choose another, to skip choose 0!");
                choice = int.Parse(Console.ReadLine());
            }
            return choice;
        }
        public void DisplayChoice(int choice)
        {
            ref Player currentPlayer = ref players[(turn + 1) % 2];
            ref Player otherPlayer = ref players[turn % 2];
            Console.WriteLine($"Round {round} : Turn {turn}");
            if (choice == 0)
            {
                Console.WriteLine($"\n{currentPlayer.Name} skipped the turn.");
            }
            else
            {
                Console.WriteLine($"\n{currentPlayer.Name} played {currentPlayer.Hand[choice - 1]}");
            }
        }
        public void PlayChoice(int choice)
        {
            if (choice != 0)
            {
                ref Player currentPlayer = ref players[(turn + 1) % 2];
                ref Player otherPlayer = ref players[turn % 2];
                currentPlayer.Hand[choice - 1].Play(ref currentPlayer, ref otherPlayer);
            }
        }
        public int Next()
        {
            // returns state-dependent int:
            //   0   round over, player0 loses round, the game continues
            //   1   round over, player1 loses round, the game continues
            //   2   round over, round tied, the game continues
            //   3   round over, player0 loses round, the game is over (third round ended)
            //   4   round over, player1 loses round, the game is over (third round ended)
            //   5   round over, round tied tie, the game is over (third round ended)
            //   6   round over, player0 loses round, the game is over (player0 lost all lives)
            //   7   round over, player1 loses round, the game is over (player1 lost all lives)
            //   8   round continues
            int state;
            if (turn == 10)
            {                
                if (players[0].Score < players[1].Score)
                {
                    players[0].Lives--;
                    state = 0;
                    if (players[0].Lives == 0)
                    {
                        state = 6;
                    }
                }
                else if (players[0].Score > players[1].Score)
                {
                    players[1].Lives--;
                    state = 1;
                    if (players[1].Lives == 0)
                    {
                        state = 7;
                    }
                }
                else
                {
                    state = 2;
                }
                if (state < 3)
                {
                    if (round == 3)
                    {
                        state += 3;
                    }
                    else
                    {
                        players[0].Score = 0;
                        players[1].Score = 0;
                        players[0].Draw();
                        players[1].Draw();
                        round++;
                        turn = 1;
                    }
                }

            }
            else
            {
                turn++;
                state = 8;
            }
            return state;
        }
        public void DisplayNext(int state)
        {
            ref Player currentPlayer = ref players[(turn + 1) % 2];
            if (state % 3 != 2)
            {
                Console.WriteLine($"\nRound {round - 1} over.\n{players[state % 3].Name} lost a life.");
            }
            else if (state != 8)
            {
                Console.WriteLine($"\nRound {round - 1} over.\nRound tied.");
            }
            if (state < 3)
            {
                Console.WriteLine($"\nLives:\n\t{players[0].Name}: {players[0].Lives}\n\t{players[1].Name}: {players[1].Lives}");
                Console.WriteLine($"\nRound {round} : turn {turn} coming up.\n{currentPlayer.Name} ready?");
            }
            else if (state < 6)
            {
                Console.WriteLine("\nGame Over\nLast round ended.");
            }
            else if (state < 8)
            {
                Console.WriteLine($"\nGame Over\n{players[state % 3].Name} lost all lives.");
            }
            else
            {
                Console.WriteLine($"\nCurrent score:\n\t{players[0].Name}: {players[0].Score}\n\t{players[1].Name}: {players[1].Score}");
                Console.WriteLine($"\nRound {round} : turn {turn} coming up.\n{currentPlayer.Name} ready?");
            }
            if (state > 2 && state != 8)
            {
                int winner = GetWinner();
                if (winner == -1)
                {
                    Console.WriteLine("Tie.");
                }
                else
                {
                    Console.WriteLine($"The winner is {players[winner].Name}.");
                }
            }
        }
        List<Card> GenerateDeck(Random rnd)
        {
            List<Card> deck = new List<Card>(30);
            for (int i = 0; i < 30; i++)
            {
                if (i < 5)
                {
                    deck.Add(new WeatherCard(rnd));
                }
                else
                {
                    deck.Add(new FieldCard(rnd));
                }
            }
            return deck;
        }
        int[] Player0Subset(Random rnd)
        {
            int[] subset = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            for (int i = 0; i < 15; i++)
            {
                int tmp;
                do
                {
                    tmp = rnd.Next(0, 30);
                } while (subset.Contains(tmp));
                subset[i] = tmp;
            }
            return subset;
        }
        List<Card> SplitDeck(List<Card> deck, int[] player0Subset, bool player0)
        {
            List<Card> playerDeck = new List<Card>(15);
            for (int i = 0; i < 30; i++)
            {
                if(Match(i, player0Subset, player0))
                {
                    playerDeck.Add(deck[i]);
                }
            }
            return playerDeck;
        }
        bool Match(int idx, int[] player0Subset, bool player0)
        {
            bool match = false;
            if (( player0 &&  player0Subset.Contains(idx))
            ||  (!player0 && !player0Subset.Contains(idx)))
            {
                match = true;
            }
            return match;
        }
        int GetWinner()
        {
            int winner = -1;
            if (players[0].Lives > players[1].Lives)
            {
                winner = 0;
            }
            else if (players[0].Lives < players[1].Lives)
            {
                winner = 1;
            }
            else
            {
                if (players[0].Score > players[1].Score)
                {
                    winner = 0;
                }
                else if (players[0].Score < players[1].Score)
                {
                    winner = 1;
                }
            }
            return winner;
        }
    }
}
