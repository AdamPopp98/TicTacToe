using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Client.Classes
{
    public class TileModel
    {
        private int Position { get; set; }
        private char CurValue { get; set; }
        public int Row { get => GetRow(); }
        public int Col { get => GetCol(); }
        public TileModel(int position, char val = 't')
        {
            CurValue = val;
            Position = position;
        }

        public char GetValue()
        {
            return CurValue;
        }
        public bool SetValue(char val)
        {
            if (CurValue == ' ')
            {
                CurValue = val;
                return true;
            }
            return false;
        }
        private int GetRow()
        {
            if (CurValue < 3)
            {
                return 0;
            }
            else if (CurValue < 6)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        private int GetCol()
        {
            return CurValue % 3;
        }
    }
}
