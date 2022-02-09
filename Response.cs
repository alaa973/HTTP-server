using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            int contentLength = content.Length;
            string contentLengthString = contentLength.ToString();
            DateTime date = DateTime.Now;
            string dateString = date.ToString();

            headerLines.Add("Content-Type: " + contentType);
            headerLines.Add("Content-Length: " + contentLengthString);
            headerLines.Add("Date: " + dateString);
            if (redirectoinPath != "")
            {
                headerLines.Add("Location: " + redirectoinPath);
            }
            // TODO: Create the request string
            string statusLine = GetStatusLine(code);
            string headerLine = "";
            for (int i = 0; i < headerLines.Count; i++)
            {
                headerLine += headerLines[i] + "\r\n";
            }
            headerLine += "\r\n";
            responseString = statusLine + headerLine + content;

        }

        private string GetStatusLine(StatusCode code)
        {
            string statusLine;
            string message = string.Empty;
            switch (code)
            {
                case StatusCode.OK:
                    message = "OK";
                    break;
                case StatusCode.Redirect:
                    message = "Moved Permanently";
                    break;
                case StatusCode.BadRequest:
                    message = "Bad Request";
                    break;
                case StatusCode.NotFound:
                    message = "Not Found";
                    break;
                case StatusCode.InternalServerError:
                    message = "Internal Server Error";
                    break;
            }

            statusLine = Configuration.ServerHTTPVersion+" " + (int)code +" "+ message + "\r\n";

            return statusLine;
        }

    }
}
