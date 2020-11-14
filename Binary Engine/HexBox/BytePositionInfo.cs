using System;
using System.Collections.Generic;
using System.Text;

namespace Binary_Engine.HexBox
{
    /// <summary>
    /// Represents a position in the HexBox control
    /// </summary>
    struct BytePositionInfo
    {
        public BytePositionInfo(long index, int characterPosition)
        {
            Index = index;
            CharacterPosition = characterPosition;
        }

        public int CharacterPosition { get; }

        public long Index { get; }
    }
}
