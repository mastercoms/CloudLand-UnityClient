using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Collections.Generic;

using UnityEngine;

using System.Net.Sockets;
using Google.Protobuf;
using Org.Dragonet.Cloudland.Net.Protocol;
using UnityEngine.SceneManagement;

namespace CloudLand
{
    public class CloudLandClient
    {
        private ClientComponent clientComponent;

        private TcpClient socket;
        private bool connected;
        private NetworkStream networkStream;
        public byte[] buffer;
        private ByteListStream buffers = new ByteListStream();
        private Boolean readingHeader = true;
        private int headerMessageId;
        private int headerMessageLength;


        private MessageRegister messageRegister = new MessageRegister();

        public bool loggedIn;

        public Queue<byte[]> sendQueue = new Queue<byte[]>();

        public CloudLandClient(ClientComponent clientComponent)
        {
            this.clientComponent = clientComponent;
        }

        public void connect(String ip, int port)
        {
            socket = new TcpClient();
            socket.NoDelay = true;
            socket.ReceiveBufferSize = 65535;
            socket.SendBufferSize = 65535;
            socket.BeginConnect(ip, port, onConnect, socket);
        }

        private void onConnect(IAsyncResult result)
        {
            socket.EndConnect(result);
            connected = true;

            networkStream = new NetworkStream(socket.Client);

            ClientHandshakeMessage handshake = new ClientHandshakeMessage();
            handshake.ClientVersion = Versioning.NETWORK_VERSION;
            handshake.RenderDistance = (uint)LoginScreen.rd;
            sendMessage(handshake);
            UnityEngine.Debug.Log("Handshake sent! ");

            buffer = new byte[4096];
            networkStream.BeginRead(buffer, 0, 4096, receiveCallback, socket);
        }

        public void receiveCallback(IAsyncResult ar)
        {
            try
            {
                //UnityEngine.Debug.Log("* 1");
                int bytesRead = networkStream.EndRead(ar);
                if (bytesRead <= 0)
                {
                    onDisconnect();
                    return;
                }
                buffers.Add(new ArraySegment<byte>(buffer, 0, bytesRead));
                //UnityEngine.Debug.Log("BYTES READ=" + bytesRead);
                int total = buffers.available();
                //UnityEngine.Debug.Log("ENTERING LOOP");
                while (true)
                {
                    //UnityEngine.Debug.Log("TOTAL = " + total);
                    if (total <= 0) break;
                    if (readingHeader)
                    {
                        //UnityEngine.Debug.Log("READING HEADER");
                        if (total < 8)
                        {
                            break;
                        }
                        else
                        {
                            byte[] header = buffers.getBytes(8);
                            headerMessageId = ((header[0] & 0xFF) << 24) | ((header[1] & 0xFF) << 16) | ((header[2] & 0xFF) << 8) | ((header[3] & 0xFF));
                            headerMessageLength = ((header[4] & 0xFF) << 24) | ((header[5] & 0xFF) << 16) | ((header[6] & 0xFF) << 8) | ((header[7] & 0xFF));
                            //UnityEngine.Debug.Log("FOUND HEADER, ID=" + headerMessageId + ", LEN=" + headerMessageLength);
                            readingHeader = false;
                            if (total == 8) break;
                            total -= 8;
                            //UnityEngine.Debug.Log("TOTAL AFTER HEADER = " + total);
                        }
                    }
                    if (total >= headerMessageLength)
                    {
                        byte[] messageData = buffers.getBytes(headerMessageLength);
                        total -= headerMessageLength;
                        messageReceived((uint)headerMessageId, messageData);
                        readingHeader = true;
                        // Repeat
                    }
                    else
                    {
                        break;
                    }
                }
                if (!socket.Client.Connected)
                {
                    onDisconnect();
                    return;
                }
                buffer = new byte[4096];
                networkStream.BeginRead(buffer, 0, 4096, receiveCallback, socket);
                //UnityEngine.Debug.Log("EXIT LOOP");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("receiveCallback() error: " + e.Message + ", stack: " + e.StackTrace);
            }
        }

        private void onDisconnect()
        {
            Debug.Log("DISCONNECTED! ");
            Loom.QueueOnMainThread(() => SceneManager.LoadScene(0));
        }


        private void messageReceived(uint id, byte[] data)
        {
            MessageParser parser = messageRegister.getParser(id);

            IMessage message;
            if (data.Length > 0)
            {
                // Decompress
                MemoryStream mem = new MemoryStream();
                GZipStream inp = new GZipStream(new MemoryStream(data), CompressionMode.Decompress);
                byte[] buffer = new byte[4096];
                int rlen;
                while ((rlen = inp.Read(buffer, 0, 4096)) > 0)
                {
                    mem.Write(buffer, 0, rlen);
                }
                byte[] decompressedData = mem.ToArray();
                //UnityEngine.Debug.Log("inflated/received = " + decompressedData.Length + "/" + data.Length);
                message = parser.ParseFrom(decompressedData);
            }
            else
            {
                message = parser.ParseFrom(data);
            }

            //UnityEngine.Debug.Log("Received message [" + message.GetType().Name + "]");

            MessageHandler handler = messageRegister.getMessageHandler(id);
            if (handler != null)
            {
                handler.handle(this, message);
            }
        }

        public void sendMessage(IMessage message)
        {
            if (socket == null || !socket.Connected || !connected || message == null) return;
            uint id = messageRegister.getId(message);
            byte[] data = message.ToByteArray();

            MemoryStream encoded = new MemoryStream();
            encoded.Write(new byte[] {
                (byte)(id >> 24 & 0xFF),
                (byte)(id >> 16 & 0xFF),
                (byte)(id >> 8 & 0xFF),
                (byte)(id & 0xFF)
            }, 0, 4);

            if (data.Length > 0)
            {
                MemoryStream compressed = new MemoryStream();
                GZipStream z = new GZipStream(compressed, CompressionMode.Compress);
                z.Write(data, 0, data.Length);
                z.Close();
                byte[] compressedData = compressed.ToArray();
                int len = compressedData.Length;
                encoded.Write(new byte[] {
                    (byte)(len >> 24 & 0xFF),
                    (byte)(len >> 16 & 0xFF),
                    (byte)(len >> 8 & 0xFF),
                    (byte)(len & 0xFF)
                    }, 0, 4);
                encoded.Write(compressedData, 0, compressedData.Length);
            }
            else
            {
                encoded.Write(new byte[4], 0, 4);
            }

            byte[] s = encoded.ToArray();

            // socket.SendAsync(encoded.ToArray(), (v) => { });
            // socket.Send(encoded.ToArray());
            networkStream.Write(s, 0, s.Length);
            networkStream.Flush();
        }

        public ClientComponent getClientComponent()
        {
            return clientComponent;
        }
    }
}
