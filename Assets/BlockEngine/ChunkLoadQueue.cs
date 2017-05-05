using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoadQueue : MonoBehaviour {

    public class ChunkCreationParameters
    {
        public int x;
        public int y;
        public int z;
        public byte[] ids;
    }

    private ChunkManager chunkManager;
    
    private Queue<ChunkCreationParameters> creationQueue = new Queue<ChunkCreationParameters>();
    
    private Queue<Chunk> renderQueue = new Queue<Chunk>();
    
    private Queue<Chunk> relationQueue = new Queue<Chunk>();
	
    void Start()
    {
        chunkManager = GetComponent<ChunkManager>();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(16f, Screen.height - 96f, 256f, 32f), "Creation Queue: " + creationQueue.Count);
        GUI.Label(new Rect(16f, Screen.height - 64f, 256f, 32f), "Render Queue: " + renderQueue.Count);
        GUI.Label(new Rect(16f, Screen.height - 32f, 256f, 32f), "Relations Queue: " + relationQueue.Count);
    }

	// Update is called once per frame
	void Update () {
        lock(creationQueue)
        {
            int countCreation = 0;
            while(creationQueue.Count > 0 && countCreation < 1)
            {
                countCreation++;
                ChunkCreationParameters para = creationQueue.Dequeue();
                chunkManager.CreateChunk(para.x, para.y, para.z, para.ids);
            }
        }

        lock(relationQueue)
        {
            int countRelations = 0;
            while(relationQueue.Count > 0 && countRelations < 64)
            {
                countRelations++;
                Chunk c = relationQueue.Dequeue();
                if (c.GetChunkDistanceToPlayer() > Chunk.renderDistanceSquared) continue;
                countRelations++;
                chunkManager.UpdateRelations(c);
            }
        }

		lock(renderQueue)
        {
            int countRender = 0;
            while (renderQueue.Count > 0 && countRender < 32)
            {
                Chunk c = renderQueue.Dequeue();
                if (c.GetChunkDistanceToPlayer() > Chunk.renderDistanceSquared) continue;
                countRender++;
                c.RenderMesh();
            }
        }
	}

    public void QueueCreation(int x, int y, int z, byte[] ids, byte[] data)
    {
        ChunkCreationParameters c = new ChunkCreationParameters()
        {
            x = x,
            y = y,
            z = z,
            ids = ids
        };
        lock (creationQueue)
        {
            creationQueue.Enqueue(c);
        }
    }

    public void QueueRelations(Chunk c)
    {
        lock(relationQueue)
        {
            relationQueue.Enqueue(c);
        }
    }

    public void QueueRender(Chunk c)
    {
        lock(renderQueue)
        {
            renderQueue.Enqueue(c);
        }
    }
}
