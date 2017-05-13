namespace BlockEngine.Blocks
{
    abstract class TransparentBlock : Block
    {
        protected override bool isUpperSolid(Chunk chunk, int x, int y, int z)
        {
			return false;
        }

        protected override bool isLowerSolid(Chunk chunk, int x, int y, int z)
        {
			return false;
        }

        protected override bool isNorthernSolid(Chunk chunk, int x, int y, int z)
        {
			return false;
        }

        protected override bool isEasternSolid(Chunk chunk, int x, int y, int z)
        {
			return false;
        }

        protected override bool isSouthernSolid(Chunk chunk, int x, int y, int z)
        {
			return false;
        }

        protected override bool isWesternSolid(Chunk chunk, int x, int y, int z)
        {
			return false;
        }

        public override bool IsSolid(Block target, Direction direction)
        {
			return false;
        }
    }
}
