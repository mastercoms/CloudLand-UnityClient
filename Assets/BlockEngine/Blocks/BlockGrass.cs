using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockEngine.Blocks
{
    class BlockGrass : Block
    {

        private int top;
        private int side;
        private int down;

        public override long GetBreakTime(int toolId)
        {
            return 200L;
        }

        public override bool IsBlockAnimated()
        {
            return false;
        }



        // Textures 
        protected override void InitTextures(TextureManager textureManager)
        {
            top = textureManager.RegisterTexture("grass_top");
            side = textureManager.RegisterTexture("grass_side");
            down = textureManager.RegisterTexture("grass_down");
        }

        public override int GetTexturePosition(Direction direction)
        {
            if (direction == Direction.up) return top;
            if (direction == Direction.down) return down;
            return side;
        }

    }
}
