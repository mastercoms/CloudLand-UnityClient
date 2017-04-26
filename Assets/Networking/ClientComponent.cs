using UnityEngine;
using CloudLand;
using Org.Dragonet.Cloudland.Net.Protocol;

public class ClientComponent : MonoBehaviour {

    public static ClientComponent INSTANCE;

    private float updateTime;

    public GameObject playerObject;
    public ChunkManager chunkManager;

    public Transform entitiesParent;

    public Transform windowsParent;

    public string username;
    public string uuid;

    private CloudLandClient client;

    private Vector3 previousSent;
    private float previousYaw;
    private float previousPitch;

	// Use this for initialization
	void Start () {
        INSTANCE = this;

        GameObject obj = new GameObject();
        obj.name = "Loom Object";
        obj.AddComponent(typeof(Loom));

        Block.InitializePrototypes();

        client = new CloudLandClient(this);
        client.connect(LoginScreen.addr, 21098);
	}

    public CloudLandClient GetClient()
    {
        return client;
    }

    void OnGUI()
    {
        GUILayout.Label("Username: " + username);
        GUILayout.Label("UUID: " + uuid);
    }
	
	// Update is called once per frame
	void Update () {
        if (client == null || !client.loggedIn) return;
        updateTime += Time.deltaTime;
        if (updateTime < 0.1) return;
        if(!playerObject.transform.position.Equals(previousSent) || playerObject.transform.rotation.y != previousYaw || playerObject.transform.rotation.x != previousPitch)
        {
            ClientMovementMessage msg = new ClientMovementMessage();
            Vector3 savedPosition = playerObject.transform.position;
            msg.X = savedPosition.x;
            msg.Y = savedPosition.y;
            msg.Z = savedPosition.z;
            msg.Yaw = playerObject.transform.rotation.eulerAngles.y;
            msg.Pitch = playerObject.transform.FindChild("FirstPersonCharacter").rotation.eulerAngles.x;
            /*Debug.Log("Position and rotation in engine: " + string.Format("({0}, {1}, {2}) # {3}, {4}", savedPosition.x, savedPosition.y, savedPosition.z, playerObject.transform.rotation.y, playerObject.transform.rotation.x));
            Debug.Log("Updating position and rotation: " + string.Format("({0}, {1}, {2}) # {3}, {4}", msg.X, msg.Y, msg.Z, msg.Yaw, msg.Pitch));
            Debug.Log("========");*/
            client.sendMessage(msg);

            previousSent = savedPosition;
            previousYaw = playerObject.transform.localEulerAngles.y;
            previousPitch = playerObject.transform.localEulerAngles.x;
            updateTime = 0f;
        }
	}
}
