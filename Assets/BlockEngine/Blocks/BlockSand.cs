using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockEngine.Blocks
{
    class BlockSand : Block
    {

        private int tex;


        public override long GetBreakTime(int toolId)
        {
            return 200L;
        }

        public override int GetTexturePosition(Direction direction)
        {
            return tex;
        }

        public override bool IsBlockAnimated()
        {
            return false;
        }

        protected override void InitTextures(TextureManager textureManager)
        {
            tex = textureManager.RegisterTexture("sand");
        }
    }
}
