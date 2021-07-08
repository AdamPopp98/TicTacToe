using System;

namespace TicTacToeClasses
{
    public class Node
    {
        private int m_X;
        private int m_Y;
        private char m_Symbol;

        //Getters
        public int GetX()
        {
            return m_X;
        }
        public int GetY()
        {
            return m_Y;
        }
        public char GetSymbol()
        {
            return m_Symbol;
        }

        //Setters
        public void SetX(int X)
        {
            m_X = X;
        }
        public void SetY(int Y)
        {
            m_Y = Y;
        }
        public void SetSymbol(char symbol)
        {
            m_Symbol = symbol;
        }

        //Base Constructor
        public Node(int X, int Y, char symbol = ' ')
        {
            SetX(X);
            SetY(Y);
            SetSymbol(symbol);
        }

        public int GetPosition()
        {
            return (GetY() * 3) + GetX() + 1;
        }

        public virtual string[] ParsePosition()
        {
            string row;
            //parses the row
            if (GetY() == 0)
            {
                row = "Top";
            }
            else if (GetY() == 1)
            {
                row = "Mid";
            }
            else
            {
                row = "Bottom";
            }

            string col;
            //parses the column
            if (GetX() == 0)
            {
                col = "Left";
            }
            else if (GetX() == 1)
            {
                col = "Mid";
            }
            else
            {
                col = "Right";
            }
            string[] position = { row, col };
            return position;
        }

    }
    public class CenterNode : Node
    {
        public CenterNode(int X, int Y, char symbol = ' ') : base(X, Y, symbol) { }
        public override string[] ParsePosition()
        {
            string[] position = { "Center" };
            return position;
        }
    }
    public class CornerNode : Node
    {
        public CornerNode(int X, int Y, char symbol = ' ') : base(X, Y, symbol) { }
    }
    public class SideNode : Node
    {
        public SideNode(int X, int Y, char symbol = ' ') : base(X, Y, symbol) { }
    }
    public class GameBoard
    {
        public char m_Empty;
        public int m_Turn;
        public Node[] m_Nodes;
        public GameBoard(char empty = ' ')
        {
            m_Empty = empty;
            m_Nodes = new Node[9];
            Node nextNode;
            for (int Y = 0, i = 0; Y < 3; Y++)
            {
                for (int X = 0; X < 3; X++)
                {
                    if (X != 1 && Y != 1)
                    {
                        nextNode = new CornerNode(X, Y, m_Empty);
                    }
                    else if (X == 1 || Y == 1)
                    {
                        nextNode = new SideNode(X, Y, m_Empty);
                    }
                    else
                    {
                        nextNode = new CenterNode(X, Y, m_Empty);
                    }
                    m_Nodes[i] = nextNode;
                    i++;
                }
            }
        }
    }

    public class GameState
    {
        public Player[] m_Players { get; set; }
        public Player m_CurrentPlayer { get => GetCurrentPlayer(); }
        public int m_TurnNumber { get; set; }
        private Player GetCurrentPlayer()
        {
            return m_Players[m_TurnNumber % 2];
        }
        public GameState(Player[] players)
        {
            m_Players = players;
            m_TurnNumber = 0;
        }
    }

    public static class GameController
    {
        public static Player[] SelectPlayers(Player p1, Player p2)
        {
            //Validates Player 1's symbol
            string p1Symbol;
            string p2Symbol;
            bool validSymbol = true;
            if (p1.GetSymbol().ToString().Trim().Length != 1)
            {
                Console.WriteLine("Player 1's symbol must be a single character (not a space).");
                validSymbol = false;
            }
            else if (p1.GetSymbol() == p2.GetSymbol())
            {
                Console.WriteLine("Player 1 must have a different symbol than Player 2.");
                validSymbol = false;
            }
            while (!validSymbol)
            {
                Console.WriteLine("Please type player 1's symbol.");
                p1Symbol = Console.ReadLine().Trim();
                //Checks that only a single character is entered and that it is not a space.
                if (p1Symbol.Length == 1)
                {
                    //Checks that player 1 and player 2 have different symbols
                    if (Convert.ToChar(p1Symbol) != p2.GetSymbol())
                    {
                        //Sets player 1's symbol once a valid character is entered.
                        p1.SetSymbol(Convert.ToChar(p1Symbol));
                        validSymbol = true;
                    }
                    else
                    {
                        Console.WriteLine("Player 1 must have a different symbol than Player 2.");
                    }

                }
                else
                {
                    Console.WriteLine("Player 1's symbol must be a single character (not a space).");
                }
            }

            //Validates Player 2's symbol
            if (p2.GetSymbol().ToString().Trim().Length != 1)
            {
                Console.WriteLine("Player 2's symbol must be a single character (not a space).");
                validSymbol = false;
            }
            else if (p1.GetSymbol() == p2.GetSymbol())
            {
                Console.WriteLine("Player 2 must have a different symbol than Player 1.");
                validSymbol = false;
            }
            while (!validSymbol)
            {
                Console.WriteLine("Please type player 2's symbol.");
                p2Symbol = Console.ReadLine().Trim();
                //Checks that only a single character is entered and that it is not a space.
                if (p2Symbol.Length == 1)
                {
                    //Checks that player 1 and player 2 have different symbols
                    if (Convert.ToChar(p2Symbol) != p2.GetSymbol())
                    {
                        //Sets player 2's symbol once a valid character is entered.
                        p2.SetSymbol(Convert.ToChar(p2Symbol));
                        break;
                    }
                    Console.WriteLine("Player 2 must have a different symbol than Player 1.");
                }
                else
                {
                    Console.WriteLine("Player 2's symbol must be a single character (not a space).");
                }
            }
            return new Player[] { p1, p2 };
        }
        public static void CreateBoard(Node[] nodes)
        {
            string horizontal, vertical, grid;
            horizontal = " #################\n";
            vertical =   "      #     #     \n";
            grid = (vertical + vertical + vertical + horizontal +
                    vertical + vertical + vertical + horizontal +
                    vertical + vertical + vertical);
            Console.WriteLine(grid);
        }

        public static void UpdateBoard(int lastPlayedPosition, Node[] nodes)
        {
            int startLeft = Console.CursorLeft;
            int startTop = Console.CursorTop;
            int nextLeft, nextTop;
            if (lastPlayedPosition < 3)
            {
                nextTop = 1;
            }
            else if (lastPlayedPosition < 6)
            {
                nextTop = 5;
            }
            else
            {
                nextTop = 9;
            }
            if (lastPlayedPosition == 0 || lastPlayedPosition == 3 || lastPlayedPosition == 6)
            {
                nextLeft = 3;
            }
            else if (lastPlayedPosition == 1 || lastPlayedPosition == 4 || lastPlayedPosition == 7)
            {
                nextLeft = 9;
            }
            else
            {
                nextLeft = 15;
            }
            Console.SetCursorPosition(nextLeft, nextTop);
            Console.Write(nodes[lastPlayedPosition].GetSymbol());
            Console.SetCursorPosition(startLeft, startTop);
        }

        public static int GetPlayerMove(GameState game, GameBoard board)
        {
            int lastMove = game.m_CurrentPlayer.TakeTurn(board);
            game.m_TurnNumber++;
            return lastMove;
        }

        public static bool CheckGameOver(int turnNumber, GameBoard board)
        {
            string GameOverMsg = "";
            //Players cannot win before the 4th move is played (starting at 0)
            if (turnNumber < 4)
            {
                return false;
            }
            //Checks for a winning condition before checking for a draw.
            else if (!CheckForWin(board, ref GameOverMsg))
            {
                if (!CheckForDraw(turnNumber, ref GameOverMsg))
                {
                    return false;
                }
            }
            Console.WriteLine(GameOverMsg);
            return true;
        }

        private static bool CheckForWin(GameBoard board, ref string GameOverMsg)
        {
            //Checks inner paths
            int[,] innerPaths = { { 0, 8 }, { 2, 6 }, { 1, 7 }, { 3, 5 } };
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Node CurNode = board.m_Nodes[innerPaths[i, j]];
                    if (board.m_Nodes[4].GetSymbol() != CurNode.GetSymbol())
                    {
                        break;
                    }
                    else if (j == 1)
                    {
                        GameOverMsg = string.Format("Game Over: {0} wins!", CurNode.GetSymbol());
                        return true;
                    }
                }
            }
            //checks outer paths
            int[,] outerPaths = { { 0, 2, 1 }, { 0, 6, 3 }, { 8, 2, 5 }, { 8, 6, 7 } };
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Node CurNode = board.m_Nodes[outerPaths[i, j]];
                    if (CurNode.GetSymbol() == board.m_Empty)
                    {
                        if (i % 2 == 0 && j == 0)
                        {
                            i++;
                        }
                        else if (i < 2 && j == 1)
                        {
                            i++;
                        }
                        break;
                    }
                    else if (CurNode.GetSymbol() != board.m_Nodes[outerPaths[i, 0]].GetSymbol())
                    {
                        break;
                    }
                    else if (j == 2)
                    {
                        GameOverMsg = string.Format("Game Over: {0} wins!", CurNode.GetSymbol());
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool CheckForDraw(int turnNumber, ref string GameOverMsg)
        {
            if (turnNumber > 8)
            {
                GameOverMsg = "Game Over: Draw.";
            }
            return turnNumber > 8;
        }
    }
}
