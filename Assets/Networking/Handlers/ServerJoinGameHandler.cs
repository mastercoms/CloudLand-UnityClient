using Google.Protobuf;
using Org.Dragonet.Cloudland.Net.Protocol;

namespace CloudLand.Networking.Handlers
{
    class ServerJoinGameHandler : MessageHandler
    {
        public void handle(CloudLandClient client, IMessage messageReceived)
        {
            ServerJoinGameMessage message = (ServerJoinGameMessage)messageReceived;

            Loom.QueueOnMainThread(() =>
            {
                client.getClientComponent().playerObject.transform.position = new UnityEngine.Vector3(message.X, message.Y, message.Z);
                client.getClientComponent().username = message.Username;
                client.getClientComponent().uuid = message.Uuid;
                client.getClientComponent().playerObject.SetActive(true);
                UnityEngine.Debug.Log("Player location updated to: " + string.Format("({0}, {1}, {2})", message.X, message.Y, message.Z));

                client.loggedIn = true;
            });
        }
    }
}
