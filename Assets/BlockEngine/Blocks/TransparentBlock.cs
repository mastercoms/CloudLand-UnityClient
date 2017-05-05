namespace BlockEngine.Blocks
{
    abstract class TransparentBlock : Block
    {
        protected override bool isUpperSolid(Chunk chunk, int x, int y, int z)
        {
            if (GetUpperBlockPrototype(chunk, x, y, z).id == id)
            {
                return true;
            }
            else
            {
                return base.isUpperSolid(chunk, x, y, z);
            }
        }

        protected override bool isLowerSolid(Chunk chunk, int x, int y, int z)
        {
            if (GetLowerBlockPrototype(chunk, x, y, z).id == id)
            {
                return true;
            }
            else
            {
                return base.isLowerSolid(chunk, x, y, z);
            }
        }

        protected override bool isNorthernSolid(Chunk chunk, int x, int y, int z)
        {
            if (GetNorthernBlockPrototype(chunk, x, y, z).id == id)
            {
                return true;
            }
            else
            {
                return base.isNorthernSolid(chunk, x, y, z);
            }
        }

        protected override bool isEasternSolid(Chunk chunk, int x, int y, int z)
        {
            if (GetEasternBlockPrototype(chunk, x, y, z).id == id)
            {
                return true;
            }
            else
            {
                return base.isEasternSolid(chunk, x, y, z);
            }
        }

        protected override bool isSouthernSolid(Chunk chunk, int x, int y, int z)
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

        protected override bool isWesternSolid(Chunk chunk, int x, int y, int z)
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

        public override bool IsSolid(Block target, Direction direction)
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
