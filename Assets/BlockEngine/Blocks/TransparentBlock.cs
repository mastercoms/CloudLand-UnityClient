using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockEngine.Blocks
{
    abstract class TransparentBlock : Block
    {
        protected override bool isUpperSolid(Chunk chunk, int x, int y, int z, int meta)
        {
            if (GetUpperBlockPrototype(chunk, x, y, z).id == id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool isLowerSolid(Chunk chunk, int x, int y, int z, int meta)
        {
            if (GetLowerBlockPrototype(chunk, x, y, z).id == id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool isNorthernSolid(Chunk chunk, int x, int y, int z, int meta)
        {
            if (GetNorthernBlockPrototype(chunk, x, y, z).id == id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool isEasternSolid(Chunk chunk, int x, int y, int z, int meta)
        {
            if (GetEasternBlockPrototype(chunk, x, y, z).id == id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool isSouthernSolid(Chunk chunk, int x, int y, int z, int meta)
        {
            if (GetSouthernBlockPrototype(chunk, x, y, z).id == id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool isWesternSolid(Chunk chunk, int x, int y, int z, int meta)
        {
            if (GetWesternBlockPrototype(chunk, x, y, z).id == id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool IsSolid(Block target, Direction direction, int meta)
        {
            if (target.id == id)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
