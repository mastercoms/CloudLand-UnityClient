using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudLand
{
    public interface MessageHandler
    {
        void handle(CloudLandClient client, IMessage messageReceived);

    }
}
