using GO2018_MKS_MessageLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GO2018_MKS_Server
{
    class Program
    {
        bool runFlag = true;

        List<TcpClient> connectedTcpClients = new List<TcpClient>();

        Stack<KeyValuePair<TcpClient, GenericMessage>> StackOfPendingClientMessages = new Stack<KeyValuePair<TcpClient, GenericMessage>>();

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

        public void Run()
        {
            Console.WriteLine("GO2018 MKS Server");

            Thread tcpListenerThread = new Thread(new ThreadStart(TcpHandlerThreadProc));
            tcpListenerThread.Start();

            PrintHelpMessage();

            while (runFlag)
            {
                string inputLine = Console.ReadLine();
                switch (inputLine.Trim())
                {
                    case "quit":
                    case "exit":
                        {
                            runFlag = false;
                        }
                        break;
                    case "help":
                        {
                            PrintHelpMessage();
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown command: " + inputLine);
                        break;
                }
            }

            tcpListenerThread.Join();
        }

        void PrintHelpMessage()
        {
            Console.WriteLine("Commands (help, quit, exit)");
        }

        private void TcpHandlerThreadProc()
        {
            int port = 13000;
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();

            while (runFlag)
            {
                AcceptClients(server);
                ReadClientMessages();
                WriteClientMessages();
                ClearLostClients();

                // TEMP Break for a bit
                Thread.Sleep(200);
            }

            server.Stop();

            foreach (TcpClient client in connectedTcpClients)
            {
                if (client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    stream.Close();

                    client.Close();
                }
            }
        }

        // Connect players
        private void AcceptClients(TcpListener server)
        {
            while (server.Pending())
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected: " + client.Client.Handle.ToString());

                client.LingerState = new LingerOption(false, 0);
                client.NoDelay = true;

                connectedTcpClients.Add(client);

                WelcomeMessage welcomeMessage = new WelcomeMessage();
                StackOfPendingClientMessages.Push(new KeyValuePair<TcpClient, GenericMessage>(client, welcomeMessage));
            }
        }

        // Read player messages
        private void ReadClientMessages()
        {
            foreach(TcpClient client in connectedTcpClients)
            {
                if (!VerifyClientConnection(client))
                {
                    continue;
                }

                int availableClientData = client.Available;
                if (availableClientData > 0)
                {
                    NetworkStream stream = client.GetStream();

                    byte[] readBuffer = new byte[availableClientData];
                    int bytesRead = stream.Read(readBuffer, 0, availableClientData);

                    string messageText = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);
                    // TEMP DEBUG
                    Console.WriteLine(client.Client.Handle.ToString() + ": " + messageText);

                    // TODO  Cut incoming mesage into portion for JSON desierialization

                    GenericMessage genericMessage = JsonConvert.DeserializeObject<GenericMessage>(messageText);
                    switch (genericMessage.Type)
                    {
                        case MessageType.generic:
                            {
                                Console.WriteLine("Generic TCP message received: " + client.Client.Handle.ToString());
                            }
                            break;

                        case MessageType.login:
                            {
                                Console.WriteLine("Login TCP message received: " + client.Client.Handle.ToString());

                                //LoginAnswerMessage loginAnswer = new LoginAnswerMessage(true, "OK");
                                //LoginAnswerMessage answer = new LoginAnswerMessage(false, "Player credentials already used in session");

                                //KeyValuePair<TcpClient, GenericMessage> stackEntry = new KeyValuePair<TcpClient, GenericMessage>(client, loginAnswer);
                                //StackOfPendingClientMessages.Push(stackEntry);
                            }
                            break;
                        case MessageType.logout:
                            {
                                Console.WriteLine("Logout TCP message received: " + client.Client.Handle.ToString());

                                // TEMP WORKAROUND - Close client
                                stream.Close();
                                client.Close();
                            }
                            break;

                        default:
                            {
                                Console.WriteLine("Unhandled TCP message received: " + client.Client.Handle.ToString());
                            }
                            break;
                    }
                }
            }
        }

        // Write player messages
        private void WriteClientMessages()
        {
            while (StackOfPendingClientMessages.Count > 0)
            {
                KeyValuePair<TcpClient, GenericMessage> entry = StackOfPendingClientMessages.Pop();

                if (!VerifyClientConnection(entry.Key))
                {
                    continue;
                }

                string message = JsonConvert.SerializeObject(entry.Value);
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                NetworkStream stream = entry.Key.GetStream();
                stream.Write(data, 0, data.Length);
            }
        }

        private void ClearLostClients()
        {
            for (int clientIndex = connectedTcpClients.Count - 1; clientIndex >= 0; clientIndex--)
            {
                TcpClient client = connectedTcpClients[clientIndex];
                if(VerifyClientConnection(client))
                {
                    continue;
                }

                Console.WriteLine("Client lost, clearing: " + client.Client.Handle.ToString());

                StorePlayerResults(client);

                connectedTcpClients.Remove(client);

                StackOfPendingClientMessages = new Stack<KeyValuePair<TcpClient, GenericMessage>>();

                KeyValuePair<TcpClient, GenericMessage>[] stackPairArray = StackOfPendingClientMessages.ToArray();
                for (int stackIndex = 0; stackIndex < stackPairArray.Length; stackIndex++)
                {
                    KeyValuePair<TcpClient, GenericMessage> stackEntry = stackPairArray[stackIndex];
                    if(client != stackEntry.Key)
                    {
                        StackOfPendingClientMessages.Push(stackEntry);
                    }
                }
            }
        }

        private bool VerifyClientConnection(TcpClient client)
        {
            return client.Client.Connected;
        }

        private void StorePlayerResults(TcpClient client)
        {
            // TODO - Store persistent results of client brefore removing
        }
    }
}
