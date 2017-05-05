using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockEngine.Blocks
{
    class BlockLeaves : Block
    {

        private int oak;

        public override int GetTexturePosition(Direction direction)
        {
            return oak;
        }

        public override bool IsBlockAnimated()
        {
            return false;
        }

        protected override void InitTextures(TextureManager textureManager)
        {
            oak = textureManager.RegisterTexture("leaves_oak");
        }

        public override long GetBreakTime(int toolId)
        {
            return 200L;
        }
    }
}
