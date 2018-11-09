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
            Console.WriteLine("Commands [help, quit, exit]");
        }

        private void TcpHandlerThreadProc()
        {
            int port = 13000;
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();

            while (runFlag)
            {
                while(server.Pending())
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected: " + client.Client.Handle.ToString());

                    connectedTcpClients.Add(client);

                    WelcomeMessage welcomeMessage = new WelcomeMessage();
                    string message = JsonConvert.SerializeObject(welcomeMessage);
                    Byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                    NetworkStream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                }

                foreach (TcpClient client in connectedTcpClients)
                {
                    int availableClientData = client.Available;
                    if (availableClientData > 0)
                    {
                        NetworkStream stream = client.GetStream();

                        byte[] readBuffer = new byte[availableClientData];
                        int bytesRead = stream.Read(readBuffer, 0, availableClientData);

                        string messageText = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);
                        //Console.WriteLine(client.Client.Handle.ToString() + ": " + messageText);

                        GenericMessage genericMessage = JsonConvert.DeserializeObject< GenericMessage>(messageText);
                        switch(genericMessage.Type)
                        {
                            case MessageType.generic:
                                {
                                    Console.WriteLine("Generic TCP message received: " + client.Client.Handle.ToString());
                                }
                                break;

                            case MessageType.login:
                                {
                                    Console.WriteLine("Login TCP message received: " + client.Client.Handle.ToString());
                                }
                                break;
                            case MessageType.logout:
                                {
                                    Console.WriteLine("Logout TCP message received: " + client.Client.Handle.ToString());
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

                Thread.Sleep(200);
            }

            server.Stop();

            foreach (TcpClient client in connectedTcpClients)
            {
                client.Close();
            }
        }
    }

}
