using UnityEngine;
using BlockEngine.Blocks;

public abstract class Block
{
    public enum Direction { north, east, south, west, up, down };

    // Texture
    public static TextureManager textureManager = new TextureManager();

    public static Block[] prototypes = new Block[]{
        /* 0 */ new BlockAir(),
        /* 1 */ new BlockStone(),
        /* 2 */ new BlockDirt(),
        /* 3 */ new BlockGrass(),
        /* 4 */ new BlockSand(),
        /* 5 */ new BlockWater(),
        /* 6 */ new BlockLog(),
        /* 7 */ new BlockLeaves()
    };

    public static int[] meshBlocks = new int[]
    {

    };

    public static bool isMeshBlock(int id)
    {
        return System.Array.Exists(meshBlocks, (t) => { return id == t; });
    }

    public static readonly Block AIR = prototypes[0];
    public static readonly Block STONE = prototypes[1];
    public static readonly Block DIRT = prototypes[2];
    public static readonly Block GRASS = prototypes[3];
    public static readonly Block SAND = prototypes[4];
    public static readonly Block WATER = prototypes[5];
    public static readonly Block LOG = prototypes[6];
    public static readonly Block LEAVES = prototypes[7];

    // Initialization
    protected abstract void InitTextures(TextureManager textureManager);

    public static void InitializePrototypes()
    {
        for(int b = 0; b < prototypes.Length; b++)
        {
            prototypes[b]._id = b;
            prototypes[b].InitTextures(textureManager);
        }
        textureManager.PackTextures();
    }

    private int _id;

    public int id
    {
        get
        {
            return _id;
        }
    }

    public abstract bool IsBlockAnimated();

    protected Block GetUpperBlockPrototype(Chunk chunk, int x, int y, int z)
    {
        Chunk upperChunk = chunk.relations[4];
        if (y == Chunk.chunkSize - 1)
        {
            if (upperChunk == null) return null;
            int upper = upperChunk.GetBlock(x, 0, z);
            if (upper == 0) return null;
            return Block.prototypes[upper];
        }
        return prototypes[chunk.GetBlock(x, y + 1, z)];
    }
    protected virtual bool isUpperSolid(Chunk chunk, int x, int y, int z, int meta)
    {
        Block upperBlock = GetUpperBlockPrototype(chunk, x, y, z);
        if (upperBlock == null) return false;
        return upperBlock.IsSolid(upperBlock, Direction.down, meta);
    }

    protected Block GetLowerBlockPrototype(Chunk chunk, int x, int y, int z)
    {
        Chunk lowerChunk = chunk.relations[5];
        if (y == Chunk.chunkSize - 1)
        {
            if (lowerChunk == null) return null;
            int lower = lowerChunk.GetBlock(x, 15, z);
            if (lower == 0) return null;
            return Block.prototypes[lower];
        }
        return prototypes[chunk.GetBlock(x, y - 1, z)];
    }

    protected virtual bool isLowerSolid(Chunk chunk, int x, int y, int z, int meta)
    {
        Block lowerBlock = GetLowerBlockPrototype(chunk, x, y, z);
        if(lowerBlock == null) return false;
        return lowerBlock.IsSolid(lowerBlock, Direction.up, meta);
    }

    protected Block GetNorthernBlockPrototype(Chunk chunk, int x, int y, int z)
    {
        Chunk northernChunk = chunk.relations[0];
        if (z == Chunk.chunkSize - 1)
        {
            if (northernChunk == null) return null;
            int nothern = northernChunk.GetBlock(x, y, 0);
            if (nothern == 0) return null;
            return Block.prototypes[nothern];
        }
        return prototypes[chunk.GetBlock(x, y, z + 1)];
    }

    protected virtual bool isNorthernSolid(Chunk chunk, int x, int y, int z, int meta)
    {
        Block northernBlock = GetNorthernBlockPrototype(chunk, x, y, z);
        if (northernBlock == null) return false;
        return northernBlock.IsSolid(northernBlock, Direction.south, meta);
    }

    protected Block GetSouthernBlockPrototype(Chunk chunk, int x, int y, int z)
    {
        Chunk southernChunk = chunk.relations[2];
        if (z == 0)
        {
            if (southernChunk == null) return null;
            int southern = southernChunk.GetBlock(x, y, 15);
            if (southern == 0) return null;
            return Block.prototypes[southern];
        }
        return prototypes[chunk.GetBlock(x, y, z - 1)];
    }

    protected virtual bool isSouthernSolid(Chunk chunk, int x, int y, int z, int meta)
    {
        Block southernBlock = GetSouthernBlockPrototype(chunk, x, y, z);
        if (southernBlock == null) return false;
        return southernBlock.IsSolid(southernBlock, Direction.north, meta);
    }

    protected Block GetEasternBlockPrototype(Chunk chunk, int x, int y, int z)
    {
        Chunk easternChunk = chunk.relations[1];
        if (x == Chunk.chunkSize - 1)
        {
            if (easternChunk == null) return null;
            int eastern = easternChunk.GetBlock(0, y, z);
            if (eastern == 0) return null;
            return Block.prototypes[eastern];
        }
        return prototypes[chunk.GetBlock(x + 1, y, z)];
    }

    protected virtual bool isEasternSolid(Chunk chunk, int x, int y, int z, int meta)
    {
        Block easternBlock = GetEasternBlockPrototype(chunk, x, y, z);
        if (easternBlock == null) return false;
        return easternBlock.IsSolid(easternBlock, Direction.west, meta);
    }

    protected Block GetWesternBlockPrototype(Chunk chunk, int x, int y, int z)
    {
        Chunk westernChunk = chunk.relations[3];
        if (x == 0)
        {
            if (westernChunk == null) return null;
            int western = westernChunk.GetBlock(15, y, z);
            if (western == 0) return null;
            return Block.prototypes[western];
        }
        return prototypes[chunk.GetBlock(x - 1, y, z)];
    }

    protected virtual bool isWesternSolid(Chunk chunk, int x, int y, int z, int meta)
    {
        Block westernBlock = GetWesternBlockPrototype(chunk, x, y, z);
        if (westernBlock == null) return false;
        return westernBlock.IsSolid(westernBlock, Direction.east, meta);
    }


    public virtual MeshData GetBlockMeshData (Chunk chunk, Chunk[] relations, int x, int y, int z, int meta, MeshData meshData)
    {
        if (!isUpperSolid(chunk, x, y, z, meta)){
            meshData = FaceDataUp(chunk, x, y, z, meta,  meshData);
        }

        
        if (!isLowerSolid(chunk, x, y, z, meta))
        {
            meshData = FaceDataDown(chunk, x, y, z, meta, meshData);
        }
        
        if (!isNorthernSolid(chunk, x, y, z, meta))
        {
            meshData = FaceDataNorth(chunk, x, y, z, meta, meshData);
        }

        if (!isSouthernSolid(chunk, x, y, z, meta))
        {
            meshData = FaceDataSouth(chunk, x, y, z, meta, meshData);
        }

        if (!isEasternSolid(chunk, x, y, z, meta))
        {
            meshData = FaceDataEast(chunk, x, y, z, meta, meshData);
        }

        if (!isWesternSolid(chunk, x, y, z, meta))
        {
            meshData = FaceDataWest(chunk, x, y, z, meta, meshData);
        }

        return meshData;

    }

    public virtual bool HasFaceCollider(Direction dir, int meta)
    {
        return true;
    }

    #region "face data"
    protected virtual MeshData FaceDataUp
        (Chunk chunk, int x, int y, int z, int meta, MeshData meshData)
    {
        meshData.vertices.Add(new Vector3(x, y + 1f, z + 1f));
        meshData.vertices.Add(new Vector3(x + 1f, y + 1f, z + 1f));
        meshData.vertices.Add(new Vector3(x + 1f, y + 1f, z));
        meshData.vertices.Add(new Vector3(x, y + 1f, z));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(GetFaceUVs(Direction.up, meta));

        if(HasFaceCollider(Direction.up, meta))
        {
            meshData.collider.Add(new Vector3(x, y + 1f, z + 1f));
            meshData.collider.Add(new Vector3(x + 1f, y + 1f, z + 1f));
            meshData.collider.Add(new Vector3(x + 1f, y + 1f, z));
            meshData.collider.Add(new Vector3(x, y + 1f, z));

            meshData.AddQuadColliderTriangles();
        }
        return meshData;
    }

    protected virtual MeshData FaceDataDown
        (Chunk chunk, int x, int y, int z, int meta, MeshData meshData)
    {
        meshData.vertices.Add(new Vector3(x, y, z));
        meshData.vertices.Add(new Vector3(x + 1f, y, z));
        meshData.vertices.Add(new Vector3(x + 1f, y, z + 1f));
        meshData.vertices.Add(new Vector3(x, y, z + 1f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(GetFaceUVs(Direction.down, meta));
        if (HasFaceCollider(Direction.down, meta))
        {
            meshData.collider.Add(new Vector3(x, y, z));
            meshData.collider.Add(new Vector3(x + 1f, y, z));
            meshData.collider.Add(new Vector3(x + 1f, y, z + 1f));
            meshData.collider.Add(new Vector3(x, y, z + 1f));

            meshData.AddQuadColliderTriangles();
        }
        return meshData;
    }

    protected virtual MeshData FaceDataNorth
        (Chunk chunk, int x, int y, int z, int meta, MeshData meshData)
    {
        meshData.vertices.Add(new Vector3(x + 1f, y, z + 1f));
        meshData.vertices.Add(new Vector3(x + 1f, y + 1f, z + 1f));
        meshData.vertices.Add(new Vector3(x, y + 1f, z + 1f));
        meshData.vertices.Add(new Vector3(x, y, z + 1f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(GetFaceUVs(Direction.north, meta));

        if(HasFaceCollider(Direction.north, meta))
        {
            meshData.collider.Add(new Vector3(x + 1f, y, z + 1f));
            meshData.collider.Add(new Vector3(x + 1f, y + 1f, z + 1f));
            meshData.collider.Add(new Vector3(x, y + 1f, z + 1f));
            meshData.collider.Add(new Vector3(x, y, z + 1f));

            meshData.AddQuadColliderTriangles();
        }
        return meshData;
    }

    protected virtual MeshData FaceDataEast
        (Chunk chunk, int x, int y, int z, int meta, MeshData meshData)
    {
        meshData.vertices.Add(new Vector3(x + 1f, y, z));
        meshData.vertices.Add(new Vector3(x + 1f, y + 1f, z));
        meshData.vertices.Add(new Vector3(x + 1f, y + 1f, z + 1f));
        meshData.vertices.Add(new Vector3(x + 1f, y, z + 1f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(GetFaceUVs(Direction.east, meta));

        if(HasFaceCollider(Direction.east, meta))
        {
            meshData.collider.Add(new Vector3(x + 1f, y, z));
            meshData.collider.Add(new Vector3(x + 1f, y + 1f, z));
            meshData.collider.Add(new Vector3(x + 1f, y + 1f, z + 1f));
            meshData.collider.Add(new Vector3(x + 1f, y, z + 1f));

            meshData.AddQuadColliderTriangles();
        }
        return meshData;
    }

    protected virtual MeshData FaceDataSouth
        (Chunk chunk, int x, int y, int z, int meta, MeshData meshData)
    {
        meshData.vertices.Add(new Vector3(x, y, z));
        meshData.vertices.Add(new Vector3(x, y + 1f, z));
        meshData.vertices.Add(new Vector3(x + 1f, y + 1f, z));
        meshData.vertices.Add(new Vector3(x + 1f, y, z));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(GetFaceUVs(Direction.south, meta));

        if(HasFaceCollider(Direction.south, meta))
        {
            meshData.collider.Add(new Vector3(x, y, z));
            meshData.collider.Add(new Vector3(x, y + 1f, z));
            meshData.collider.Add(new Vector3(x + 1f, y + 1f, z));
            meshData.collider.Add(new Vector3(x + 1f, y, z));

            meshData.AddQuadColliderTriangles();
        }
        return meshData;
    }

    protected virtual MeshData FaceDataWest
        (Chunk chunk, int x, int y, int z, int meta, MeshData meshData)
    {
        meshData.vertices.Add(new Vector3(x, y, z + 1f));
        meshData.vertices.Add(new Vector3(x, y + 1f, z + 1f));
        meshData.vertices.Add(new Vector3(x, y + 1f, z));
        meshData.vertices.Add(new Vector3(x, y, z));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(GetFaceUVs(Direction.west, meta));

        if(HasFaceCollider(Direction.west, meta))
        {
            meshData.collider.Add(new Vector3(x, y, z + 1f));
            meshData.collider.Add(new Vector3(x, y + 1f, z + 1f));
            meshData.collider.Add(new Vector3(x, y + 1f, z));
            meshData.collider.Add(new Vector3(x, y, z));

            meshData.AddQuadColliderTriangles();
        }
        return meshData;
    }
    #endregion

    #region "uv data"
    public abstract int GetTexturePosition(Direction direction, int meta);

    public virtual Vector2[] GetFaceUVs(Direction direction, int meta)
    {
        Vector2[] UVs = new Vector2[4];
        Rect tilePos = textureManager.texturePositions[0][GetTexturePosition(direction, meta)];
        /*
        UVs[0] = new Vector2(tilePos.width * tilePos.x + tilePos.width,
            tilePos.height * tilePos.y);
        UVs[1] = new Vector2(tilePos.width * tilePos.x + tilePos.width,
            tilePos.height * tilePos.y + tilePos.height);
        UVs[2] = new Vector2(tilePos.width * tilePos.x,
            tilePos.height * tilePos.y + tilePos.height);
        UVs[3] = new Vector2(tilePos.width * tilePos.x,
            tilePos.height * tilePos.y);
        */
        UVs[0] = new Vector2(tilePos.x + tilePos.width, tilePos.y);
        UVs[1] = new Vector2(tilePos.x + tilePos.width, tilePos.y + tilePos.height);
        UVs[2] = new Vector2(tilePos.x, tilePos.y + tilePos.height);
        UVs[3] = new Vector2(tilePos.x, tilePos.y);
        return UVs;
    }
    #endregion

    public long GetBreakTime(Org.Dragonet.Cloudland.Net.Protocol.SerializedItem item)
    {
        if (item == null) return GetBreakTime(0, 0);
        return GetBreakTime(item.Id, item.Meta);
    }

    public virtual long GetBreakTime(int toolId, int toolMeta)
    {
        return 500L;
    }

    public virtual bool IsSolid(Block target, Direction direction, int meta)
    {
        return true;
    }

}