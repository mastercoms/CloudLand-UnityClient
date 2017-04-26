using Google.Protobuf;

using Org.Dragonet.Cloudland.Net.Protocol;

namespace CloudLand.Networking.Handlers
{
    class ServerUpdateWindowElementHandler : MessageHandler
    {
        public void handle(CloudLandClient client, IMessage messageReceived)
        {
            ServerUpdateWindowElementMessage msg = (ServerUpdateWindowElementMessage)messageReceived;

            if(msg.WindowId == 0)
            {
                // TODO: handle inventory
                Loom.QueueOnMainThread(() => {
                    ClientComponent.INSTANCE.GetComponent<PlayerInventory>().updateFromMetadata(msg.ElementIndex, msg.NewElement.Value);
                });
                return;
            }

            GameWindow window = WindowManager.INSTANCE.getWindowById(msg.WindowId);
            if (window == null) return;
            Loom.QueueOnMainThread(() => window.updateElement(msg.ElementIndex, msg.NewElement));
        }
    }
}
