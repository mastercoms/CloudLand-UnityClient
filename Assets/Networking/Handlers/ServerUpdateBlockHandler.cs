using System;
using Google.Protobuf;

using Org.Dragonet.Cloudland.Net.Protocol;

namespace CloudLand.Networking.Handlers
{
    class ServerUpdateBlockHandler : MessageHandler
    {
        public void handle(CloudLandClient client, IMessage messageReceived)
        {
            if(messageReceived is ServerUpdateBlockMessage)
            {
                handleSingle(client, (ServerUpdateBlockMessage)messageReceived);
            } else
            {
                foreach(ServerUpdateBlockMessage single in ((ServerUpdateBlockBatchMessage)messageReceived).Records)
                {
                    handleSingle(client, single);
                }
            }
        }

        private void handleSingle(CloudLandClient client, ServerUpdateBlockMessage message)
        {
            Loom.QueueOnMainThread(() => client.getClientComponent().chunkManager.setBlockAt(message.X, message.Y, message.Z, (int)(message.Id & 0xFFFF), (int)(message.Meta & 0xFFFF)));
        }
    }
}
