using UnityEngine;
using Org.Dragonet.Cloudland.Net.Protocol;

public class WindowManager : MonoBehaviour {

    private static WindowManager _INSTANCE;

    public SerializedItem cursorItem;  // Item on cursor

    public static WindowManager INSTANCE
    {
        get
        {
            return _INSTANCE;
        }
    }

    public GameObject windowPrefab;
    public GameObject inventoryWindowPrefab;

	// Use this for initialization
	void Start () {
        _INSTANCE = this;
	}

    void LateUpdate()
    {
        if(cursorItem != null && transform.childCount == 0)
        {
            cursorItem = null;
        }

        bool currentStatus = ClientComponent.INSTANCE.playerObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled;
        if(transform.childCount > 0)
        {
            if(currentStatus == true)
            {
                ClientComponent.INSTANCE.playerObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        } else
        {
            if(currentStatus == false)
            {
                ClientComponent.INSTANCE.playerObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void OnGUI()
    {
        if (cursorItem == null || cursorItem.Id == 0) return;
        GUI.DrawTexture(new Rect(Input.mousePosition.x + 24, Screen.height - Input.mousePosition.y + 24, 24, 24), Inventory.getItemTexture(cursorItem.Id, cursorItem.Meta));
        GUI.Label(new Rect(Input.mousePosition.x + 32, Screen.height - Input.mousePosition.y + 32, 24, 20), cursorItem.Count.ToString());
    }

    public GameWindow getWindowById(uint id)
    {
        string key = "Window-" + id;
        Transform obj = transform.FindChild(key);
        if (obj == null) return null;
        return obj.GetComponent<GameWindow>();
    }

    public void createWindow(ServerWindowOpenMessage msg)
    {
        GameObject obj = GameObject.Instantiate(windowPrefab);
        obj.transform.name = "Window-" + msg.WindowId;
        obj.transform.parent = transform;
        obj.GetComponent<GameWindow>().Initialize(msg);
    }

    public GameWindow getInventoryWindow()
    {
        return getWindowById(0);
    }

    public void openInventoryWindow()
    {
        if (getInventoryWindow() != null) return;
        GameObject obj = GameObject.Instantiate(inventoryWindowPrefab);
        obj.transform.name = "Window-0";
        obj.transform.parent = transform;
        obj.GetComponent<GameWindow>().Initialize(null);
    }
}
