using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            this.LoadRedirectionRules(redirectionMatrixPath);
            
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, portNumber);

            this.serverSocket.Bind(iPEndPoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                // start thread for each accepted connection.
                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newThread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSocket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            int receivedLength;
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] dataRecieved = new byte[1024*1024];
                    receivedLength = clientSocket.Receive(dataRecieved);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        //Console.WriteLine("Client: {0} ended the connection",
                        //clientSocket.RemoteEndPoint);

                        break;
                    }

                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(dataRecieved,0,receivedLength));
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client
                    clientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }
            // TODO: close client socket()
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
        Response HandleRequest(Request request)
        {
            string content;
            string redirection = string.Empty;
            StatusCode status_code = StatusCode.OK;
            try
            {
                
                if (!(request.ParseRequest()))
                {
                    status_code = StatusCode.BadRequest;;
                    content = File.ReadAllText(Path.Combine(Configuration.RootPath, Configuration.BadRequestDefaultPageName));
                    return new Response(StatusCode.BadRequest, "text/html", content, redirection);
                }

                string physicalPath = Path.Combine(Configuration.RootPath, request.relativeURI);

                // check for redirect
                redirection = GetRedirectionPagePathIFExist(request.relativeURI);
                if (redirection != string.Empty)
                {
                    status_code = StatusCode.Redirect;
                    physicalPath = Path.Combine(Configuration.RootPath, Configuration.RedirectionDefaultPageName);
                }

               
                if (!File.Exists(physicalPath))
                {
                    
                    
                        status_code = StatusCode.NotFound;
                        physicalPath = Path.Combine(Configuration.RootPath, Configuration.NotFoundDefaultPageName);
                    
                }

               
                content = LoadDefaultPage(physicalPath);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                
                status_code = StatusCode.InternalServerError;
                content = File.ReadAllText(Path.Combine(Configuration.RootPath, Configuration.InternalErrorDefaultPageName));
            }
           
            return new Response(status_code, "text/html", content, redirection);
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
           
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RedirectionRules[relativePath];
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
          
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(defaultPageName))
            {
                Logger.LogException(new FileNotFoundException());
                return string.Empty;
            }
            else
            {
                return File.ReadAllText(defaultPageName);
            }
            // else read file and return its content

        }

        private void LoadRedirectionRules(string filePath)

        {
            Configuration.RedirectionRules = new Dictionary<string, string>();
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                string[] lines = File.ReadLines(filePath).ToArray();
                foreach (string line in lines)
                {
                    string[] words = line.Split(',');
                    // then fill Configuration.RedirectionRules dictionary 
                    Configuration.RedirectionRules.Add(words[0], words[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
          
            }
        }
    }
}