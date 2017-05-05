using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockEngine.Blocks
{
    class BlockWater : TransparentBlock
    {

        private int tex;

        public override int GetTexturePosition(Direction direction)
        {
            return tex;
        }

        public override bool IsBlockAnimated()
        {
            return true;
        }

        public override bool HasFaceCollider(Direction dir)
        {
            return false;
        }

        protected override void InitTextures(TextureManager textureManager)
        {
            tex = textureManager.RegisterTexture("water_still", true);
        }
    }
}
