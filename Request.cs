using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        //string requestLine;
        RequestMethod method;
        //string method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        //string httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content,
        /// returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //TODO: parse the receivedRequest using the \r\n delimeter   
            contentLines = requestString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (contentLines.Length < 3)
                return false;
            // Parse Request line
            // Validate blank line exists
            // Load header lines into HeaderLines dictionary
   
            return (ParseRequestLine()&& ValidateBlankLine()&& LoadHeaderLines());
        }
        private bool ParseRequestLine()
        {
            //throw new NotImplementedException();
            requestLines = contentLines[0].Split(' ');
            if (requestLines.Length < 3)
            {
                return false;
            }
            if (requestLines[0] == "GET")
            {
                method = RequestMethod.GET;
            }
            else
                return false;
            if (!ValidateIsURI(requestLines[1]))
                return false;
            relativeURI = requestLines[1].Substring(1);
            if (requestLines[2] == "HTTP/0.9")
                httpVersion = HTTPVersion.HTTP09;
            else if (requestLines[2] == "HTTP/1.0")
                httpVersion = HTTPVersion.HTTP10;
            else if (requestLines[2] == "HTTP/1.1")
                httpVersion = HTTPVersion.HTTP11;
            else
                return false;
            return true;
        }
        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            //throw new NotImplementedException();
            headerLines = new Dictionary<string, string>();
            for (int i = 1; i < contentLines.Length; i++)
            {
                if (!(contentLines[i].Equals("")))
                {
                    int separator = contentLines[i].IndexOf(':');
                    if (separator == -1)
                        return false;
                    String key = contentLines[i].Substring(0, separator);
                    int idx = separator + 1;
                    while ((idx < contentLines[i].Length) && (contentLines[i][idx] == ' '))
                    {
                        idx++;
                    }

                    string value = contentLines[i].Substring(idx, contentLines[i].Length - idx);
                    headerLines[key] = value;
                }
            }
            return true;
        }
        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
            if (!(contentLines[contentLines.Length - 1].Equals("\r\n")))
            {
                return true;
            }
            return false;
        }

    }
}
