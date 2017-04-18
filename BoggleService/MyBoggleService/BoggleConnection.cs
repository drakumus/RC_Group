using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using static System.Net.HttpStatusCode;
using System.Text.RegularExpressions;
using System.Threading;

namespace Boggle
{
    class BoggleConnection
    {
        // Incoming/outgoing is UTF8-encoded.  This is a multi-byte encoding.  The first 128 Unicode characters
        // (which corresponds to the old ASCII character set and contains the common keyboard characters) are
        // encoded into a single byte.  The rest of the Unicode characters can take from 2 to 4 bytes to encode.
        private static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        
        // Buffer size for reading incoming bytes
        private const int BUFFER_SIZE = 1024;

        // The socket through which we communicate with the remote client
        private Socket socket;

        // Text that has been received from the client but not yet dealt with
        private StringBuilder incoming;

        // Text that needs to be sent to the client but which we have not yet started sending
        private StringBuilder outgoing;

        // For decoding incoming UTF8-encoded byte streams.
        private Decoder decoder = encoding.GetDecoder();

        // Buffers that will contain incoming bytes and characters
        private byte[] incomingBytes = new byte[BUFFER_SIZE];
        private char[] incomingChars = new char[BUFFER_SIZE];

        // Records whether an asynchronous send attempt is ongoing
        private bool sendIsOngoing = false;

        // For synchronizing sends
        private readonly object sendSync = new object();

        // Bytes that we are actively trying to send, along with the
        // index of the leftmost byte whose send has not yet been completed
        private byte[] pendingBytes = new byte[0];
        private int pendingIndex = 0;

        /// <summary>
        /// Creates a BoggleConnection from the socket, then begins communicating with it.
        /// </summary>
        /// <param name="s"></param>
        public BoggleConnection(Socket s)
        {
            socket = s;
            incoming = new StringBuilder();
            outgoing = new StringBuilder();

            // Ask the socket to call Recieve as soon as bytes arrive
            socket.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                SocketFlags.None, Received, null);
        }

        /// <summary>
        /// Called when some data has been received.
        /// </summary>
        /// <param name="result"></param>
        private void Received(IAsyncResult result)
        {
            // Figure out how many bytes have come in
            int bytesRead = socket.EndReceive(result);

            // If no bytes were received, it means the client closed its side of the socket.
            // Report that to the console and close our socket.
            if (bytesRead == 0)
            {
                Console.WriteLine("Socket closed");
                socket.Close();
            }
            
            // Otherwise, decode and display the incoming bytes.  Then request more bytes.
            else
            {
                // Convert the bytes into characters and appending to incoming
                int charsRead = decoder.GetChars(incomingBytes, 0, bytesRead, incomingChars, 0, false);
                incoming.Append(incomingChars, 0, charsRead);
                //Console.WriteLine(incoming + "\n");

                ParseReceived();
            }
        }

        /// <summary>
        /// Sends a string to the client
        /// </summary>
        /// <param name="lines"></param>
        private void Send(string lines)
        {
            // Get exclusive access to send mechanism
            lock (sendSync)
            {
                // Append the message to the outgoing lines
                outgoing.Append(lines);

                // If there's not a send ongoing, start one.
                if (!sendIsOngoing)
                {
                    Console.WriteLine("Appending a " + lines.Length + " char line, starting send mechanism");
                    sendIsOngoing = true;
                    SendBytes();
                }
                else
                {
                    Console.WriteLine("\tAppending a " + lines.Length + " char line, send mechanism already running");
                }
            }
        }

        /// <summary>
        /// Attempts to send the entire outgoing string.
        /// This method should not be called unless sendSync has been acquired.
        /// </summary>
        private void SendBytes()
        {
            // If we're in the middle of the process of sending out a block of bytes,
            // keep doing that.
            if (pendingIndex < pendingBytes.Length)
            {
                Console.WriteLine("\tSending " + (pendingBytes.Length - pendingIndex) + " bytes");
                socket.BeginSend(pendingBytes, pendingIndex, pendingBytes.Length - pendingIndex,
                                 SocketFlags.None, Sent, null);
            }

            // If we're not currently dealing with a block of bytes, make a new block of bytes
            // out of outgoing and start sending that.
            else if (outgoing.Length > 0)
            {
                pendingBytes = encoding.GetBytes(outgoing.ToString());
                pendingIndex = 0;
                Console.WriteLine("\tConverting " + outgoing.Length + " chars into " + pendingBytes.Length + " bytes, sending them");
                outgoing.Clear();
                socket.BeginSend(pendingBytes, 0, pendingBytes.Length,
                                 SocketFlags.None, Sent, null);
            }

            // If there's nothing to send, shut down for the time being.
            else
            {
                Console.WriteLine("Shutting down send mechanism\n");
                sendIsOngoing = false;
            }
        }

        /// <summary>
        /// Called when data has been successfully sent
        /// </summary>
        /// <param name="result"></param>
        private void Sent(IAsyncResult result)
        {
            // Find out how many bytes were actually sent
            int bytesSent;
            bytesSent = socket.EndSend(result);
            Console.WriteLine("\t" + bytesSent + " bytes were successfully sent");

            // Get exclusive access to send mechanism
            lock (sendSync)
            {
                // The socket has been closed
                if (bytesSent == 0)
                {
                    socket.Close();
                    Console.WriteLine("Socket closed");
                }

                // Update the pendingIndex and keep trying
                else
                {
                    pendingIndex += bytesSent;
                    SendBytes();
                }
            }
        }

        private void ParseReceived()
        {
            System.Net.HttpStatusCode status;
            string received = incoming.ToString();
            RegexOptions options = RegexOptions.Multiline;
            Regex reg = new Regex(@"^{.*}$", options);
            Match m = reg.Match(received);
            if (!m.Success)
            {
                // Ask for some more data
                socket.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                    SocketFlags.None, Received, null);
                return;
            }

            //Console.WriteLine(incoming + "\n");
            string json = m.ToString();
            string firstLine = received.Split('\n').First();
            string[] ssize = firstLine.Split(null);
            string type = ssize.First();
            string url = ssize[1];

            BoggleService service = new BoggleService();
            string outputJson = service.RequestParser(type, url, json, out status);
            int code = (int)status;
            string output = "HTTP/1.1 " + code.ToString() + " " + status.ToString();
            string contentLength = "Content-Length: " + encoding.GetByteCount(outputJson).ToString();
            string contentType = "Content-Type: application/json; charset=utf-8";
            output += "\r\n" + contentLength + "\r\n" + contentType + "\r\n" + "\r\n" + outputJson;
            //Console.WriteLine(output);
            Send(output);
        }
    }
}
