using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Dragonet.Cloudland.Net.Protocol;
using CloudLand.Networking.Handlers;

namespace CloudLand
{
    class MessageRegister
    {

        private Dictionary<UInt32, MessageParser> messageMap = new Dictionary<UInt32, MessageParser>();
        private Dictionary<Type, UInt32> idMap = new Dictionary<Type, UInt32>();
        private Dictionary<UInt32, MessageHandler> handlerMap = new Dictionary<uint, MessageHandler>();
        
        public MessageRegister()
        {
            registerProtocol();
        }

        private void registerProtocol()
        {
            /* ==== SERVER ==== */
            // Initialization
            register(0xAABBCCDD, typeof(ClientHandshakeMessage), ClientHandshakeMessage.Parser);
            register(0x11223344, typeof(ServerHandshakeMessage), ServerHandshakeMessage.Parser, new ServerHandshakeHandler());
            register(0xAF000000, typeof(ClientAuthenticateMessage), ClientAuthenticateMessage.Parser);
            register(0xAF000001, typeof(ServerAuthenticateResultMessage), ServerAuthenticateResultMessage.Parser);
            register(0xFF000000, typeof(ServerDisconnectMessage), ServerDisconnectMessage.Parser);

            register(0xB0000000, typeof(ServerJoinGameMessage), ServerJoinGameMessage.Parser, new ServerJoinGameHandler());
            register(0xB0000001, typeof(ServerUpdateEnvironmentMessage), ServerUpdateEnvironmentMessage.Parser);
            register(0xB0000002, typeof(ServerChunkMessage), ServerChunkMessage.Parser, new ServerChunkHandler());
            register(0xB0000003, typeof(ServerUpdateBlockMessage), ServerUpdateBlockMessage.Parser, new ServerUpdateBlockHandler());
            register(0xB0000004, typeof(ServerUpdateBlockBatchMessage), ServerUpdateBlockBatchMessage.Parser, new ServerUpdateBlockHandler());
            register(0xB0000005, typeof(ServerChatMessage), ServerChatMessage.Parser);
            register(0xB0000006, typeof(ServerUpdatePlayerPositionMessage), ServerUpdatePlayerPositionMessage.Parser);

            // Entites
            register(0xBE000001, typeof(ServerAddEntityMessage), ServerAddEntityMessage.Parser, new ServerAddEntityHandler());
            register(0xBE000002, typeof(ServerEntityUpdateMessage), ServerEntityUpdateMessage.Parser, new ServerEntityUpdateHandler());
            register(0xBE000003, typeof(ServerRemoveEntityMessage), ServerRemoveEntityMessage.Parser, new ServerRemoveEntityHandler());
            register(0xBE0000FF, typeof(ServerClearEntitiesMessage), ServerClearEntitiesMessage.Parser);

            // Window
            register(0xBA000000, typeof(ServerWindowOpenMessage), ServerWindowOpenMessage.Parser, new ServerWindowOpenHandler());
            register(0xBA000001, typeof(ServerWindowCloseMessage), ServerWindowCloseMessage.Parser);
            register(0xBA000002, typeof(ServerUpdateWindowMessage), ServerUpdateWindowMessage.Parser, new ServerUpdateWindowHandler());
            register(0xBA000003, typeof(ServerUpdateWindowElementMessage), ServerUpdateWindowElementMessage.Parser, new ServerUpdateWindowElementHandler());
            register(0xBA000004, typeof(ServerCursorItemMessage), ServerCursorItemMessage.Parser, new ServerCursorItemHandler());


            /* ==== CLIENT ==== */
            register(0xE0000000, typeof(ClientChatMessage));
            register(0xE0000001, typeof(ClientMovementMessage));
            register(0xE0000002, typeof(ClientActionMessage));
            register(0xE0000003, typeof(ClientRemoveBlockMessage));
            register(0xE0000004, typeof(ClientUseItemMessage));

            // Window
            register(0xE1000000, typeof(ClientHotbarSelectionMessage));
            register(0xE1000001, typeof(ClientPickUpItemMessage));
            register(0xE1FF0000, typeof(ClientWindowInteractMessage)); //TODO
            register(0xE1FEFFFF, typeof(ClientWindowCloseMessage));
        }

        public void register(UInt32 id, Type messageType)
        {
            register(id, messageType, null, null);
        }

        public void register(UInt32 id, Type messageType, MessageParser parser)
        {
            register(id, messageType, parser, null);
        }

        public void register(UInt32 id, Type messageType, MessageParser parser, MessageHandler handler)
        {
            messageMap.Add(id, parser);
            idMap.Add(messageType, id);
            if(handler != null)
            {
                handlerMap.Add(id, handler);
            }
        }

        public MessageParser getParser(UInt32 id)
        {
            MessageParser parser;
            messageMap.TryGetValue(id, out parser);
            return parser;
        }

        public UInt32 getId(IMessage message)
        {
            return getId(message.GetType());
        }

        public UInt32 getId(Type type) 
        {
            if (!idMap.ContainsKey(type)) throw new Exception("Invalid message instance! ");
            UInt32 id;
            idMap.TryGetValue(type, out id);
            return id;
        }

        public MessageHandler getMessageHandler(UInt32 id)
        {
            MessageHandler handler;
            handlerMap.TryGetValue(id, out handler);
            return handler;
        }
    }
}
