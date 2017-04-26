using UnityEngine;
using Google.Protobuf;

using Org.Dragonet.Cloudland.Net.Protocol;

namespace CloudLand.Networking.Handlers
{
    class ServerRemoveEntityHandler : MessageHandler
    {
        public void handle(CloudLandClient client, IMessage messageReceived)
        {
            ServerRemoveEntityMessage message = (ServerRemoveEntityMessage)messageReceived;
            Loom.QueueOnMainThread(() =>
            {
                Transform t = client.getClientComponent().entitiesParent.Find("entity|" + message.EntityId);
                if(t != null)
                {
                    GameObject.Destroy(t.gameObject);
                }
            });
        }
    }
}
