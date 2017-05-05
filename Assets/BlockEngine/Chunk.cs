using UnityEngine;
using System.Threading;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{

    public static int renderDistanceSquared = 12*12;

    public static int renderingCount = 0;

    public static string Key(int x, int y, int z)
    {
        return "chunk|" + x + "|" + y + "|" + z;
    }

    public struct ChunkPosition
    {
        public int x;
        public int y;
        public int z;

        public ChunkPosition(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChunkPosition)) return false;
            ChunkPosition target = (ChunkPosition)obj;
            return this.x == target.x && this.y == target.y && this.z == target.z;
        }
    }

    public ChunkManager chunkManager;
    public ChunkPosition position;

    public Chunk[] relations = new Chunk[6];

    public byte[] blocks;
    public ArrayList blockMeshes = new ArrayList();

    public const int chunkSize = 16;
    public bool update = false;
    public bool updateRelations = true;
    public bool forceUpdate = false;

    private bool animated = false;
    private int renderedFrame;

    private bool rendered = false;

    MeshRenderer meshRenderer;
    MeshFilter filter;
    Mesh mesh;
    MeshCollider coll;

    private MeshData meshData;
    private object meshDataLock = new object();

    // Use this for initialization
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        filter = GetComponent<MeshFilter>();
        coll = GetComponent<MeshCollider>();
    }

    public int GetChunkDistanceToPlayer()
    {
        Vector3 playerPos = ClientComponent.INSTANCE.playerObject.transform.position;
        int pChunkX = (int)(playerPos.x) >> 4;
        int pChunkZ = (int)(playerPos.z) >> 4;
        int dxs = (position.x - pChunkX) * (position.x - pChunkX);
        int dzs = (position.z - pChunkZ) * (position.z - pChunkZ);
        return Mathf.FloorToInt(dxs + dzs);
    }

    void Update()
    {
        if(updateRelations)
        {
            updateRelations = false;
            chunkManager.chunkQueue.QueueRelations(this);
        }

        int dSqr = GetChunkDistanceToPlayer();
        if (rendered == false && dSqr <= renderDistanceSquared)
        {
            rendered = true;
            update = true;
        }
        else if(rendered == true && dSqr > renderDistanceSquared)
        {
            rendered = false;
            filter.mesh.Clear();
        }
        if (update || forceUpdate)
        {
            update = false;
            ThreadPool.QueueUserWorkItem(UpdateChunk);
        }
        if (rendered && renderedFrame != GlobalFrame.Frame)
        {
            UpdateMaterial();
        }
    }

    public static int pos2index(int x, int y, int z)
    {
        return (((z & 0xF) << 8) | ((x & 0xF) << 4) | (y & 0xF)) * 2;
    }

    public int GetBlock(int x, int y, int z)
    {
        return blocks[pos2index(x, y, z)] << 8 | blocks[pos2index(x, y, z) + 1];
    }

    public void SetBlock(int x, int y, int z, int id)
    {
        blocks[pos2index(x, y, z)] = (byte)((id >> 8) & 0xFF);
        blocks[pos2index(x, y, z) + 1] = (byte)(id & 0xFF);
    }

    // Updates the chunk based on its contents
    void UpdateChunk(object state)
    {
        lock (meshDataLock)
        {
            meshData = new MeshData();
            //if (rendered/* || (position.x * position.y * position.z) % 3 != 0*/)
            //{
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        int idx = pos2index(x, y, z);
                        int id = (blocks[idx] << 8 | blocks[idx + 1]);
                        if (Block.isMeshBlock(id))
                        {
                            blockMeshes.Add(new int[] { x, y, z, id });
                        }
                        else
                        {
                            Block block = Block.prototypes[id];
                            meshData = block.GetBlockMeshData(this, relations, x, y, z, meshData);
                            if (block.IsBlockAnimated()) animated = true;
                        }
                    }
                }
            }
            //}
            meshData.verticesArray = meshData.vertices.ToArray();
            meshData.trianglesArray = meshData.triangles.ToArray();
            meshData.colliderArray = meshData.collider.ToArray();
            meshData.colliderTriangles = meshData.collTri.ToArray();
            meshData.uvArray = meshData.uv.ToArray();
            if (forceUpdate)
            {
                blockMeshes.Clear();
                forceUpdate = false;
                Loom.QueueOnMainThread(() => RenderMesh());
            }
            else if (rendered)
            {
                blockMeshes.Clear();
                chunkManager.chunkQueue.QueueRender(this);
            }
        }
    }

    public void clearChildMeshes()
    {
        if (transform.childCount == 0) return;
        Transform[] t = new Transform[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            t[i] = transform.GetChild(i);
        }
        foreach(Transform o in t)
        {
            DestroyImmediate(o.gameObject);
        }
    }

    // Sends the calculated mesh information
    // to the mesh and collision components
    public void RenderMesh()
    {
        lock (meshDataLock)
        {
            clearChildMeshes();
            mesh = new Mesh();

            mesh.vertices = meshData.verticesArray;
            mesh.triangles = meshData.trianglesArray;

            mesh.uv = meshData.uvArray;
            mesh.RecalculateNormals();

            filter.mesh = mesh;
            coll.sharedMesh = null;
            coll.sharedMesh = mesh;
        }

        foreach (int[] loc in blockMeshes)
        {
            GameObject prefab = (GameObject) Resources.Load("MeshBlocks/" + loc[3] + "_" + loc[4]);
            GameObject n = Instantiate(prefab, new Vector3(loc[0], loc[1], loc[2]), Quaternion.Euler(Vector3.zero), transform);
            n.transform.name = string.Format("MeshBlock|{0}|{1}|{2}", loc[0], loc[1], loc[2]);
        }

        UpdateMaterial();
    }

    public void UpdateMaterial()
    {
        meshRenderer.material = Block.textureManager.packedMaterials[GlobalFrame.Frame];
    }
}