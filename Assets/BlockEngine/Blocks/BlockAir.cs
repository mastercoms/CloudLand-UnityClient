using UnityEngine;
using System.Collections;
using System;

namespace BlockEngine.Blocks
{
    public class BlockAir : Block
    {
        public BlockAir()
            : base()
        {
        }

        protected override void InitTextures(TextureManager textureManager)
        {
        }

        public override int GetTexturePosition(Direction direction)
        {
            return -1;
        }

        public override bool IsBlockAnimated()
        {
            return false;
        }

        public override MeshData GetBlockMeshData(Chunk chunk, Chunk[] relations, int x, int y, int z, MeshData meshData)
        {
            return meshData;
        }

        public override bool IsSolid(Block target, Direction direction)
        {
            return false;
        }
    }
}
