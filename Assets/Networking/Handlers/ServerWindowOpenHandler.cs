using Google.Protobuf;

namespace CloudLand.Networking.Handlers
{
    class ServerWindowOpenHandler : MessageHandler
    {
        public void handle(CloudLandClient client, IMessage messageReceived)
        {
            Org.Dragonet.Cloudland.Net.Protocol.ServerWindowOpenMessage msg = (Org.Dragonet.Cloudland.Net.Protocol.ServerWindowOpenMessage)messageReceived;

            Loom.QueueOnMainThread(() => WindowManager.INSTANCE.createWindow(msg));
        }
    }
}
