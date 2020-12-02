using System;

namespace EasyGwent
{
    class EasyGwent
    {
        static void Main()
        {
            bool newGame = true;
            while (newGame)
            {
                Console.WriteLine("Enter name for first player!");
                string player0name = Console.ReadLine();
                Console.WriteLine("Enter name for second player!");
                string player1name = Console.ReadLine();
                Console.Clear();
                Random rnd = new Random();
                Game game = new Game(player0name, player1name, rnd);
                while (!game.Over)
                {
                    game.DisplayTurn();
                    int choice = game.ReadChoice();
                    Console.Clear();
                    game.DisplayChoice(choice);
                    game.PlayChoice(choice);
                    int state = game.Next();
                    game.DisplayNext(state);
                    Console.ReadLine();
                    Console.Clear();
                }
                Console.WriteLine("Play again? (y/n)");
                string input = Console.ReadLine();
                if (input[0] == 'n')
                {
                    newGame = false;
                    Console.WriteLine("Goodbye");
                    Console.ReadLine();
                }
            }            
        }        
    }
}
