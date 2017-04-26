using Google.Protobuf;
using UnityEngine;
using Org.Dragonet.Cloudland.Net.Protocol;

namespace CloudLand.Networking.Handlers
{
    class ServerAddEntityHandler : MessageHandler
    {
        public void handle(CloudLandClient client, IMessage messageReceived)
        {
            ServerAddEntityMessage message = (ServerAddEntityMessage)messageReceived;
            Loom.QueueOnMainThread(() =>
            {
                if (client.getClientComponent().entitiesParent.Find("entity|" + message.EntityId) != null)
                {
                    GameObject.Destroy(client.getClientComponent().entitiesParent.Find("entity|" + message.EntityId).gameObject);
                }
                GameObject prefab = (GameObject)Resources.Load("Entities/" + message.EntityType.ToString());
                GameObject obj = GameObject.Instantiate(prefab, new Vector3((float)message.X, (float)message.Y, (float)message.Z), Quaternion.Euler(message.Pitch, message.Yaw, 0f), client.getClientComponent().entitiesParent);
                // An entity prefab MUST include a sub-class of Entity
                obj.GetComponent<Entity>().entityId = message.EntityId;
                obj.GetComponent<Entity>().meta = message.Meta;
                obj.transform.name = "entity|" + message.EntityId;
            });
        }
    }
}
