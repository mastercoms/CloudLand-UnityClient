using Google.Protobuf;

using Org.Dragonet.Cloudland.Net.Protocol;

namespace CloudLand.Networking.Handlers
{
    class ServerCursorItemHandler : MessageHandler
    {
        public void handle(CloudLandClient client, IMessage messageReceived)
        {
            ServerCursorItemMessage msg = (ServerCursorItemMessage)messageReceived;
            if (msg.Item.Id == 0 && msg.Item.Meta == 0 && msg.Item.Count == 0)
            {
                Loom.QueueOnMainThread(() => WindowManager.INSTANCE.cursorItem = null);
            }
            else
            {
                Loom.QueueOnMainThread(() => WindowManager.INSTANCE.cursorItem = msg.Item);
            }
        }
    }
}
