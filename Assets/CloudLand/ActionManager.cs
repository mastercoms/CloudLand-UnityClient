using System;
using UnityEngine;

using Org.Dragonet.Cloudland.Net.Protocol;

public class ActionManager : MonoBehaviour {

    public Animation handAnimation;

    public GameObject selection;
    public PlayerInventory inventory;

    private ClientComponent client;

    private Texture2D[] breakStageTextures;
    private int displayingStageTexture = -1;

    private DateTime breakingTime;
    private Block breakingBlock; // null if not breaking
    private int breakingX;
    private int breakingY;
    private int breakingZ;
    private bool c;

    // Use this for initialization
    void Start () {
        client = GetComponent<ClientComponent>();
        inventory = GetComponent<PlayerInventory>();

        breakStageTextures = new Texture2D[10];
        for(int i = 0; i < 10; i++)
        {
            breakStageTextures[i] = (Texture2D)Resources.Load("Images/Block_Break_Animation/destroy_stage_" + i);
        }
    }

    const int crossSize = 16;

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - (crossSize / 2), Screen.height / 2 - (crossSize / 2), crossSize, crossSize), "+");
    }

	// Update is called once per frame
	void LateUpdate () {

        if (WindowManager.INSTANCE.transform.childCount > 0) return;

        Ray r = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit h;
        bool hit = Physics.Raycast(r, out h);
        if(!hit || Vector3.Distance(h.point, ClientComponent.INSTANCE.playerObject.transform.position) > 10f)
        {
            hideSelection();
            return;
        }

        Vector3 b = h.point - (0.5f * h.normal);
        int x = Mathf.FloorToInt(b.x);
        int y = Mathf.FloorToInt(b.y);
        int z = Mathf.FloorToInt(b.z);

        showSelection(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));

        if (breakingBlock != null)
        {
            if (x != breakingX || y != breakingY || z != breakingZ)
            {
                cancelBreaking();

                if (Input.GetMouseButton(1)) c = true;
            } else
            {
                long span = (long)((DateTime.Now - breakingTime).TotalMilliseconds);
                if(span > breakingBlock.GetBreakTime(inventory.GetSelectedItem()))
                {
                    // Should be finished
                    client.chunkManager.setBlockAt(x, y, z, 0, 0);
                    ClientRemoveBlockMessage removeBlockMessage = new ClientRemoveBlockMessage();
                    removeBlockMessage.X = x;
                    removeBlockMessage.Y = y;
                    removeBlockMessage.Z = z;
                    client.GetClient().sendMessage(removeBlockMessage);

                    selection.SetActive(false);
                    selection.GetComponent<MeshRenderer>().material.mainTexture = null;

                    if(Input.GetMouseButtonDown(1))
                    {
                        c = true;
                    } else
                    {
                        c = false;
                    }
                } else
                {
                    handAnimation.Play("hand_breaking");

                    int stageTextureId = (int)(((DateTime.Now - breakingTime).TotalMilliseconds / breakingBlock.GetBreakTime(inventory.GetSelectedItem())) * 10f);
                    if (stageTextureId > 9) stageTextureId = 9;
                    if (!selection.activeSelf) selection.SetActive(true);
                    if(stageTextureId != displayingStageTexture)
                    {
                        displayingStageTexture = stageTextureId;
                        selection.GetComponent<MeshRenderer>().material.mainTexture = breakStageTextures[displayingStageTexture];
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1) || c)
        {
            cancelBreaking();
            int f = client.chunkManager.getFullBlockAt(x, y, z);
            int id = f >> 16;
            if(id == 0)
            {
                cancelBreaking();
                return;
            }
            c = false;
            // int meta = f & 0xFFFF;
            breakingBlock = Block.prototypes[id];
            breakingX = x;
            breakingY = y;
            breakingZ = z;
            breakingTime = DateTime.Now;
            // Debug.Log("START BREAKING <" + breakingBlock.GetType().Name + ">");

            ClientActionMessage startBreakingMessage = new ClientActionMessage();
            startBreakingMessage.Action = ClientActionMessage.Types.ActionType.StartBreak;
            startBreakingMessage.BlockX = x;
            startBreakingMessage.BlockY = y;
            startBreakingMessage.BlockZ = z;
            client.GetClient().sendMessage(startBreakingMessage);
        }

        if(Input.GetMouseButtonUp(0))
        {
            cancelBreaking();
            Vector3 adding = h.point + (0.5f * h.normal);
            SerializedItem holding = inventory.items[inventory.currentSelection];

            touchBlock(holding, x, y, z, h.normal);

            if (holding != null)
            {
                if(holding.Id < Block.prototypes.Length)
                {
                    int addingX = Mathf.FloorToInt(adding.x);
                    int addingY = Mathf.FloorToInt(adding.y);
                    int addingZ = Mathf.FloorToInt(adding.z);
                    Debug.Log(string.Format("ADDING AT ({0},{1},{2})", addingX, addingY, addingZ));
                    // Place block
                    client.chunkManager.setBlockAt(addingX, addingY, addingZ, holding.Id, holding.Meta);

                    holding.Count--;
                    if(holding.Count <= 0)
                    {
                        inventory.items[inventory.currentSelection] = null;
                    }

                    handAnimation.Play("hand_place");
                }
            }
        }

        if(Input.GetMouseButtonUp(1))
        {
            cancelBreaking();
            c = false;
        }
    }

    private void touchBlock(SerializedItem holding, int x, int y, int z, Vector3 normal)
    {
        // public enum Direction { north, east, south, west, up, down };
        //                         z+   , x+  , z-   , x-  , y+, y-
        int dir = 0;
        if (normal.Equals(Vector3.forward))
            dir = 0;
        else if (normal.Equals(Vector3.back))
            dir = 2;
        else if (normal.Equals(Vector3.right))
            dir = 1;
        else if (normal.Equals(Vector3.left))
            dir = 3;
        else if (normal.Equals(Vector3.up))
            dir = 4;
        else if (normal.Equals(Vector3.down))
            dir = 5;
        ClientUseItemMessage use = new ClientUseItemMessage();
        use.HotbarIndex = (uint)(inventory.currentSelection & 0xF);
        use.Direction = (uint)(dir & 0xF);
        use.BlockX = x;
        use.BlockY = y;
        use.BlockZ = z;
        client.GetClient().sendMessage(use);
    }

    private void cancelBreaking()
    {
        if (breakingBlock == null) return;
        // Cancelled breaking
        ClientActionMessage a = new ClientActionMessage();
        a.Action = ClientActionMessage.Types.ActionType.CancelBreak;
        client.GetClient().sendMessage(a);
        // Debug.Log("Cancelled breaking");
        breakingBlock = null;

        displayingStageTexture = -1;
        selection.SetActive(false);
        selection.GetComponent<MeshRenderer>().material.mainTexture = null;
    }


    private void hideSelection()
    {
        selection.SetActive(false);
    }

    private void showSelection(Vector3 p)
    {
        if (selection.transform.position.Equals(p)) return;
        selection.transform.position = p;
        // selection.SetActive(true);
    }
}
