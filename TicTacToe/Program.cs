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
            Player p1 = new Human('X');
            Player p2 = new AI('O');

            //Program Loop
            int GameNumber = 0;
            currentGame = new GameState(GameController.SelectPlayers(p1, p2));
            while (GameNumber < 20)
            {
                //Game Setup
                GameBoard board = new GameBoard();
                GameController.CreateBoard();

                //Gameplay loop
                bool GameOver = false;
                while (!GameOver)
                {
                    int lastMove = GameController.GetPlayerMove(currentGame, board);
                    GameController.UpdateBoard(lastMove, board.m_Nodes);
                    GameOver = GameController.CheckGameOver(currentGame.m_TurnNumber, board);
                    Thread.Sleep(1000);
                }
                Console.Clear();
                GameNumber++;
                currentGame.ResetGame();
            }
        }
    }
}
