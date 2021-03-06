using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TicTacToeClasses
{
    /// <summary>
    /// Base Player class
    /// </summary>
    public abstract class Player
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

        public abstract int EdgeCaseTakeTurn(GameBoard board);
        public abstract int TakeTurn(GameBoard board);
    }


    /// <summary>
    /// Implements the Player class for a Human Player
    /// </summary>
    public class Human : Player
    {
        public Human(char symbol) : base(symbol) { }
        public override int EdgeCaseTakeTurn(GameBoard board)
        {
            return TakeTurn(board);
        }
        public override int TakeTurn(GameBoard board)
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

    /// <summary>
    /// Implements the player class for an AI Player
    /// </summary>
    public class AI : Player
    {
        private static readonly Pathway[] m_InnerPaths = { new Pathway(4, 0, 8), new Pathway(4, 2, 6), new Pathway(4, 1, 7), new Pathway(4, 3, 5) };
        private static readonly Pathway[] m_OuterPaths = { new Pathway(0, 2, 1), new Pathway(0, 6, 3), new Pathway(8, 6, 7), new Pathway(8, 2, 5) };
        private static readonly int[] m_BestMoves = { 4, 0, 2, 6, 8, 1, 3, 5, 7 };
        public AI(char symbol) : base(symbol) { }

        public override int TakeTurn(GameBoard board)
        {
            int position  = CalcBestMove(board).Result;
            board.m_Nodes[position].SetSymbol(GetSymbol());
            return position;
        }

        /// <summary>
        /// Handles edge cases that can occur on the fourth turn
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public override int EdgeCaseTakeTurn(GameBoard board)
        {
            if (board.m_Nodes[5].GetSymbol() != GetSymbol() && board.m_Nodes[5].GetSymbol() != board.m_Empty)
            {
                if (board.m_Nodes[7].GetSymbol() != GetSymbol() && board.m_Nodes[7].GetSymbol() != board.m_Empty)
                {
                    board.m_Nodes[8].SetSymbol(GetSymbol());
                    return 8;
                }
            }
            else if (board.m_Nodes[0].GetSymbol() != GetSymbol() && board.m_Nodes[0].GetSymbol() != board.m_Empty)
            {
                if (board.m_Nodes[8].GetSymbol() != GetSymbol() && board.m_Nodes[8].GetSymbol() != board.m_Empty)
                {
                    board.m_Nodes[1].SetSymbol(GetSymbol());
                    return 1;
                }
            }
            else if (board.m_Nodes[2].GetSymbol() != GetSymbol() && board.m_Nodes[2].GetSymbol() != board.m_Empty)
            {
                if (board.m_Nodes[6].GetSymbol() != GetSymbol() && board.m_Nodes[6].GetSymbol() != board.m_Empty)
                {
                    board.m_Nodes[1].SetSymbol(GetSymbol());
                    return 1;
                }
            }
            return TakeTurn(board);
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
            int endAt;
            if (!checkOpponent)
            {
                for (int pathNum = 0; pathNum < m_OuterPaths.Length; pathNum++)
                {
                    foundWin = false;
                    WinningPosition = -1;
                    numOwned = 0;
                    Pathway pathway = m_OuterPaths[pathNum];
                    endAt = pathway.GetNodePositions().Length;
                    for (int nodeNum = 0; nodeNum < endAt; nodeNum++)
                    {
                        int Position = pathway.GetNodePositions()[nodeNum];
                        if (board.m_Nodes[Position].GetSymbol() == board.m_Empty)
                        {
                            WinningPosition = Position;
                        }
                        else if (board.m_Nodes[Position].GetSymbol() == this.GetSymbol())
                        {
                            //increments the numOwned when the player's symbol is found
                            numOwned++;
                            if (numOwned > 1)
                            {
                                foundWin = true;
                            }
                        }
                        else
                        {
                            //Checks the 1st and 3rd pathways.
                            if (pathNum % 2 == 0)
                            {
                                //Skips the 2nd and 4th pathways.
                                if (nodeNum == 0)
                                {
                                    pathNum++;
                                }
                                //prevents last node from being checked
                                else if (pathNum == 0 && pathNum == nodeNum)
                                {
                                    endAt--;
                                }
                            }
                            //skips the 3rd pathway
                            else if (nodeNum == 1)
                            {
                                pathNum++;
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
                for (int pathNum = 0; pathNum < m_OuterPaths.Length; pathNum++)
                {
                    foundWin = false;
                    WinningPosition = -1;
                    numOwned = 0;
                    Pathway pathway = m_OuterPaths[pathNum];
                    endAt = pathway.GetNodePositions().Length;
                    for (int nodeNum = 0; nodeNum < endAt; nodeNum++)
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
                            //Checks the 1st and 3rd pathways.
                            if (pathNum % 2 == 0)
                            {
                                //Skips the 2nd and 4th pathways.
                                if (nodeNum == 0)
                                {
                                    pathNum++;
                                }
                                //prevents last node from being checked
                                else if (pathNum == 0 && pathNum == nodeNum)
                                {
                                    endAt--;
                                }
                            }
                            //skips the 3rd pathway
                            else if (nodeNum == 1)
                            {
                                pathNum++;
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
}
