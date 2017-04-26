using Org.Dragonet.Cloudland.Net.Protocol;
using Google.Protobuf;

namespace CloudLand.Networking.Handlers
{
    class ServerHandshakeHandler : MessageHandler
    { 
        public void handle(CloudLandClient client, IMessage messageReceived)
        {
            ServerHandshakeMessage message = (ServerHandshakeMessage)messageReceived;
            if(!message.Success)
            {
                UnityEngine.Debug.Log("Failed to handshake, reason: " + message.Message);
                return;
            }
            UnityEngine.Debug.Log("success handshaked! ");
            ClientAuthenticateMessage auth = new ClientAuthenticateMessage();
            auth.UserId = 0;
            auth.Token = "random token";
            client.sendMessage(auth);
        }
    }
}
