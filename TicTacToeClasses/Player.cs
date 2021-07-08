using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TicTacToeClasses
{
    public class Player
    {
        private char m_Symbol;

        public char GetSymbol()
        {
            return m_Symbol;
        }
        public void SetSymbol(char symbol)
        {
            m_Symbol = symbol;
        }
        public Player(char symbol)
        {
            SetSymbol(symbol);
        }
        public virtual int TakeTurn(GameBoard board)
        {
            int position;
            while (true)
            {
                Console.WriteLine("What position would you like to take? (1 to 9 read left to right and top to bottom.)");
                position = Convert.ToInt32(Console.ReadLine()) - 1;
                //Checks that the position entered is between 1 and 9 inclusive.
                if (position < 9 && position > -1)
                {
                    //Checks that the position selected is an empty space
                    if (board.m_Nodes[position].GetSymbol() == board.m_Empty)
                    {
                        board.m_Nodes[position].SetSymbol(GetSymbol());
                        return position;
                    }
                    Console.WriteLine("Please select and empty space.");
                }
                else
                {
                    Console.WriteLine("Position must be between 1 and 9 (inclusive)");
                }
            }
        }
    }

    public class Human : Player
    {
        public Human(char symbol) : base(symbol) { }
    }

    public class AI : Player
    {
        private static readonly Pathway[] m_InnerPaths = { new Pathway(4, 0, 8), new Pathway(4, 2, 6), new Pathway(4, 1, 7), new Pathway(4, 3, 5) };
        private static readonly Pathway[] m_OuterPaths = { new Pathway(0, 2, 1), new Pathway(0, 6, 3), new Pathway(2, 8, 5), new Pathway(6, 8, 7) };
        private static readonly int[] m_BestMoves = { 4, 0, 2, 6, 8, 1, 3, 5, 7 };
        public AI(char symbol) : base(symbol) { }

        public override int TakeTurn(GameBoard board)
        {
            int position  = CalcBestMove(board).Result;
            board.m_Nodes[position].SetSymbol(GetSymbol());
            return position;
        }
        private async Task<int> CalcBestMove(GameBoard board)
        {
            List<Task<int>> tasks = new List<Task<int>>();
            tasks.Add(Task.Run(() => WinGameAsync(board)));
            tasks.Add(Task.Run(() => PreventOpponentWinAsync(board)));
            tasks.Add(Task.Run(() => MakeBestMoveAsync(board)));

            var moves = await Task.WhenAll(tasks);
            foreach (var move in moves)
            {
                if (move != -1)
                {
                    return move;
                }
            }
            return -1;
        }
        private int WinGameAsync(GameBoard board)
        {
            int innerCheck = CheckInnerPaths(board);
            if (innerCheck == -1)
            {
                return CheckOuterPaths(board);
            }
            return innerCheck;
        }
        private int PreventOpponentWinAsync(GameBoard board)
        {
            int innerCheck = CheckInnerPaths(board, true);
            if (innerCheck == -1)
            {
                return CheckOuterPaths(board, true);
            }
            return innerCheck;
        }

        private int CheckInnerPaths(GameBoard board, bool checkOpponent = false)
        {
            bool foundWin;
            int WinningPosition;
            int numOwned;
            if (!checkOpponent)
            {
                for (int pathNum = 0; pathNum < m_InnerPaths.Length; pathNum++)
                {
                    foundWin = false;
                    WinningPosition = -1;
                    numOwned = 0;
                    Pathway pathway = m_InnerPaths[pathNum];
                    for (int nodeNum = 0; nodeNum < pathway.GetNodePositions().Length; nodeNum++)
                    {
                        int Position = pathway.GetNodePositions()[nodeNum];
                        if (board.m_Nodes[Position].GetSymbol() == board.m_Empty)
                        {
                            WinningPosition = Position;
                        }
                        else if (board.m_Nodes[Position].GetSymbol() == this.GetSymbol())
                        {
                            numOwned++;
                            if (numOwned > 1)
                            {
                                foundWin = true;
                            }
                        }
                        else
                        {
                            if (Position == 4)
                            {
                                pathNum += 4;
                            }
                            break;
                        }
                        if (WinningPosition != -1 && foundWin == true)
                        {
                            return WinningPosition;
                        }
                    }
                }
            }
            else
            {
                for (int pathNum = 0; pathNum < m_InnerPaths.Length; pathNum++)
                {
                    foundWin = false;
                    WinningPosition = -1;
                    numOwned = 0;
                    Pathway pathway = m_InnerPaths[pathNum];
                    for (int nodeNum = 0; nodeNum < pathway.GetNodePositions().Length; nodeNum++)
                    {
                        int Position = pathway.GetNodePositions()[nodeNum];
                        if (board.m_Nodes[Position].GetSymbol() == board.m_Empty)
                        {
                            WinningPosition = Position;
                        }
                        else if (board.m_Nodes[Position].GetSymbol() != this.GetSymbol())
                        {
                            numOwned++;
                            if (numOwned > 1)
                            {
                                foundWin = true;
                            }
                        }
                        else
                        {
                            if (Position == 4)
                            {
                                pathNum += 4;
                            }
                            break;
                        }
                        if (WinningPosition != -1 && foundWin == true)
                        {
                            return WinningPosition;
                        }
                    }
                }
            }
            return -1;
        }
        private int CheckOuterPaths(GameBoard board, bool checkOpponent = false)
        {
            bool foundWin;
            int WinningPosition;
            int numOwned;
            if (!checkOpponent)
            {
                for (int pathNum = 0; pathNum < m_OuterPaths.Length; pathNum++)
                {
                    foundWin = false;
                    WinningPosition = -1;
                    numOwned = 0;
                    Pathway pathway = m_OuterPaths[pathNum];
                    for (int nodeNum = 0; nodeNum < pathway.GetNodePositions().Length; nodeNum++)
                    {
                        int Position = pathway.GetNodePositions()[nodeNum];
                        if (board.m_Nodes[Position].GetSymbol() == board.m_Empty)
                        {
                            WinningPosition = Position;
                        }
                        else if (board.m_Nodes[Position].GetSymbol() == this.GetSymbol())
                        {
                            numOwned++;
                            if (numOwned > 1)
                            {
                                foundWin = true;
                            }
                        }
                        else
                        {
                            break;
                        }
                        if (WinningPosition != -1 && foundWin == true)
                        {
                            return WinningPosition;
                        }
                    }
                }
            }
            else
            {
                for (int pathNum = 0; pathNum < m_OuterPaths.Length; pathNum++)
                {
                    foundWin = false;
                    WinningPosition = -1;
                    numOwned = 0;
                    Pathway pathway = m_OuterPaths[pathNum];
                    for (int nodeNum = 0; nodeNum < pathway.GetNodePositions().Length; nodeNum++)
                    {
                        int Position = pathway.GetNodePositions()[nodeNum];
                        if (board.m_Nodes[Position].GetSymbol() == board.m_Empty)
                        {
                            WinningPosition = Position;
                        }
                        else if (board.m_Nodes[Position].GetSymbol() != this.GetSymbol())
                        {
                            numOwned++;
                            if (numOwned > 1)
                            {
                                foundWin = true;
                            }
                        }
                        else
                        {
                            break;
                        }
                        if (WinningPosition != -1 && foundWin == true)
                        {
                            return WinningPosition;
                        }
                    }
                }
            }
            return -1;
        }
        private int MakeBestMoveAsync(GameBoard board)
        {
            for (int i = 0; i < 9; i++)
            {
                if (board.m_Nodes[m_BestMoves[i]].GetSymbol() == board.m_Empty)
                {
                    return m_BestMoves[i];
                }
            }
            return -1;
        }
    }
    public class Pathway
    {
        private int[] m_NodePositions;
        public int[] GetNodePositions()
        {
            return m_NodePositions;
        }
        public void SetNodePositions(int[] positions)
        {
            m_NodePositions = positions;
        }
        public void SetNodePositions(int first, int second, int third)
        {
            m_NodePositions = new int[] { first, second, third };
        }
        public Pathway(int first, int second, int third)
        {
            SetNodePositions(first, second, third);
        }
    }

}
