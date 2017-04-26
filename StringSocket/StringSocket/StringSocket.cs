// Written by Joe Zachary for CS 3500, November 2012
// Revised by Joe Zachary April 2016
// Revised extensively by Joe Zachary April 2017

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CustomNetworking
{
    /// <summary>
    /// The type of delegate that is called when a StringSocket send has completed.
    /// </summary>
    public delegate void SendCallback(bool wasSent, object payload);

    /// <summary>
    /// The type of delegate that is called when a receive has completed.
    /// </summary>
    public delegate void ReceiveCallback(String s, object payload);

    /// <summary> 
    /// A StringSocket is a wrapper around a Socket.  It provides methods that
    /// asynchronously read lines of text (strings terminated by newlines) and 
    /// write strings. (As opposed to Sockets, which read and write raw bytes.)  
    ///
    /// StringSockets are thread safe.  This means that two or more threads may
    /// invoke methods on a shared StringSocket without restriction.  The
    /// StringSocket takes care of the synchronization.
    /// 
    /// Each StringSocket contains a Socket object that is provided by the client.  
    /// A StringSocket will work properly only if the client refrains from calling
    /// the contained Socket's read and write methods.
    /// 
    /// We can write a string to a StringSocket ss by doing
    /// 
    ///    ss.BeginSend("Hello world", callback, payload);
    ///    
    /// where callback is a SendCallback (see below) and payload is an arbitrary object.
    /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
    /// successfully written the string to the underlying Socket, or failed in the 
    /// attempt, it invokes the callback.  The parameter to the callback is the payload.  
    /// 
    /// We can read a string from a StringSocket ss by doing
    /// 
    ///     ss.BeginReceive(callback, payload)
    ///     
    /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
    /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
    /// string of text terminated by a newline character from the underlying Socket, or
    /// failed in the attempt, it invokes the callback.  The parameters to the callback are
    /// a string and the payload.  The string is the requested string (with the newline removed).
    /// </summary>

    public class StringSocket : IDisposable
    {
        // Buffer size for reading incoming bytes
        private const int BUFFER_SIZE = 1024;

        // Underlying socket
        private Socket socket;

        // Encoding used for sending and receiving
        private Encoding encoding;

        // Text that has been received from the client but not yet dealt with
        private static StringBuilder incoming;

        // Text that needs to be sent to the client but which we have not yet started sending
        private StringBuilder outgoing;

        // For synchronizing sends
        private readonly object sendSync = new object();

        /// <summary>
        /// Creates a StringSocket from a regular Socket, which should already be connected.  
        /// The read and write methods of the regular Socket must not be called after the
        /// StringSocket is created.  Otherwise, the StringSocket will not behave properly.  
        /// The encoding to use to convert between raw bytes and strings is also provided.
        /// </summary>
        internal StringSocket(Socket s, Encoding e)
        {
            socket = s;
            encoding = e;
            incoming = new StringBuilder();
            outgoing = new StringBuilder();
            // TODO: Complete implementation of StringSocket
        }

        /// <summary>
        /// Shuts down this StringSocket.
        /// </summary>
        public void Shutdown(SocketShutdown mode)
        {
            socket.Shutdown(mode);
        }

        /// <summary>
        /// Closes this StringSocket.
        /// </summary>
        public void Close()
        {
            socket.Close();
        }

        /// <summary>
        /// We can write a string to a StringSocket ss by doing
        /// 
        ///    ss.BeginSend("Hello world", callback, payload);
        ///    
        /// where callback is a SendCallback (see below) and payload is an arbitrary object.
        /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
        /// successfully written the string to the underlying Socket it invokes the callback.  
        /// The parameters to the callback are true and the payload.
        /// 
        /// If it is impossible to send because the underlying Socket has closed, the callback 
        /// is invoked with false and the payload as parameters.
        ///
        /// This method is non-blocking.  This means that it does not wait until the string
        /// has been sent before returning.  Instead, it arranges for the string to be sent
        /// and then returns.  When the send is completed (at some time in the future), the
        /// callback is called on another thread.
        /// 
        /// This method is thread safe.  This means that multiple threads can call BeginSend
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginSend must take care of synchronization instead.  On a given StringSocket, each
        /// string arriving via a BeginSend method call must be sent (in its entirety) before
        /// a later arriving string can be sent.
        /// </summary>
        public void BeginSend(String s, SendCallback callback, object payload)
        {
            // Get exclusive access to send mechanism
            lock (sendSync)
            {
                StateObject obj = new StateObject()
                {
                    Callback = callback,
                    Payload = payload
                };
                // Append the message to the outgoing lines
                lock (outgoing)
                {
                    outgoing.Append(s);
                }

                SendBytes(obj);
            }
        }

        /// <summary>
        /// Attempts to send the entire outgoing string.
        /// This method should not be called unless sendSync has been acquired.
        /// </summary>
        private void SendBytes(StateObject obj)
        {
            // If we're in the middle of the process of sending out a block of bytes,
            // keep doing that.
            if (obj.pendingIndex < obj.pendingBytes.Length)
            {
                socket.BeginSend(obj.pendingBytes, obj.pendingIndex, obj.pendingBytes.Length - obj.pendingIndex,
                                 SocketFlags.None, new AsyncCallback(Sent), obj);
            }

            // If we're not currently dealing with a block of bytes, make a new block of bytes
            // out of outgoing and start sending that.
            else if (outgoing.Length > 0)
            {
                lock (outgoing)
                {
                    obj.pendingBytes = encoding.GetBytes(outgoing.ToString());
                    obj.pendingIndex = 0;
                    outgoing.Clear();
                }
                socket.BeginSend(obj.pendingBytes, 0, obj.pendingBytes.Length,
                                 SocketFlags.None, new AsyncCallback(Sent), obj);
            }

            // If there's nothing to send, shut down for the time being.
            else
            {
                SendCallback callback = (SendCallback)obj.Callback;
                object payload = obj.Payload;
                Task t = new Task(() => callback(true, payload));
                t.Start();
            }
        }

        /// <summary>
        /// Called when data has been successfully sent
        /// </summary>
        /// <param name="result"></param>
        private void Sent(IAsyncResult result)
        {
            StateObject obj = (StateObject)result.AsyncState;
            SendCallback callback = (SendCallback)obj.Callback;
            object payload = obj.Payload;
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
                    Task t = new Task(() => callback(false, payload));
                    t.Start();
                    Close();
                }

                // Update the pendingIndex and keep trying
                else
                {
                    obj.pendingIndex += bytesSent;
                    SendBytes(obj);
                }
            }
        }

        /// <summary>
        /// We can read a string from the StringSocket by doing
        /// 
        ///     ss.BeginReceive(callback, payload)
        ///     
        /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
        /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
        /// string of text terminated by a newline character from the underlying Socket, it 
        /// invokes the callback.  The parameters to the callback are a string and the payload.  
        /// The string is the requested string (with the newline removed).
        /// 
        /// Alternatively, we can read a string from the StringSocket by doing
        /// 
        ///     ss.BeginReceive(callback, payload, length)
        ///     
        /// If length is negative or zero, this behaves identically to the first case.  If length
        /// is positive, then it reads and decodes length bytes from the underlying Socket, yielding
        /// a string s.  The parameters to the callback are s and the payload
        ///
        /// In either case, if there are insufficient bytes to service a request because the underlying
        /// Socket has closed, the callback is invoked with null and the payload.
        /// 
        /// This method is non-blocking.  This means that it does not wait until a line of text
        /// has been received before returning.  Instead, it arranges for a line to be received
        /// and then returns.  When the line is actually received (at some time in the future), the
        /// callback is called on another thread.
        /// 
        /// This method is thread safe.  This means that multiple threads can call BeginReceive
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginReceive must take care of synchronization instead.  On a given StringSocket, each
        /// arriving line of text must be passed to callbacks in the order in which the corresponding
        /// BeginReceive call arrived.
        /// 
        /// Note that it is possible for there to be incoming bytes arriving at the underlying Socket
        /// even when there are no pending callbacks.  StringSocket implementations should refrain
        /// from buffering an unbounded number of incoming bytes beyond what is required to service
        /// the pending callbacks.
        /// </summary>
        public void BeginReceive(ReceiveCallback callback, object payload, int length = 0)
        {
            StateObject obj = new StateObject()
            {
                Callback = callback,
                Payload = payload
            };

            lock (sendSync)
            {
                socket.BeginReceive(obj.incomingBytes, 0, obj.incomingBytes.Length,
                        SocketFlags.None, new AsyncCallback(Received), obj);
            }
        }

        bool newLine = false;

        /// <summary>
        /// Called when some data has been received.
        /// </summary>
        /// <param name="result"></param>
        private void Received(IAsyncResult result)
        {
            StateObject obj = (StateObject)result.AsyncState;
            ReceiveCallback callback = (ReceiveCallback)obj.Callback;
            object payload = obj.Payload;

            // Figure out how many bytes have come in
            int bytesRead = socket.EndReceive(result);

            // If no bytes were received, it means the client closed its side of the socket.
            // Report that to the console and close our socket.
            if (bytesRead == 0)
            {
                Task t = new Task(() => callback(null, payload));
                t.Start();
                Close();
            }

            // Otherwise, decode and display the incoming bytes.  Then request more bytes.
            else
            {

                lock (incoming)
                {
                    // Convert the bytes into characters and appending to incoming
                    int charsRead = encoding.GetDecoder().GetChars(obj.incomingBytes, 0, bytesRead, obj.incomingChars, 0, false);
                    incoming.Append(obj.incomingChars, 0, charsRead);

                    // Find all of the new lines and call back for each string before it
                    String incString = incoming.ToString();
                    String[] returns = incString.Split('\n');
                    // clear incoming
                    incoming.Clear();
                    for (int i = 0; i < returns.Length; i++)
                    {
                        if (i != returns.Length - 1)
                        {
                            newLine = true;
                            int current = i;
                            Task t = new Task(() => callback(returns[current], payload));
                            t.Start();
                        }
                        else
                        {
                            incoming.Append(returns[i]);
                        }
                    }
                }

                if (bytesRead == BUFFER_SIZE || !newLine)
                {
                    // Ask for some more data
                    socket.BeginReceive(obj.incomingBytes, 0, obj.incomingBytes.Length,
                                SocketFlags.None, new AsyncCallback(Received), obj);
                }
            }
        }

        /// <summary>
        /// Frees resources associated with this StringSocket.
        /// </summary>
        public void Dispose()
        {
            Shutdown(SocketShutdown.Both);
            Close();
        }



        /// <summary>
        /// Helper class for async callback
        /// </summary>
        class StateObject
        {
            public object Callback { get; set; }
            public object Payload { get; set; }

            // Bytes that we are actively trying to send, along with the
            // index of the leftmost byte whose send has not yet been completed
            public byte[] pendingBytes = new byte[0];
            public int pendingIndex = 0;

            // Buffers that will contain incoming bytes and characters
            public byte[] incomingBytes = new byte[BUFFER_SIZE];
            public char[] incomingChars = new char[BUFFER_SIZE];
        }
    }

}
