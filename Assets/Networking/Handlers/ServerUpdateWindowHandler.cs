using System;
using Org.Dragonet.Cloudland.Net.Protocol;
using Google.Protobuf;

namespace CloudLand.Networking.Handlers
{
    class ServerUpdateWindowHandler : MessageHandler
    {
        public void handle(CloudLandClient client, IMessage messageReceived)
        {
            ServerUpdateWindowMessage msg = (ServerUpdateWindowMessage)messageReceived;
            ServerUpdateWindowElementHandler hdl = new ServerUpdateWindowElementHandler();
            foreach (ServerUpdateWindowElementMessage rec in msg.Records)
            {
                hdl.handle(client, rec);
            }
        }
    }
}
