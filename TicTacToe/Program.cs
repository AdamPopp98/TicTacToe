using System;
using System.Threading;
using TicTacToeClasses;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            //Main Variables
            GameState currentGame;
            Player[] players = new Player[2];
            Player p1 = new AI('X');
            Player p2 = new AI('O');

            //Program Loop;
            int GameNumber = 0;
            while (GameNumber < 20)
            {
                //Game Setup
                currentGame = new GameState(GameController.SelectPlayers(p1, p2));
                GameBoard board = new GameBoard();
                GameController.CreateBoard(board.m_Nodes);

                //Gameplay loop
                bool GameOver = false;
                while (!GameOver)
                {
                    int lastMove;
                    if (currentGame.m_TurnNumber == 4 && currentGame.m_CurrentPlayer.GetType() == typeof(AI))
                    {
                        
                    }
                    lastMove = GameController.GetPlayerMove(currentGame, board);
                    GameController.UpdateBoard(lastMove, board.m_Nodes);
                    GameOver = GameController.CheckGameOver(currentGame.m_TurnNumber, board);
                    Thread.Sleep(1000);
                }
                Console.Clear();
                GameNumber++;
            }
        }
    }
}
