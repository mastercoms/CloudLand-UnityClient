using UnityEngine;
using System.Collections;
using System;

namespace BlockEngine.Blocks
{
    public class BlockStone : Block
    {

        private int tex;

        public override long GetBreakTime(int toolId, int toolMeta)
        {
            return 10000L;
        }

        public override bool IsBlockAnimated()
        {
            return false;
        }

        protected override void InitTextures(TextureManager textureManager)
        {
            tex = textureManager.RegisterTexture("stone");
        }

        public override int GetTexturePosition(Direction direction, int meta)
        {
            return tex;
        }
    }
}
