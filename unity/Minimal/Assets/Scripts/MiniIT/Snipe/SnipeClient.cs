using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Ionic.Zlib;
using MiniIT;

//
// Client to Snipe server
// http://snipeserver.com
// https://github.com/Mini-IT/SnipeWiki/wiki


// Docs on how to use TCP Client:
// http://sunildube.blogspot.ru/2011/12/asynchronous-tcp-client-easy-example.html

namespace MiniIT.Snipe
{
	public class SnipeClient : IDisposable
	{
		public delegate void SnipeServerEventHandler (DataEventArgs data);

		#pragma warning disable 0067

		public event SnipeServerEventHandler ConnectionSucceeded;
		public event SnipeServerEventHandler ConnectionFailed;
		public event SnipeServerEventHandler ConnectionLost;
		//public event SnipeServerEventHandler ErrorHappened;
		public event SnipeServerEventHandler DataReceived;

		#pragma warning restore 0067

		private TcpClient mClient = null;
		private bool mConnected = false;

		private static readonly int RECEIVE_BUFFER_SIZE = 66560; // buffer size = 65 Kb
		private static readonly int MESSAGE_BUFFER_SIZE = 307200; // buffer size = 300 Kb
		private static readonly byte[] MESSAGE_MARKER = new byte[]{0xAA, 0xBB, 0xCD, 0xEF}; // marker of message beginning

		protected MemoryStream mBufferSream;

		protected int mMessageLength = 0;
		protected string mMessageString = "";
		private bool mCompressed; // compression flag: 0 - not compressed, 1 - compressed

		public SnipeClient ()
		{
		}

		private void DispatchEvent(Delegate handler)
		{
			DispatchEvent(handler, null);
		}

		private void DispatchEvent(Delegate handler, ExpandoObject data)
		{
			Delegate event_handler = handler;  // local variable for thread safety
			if (event_handler != null)
			{
				try
				{
					event_handler.DynamicInvoke(new DataEventArgs(data));
				}
				catch(Exception e)
				{
					Debug.Log("[HapiClient] DispatchEvent error: " + e.ToString() + e.Message + "\nErrorData: " + data.ToJSONString());
				}
			}
		}

		public void Connect (string host, int port)
		{
			Disconnect();

			try
			{
				if (mClient == null)
				{
					mClient = new TcpClient(AddressFamily.InterNetwork);
					mClient.ReceiveBufferSize = RECEIVE_BUFFER_SIZE;
					mClient.NoDelay = true;  // send data immediately upon calling NetworkStream.Write
				}

				IPAddress[] host_address = Dns.GetHostAddresses(host);
				//Start the async connect operation
				mClient.BeginConnect(host_address, port, new AsyncCallback(ConnectCallback), mClient);
			}
			catch (Exception e)
			{
				Debug.Log("[HapiClient] TCP Client initialization faled: " + e.Message);

//				if (this.OnConnectionFailed != null)
//					OnConnectionFailed(new HapiEventArgs(HapiEventArgs.CONNECT_FAILED, "Connection Failed: " + e.Message));
				DispatchEvent(ConnectionFailed);
			}
		}

		public void Disconnect()
		{
			if (mBufferSream != null)
			{
				mBufferSream.Close();
				mBufferSream.Dispose();
				mBufferSream = null;
			}

			if (mClient != null)
			{
//				if (mClient.Connected)
//					mClient.EndConnect(null);

				mClient.Close();
				mClient = null;
			}
		}

		private void ConnectCallback(IAsyncResult result)
		{
			try
			{
				//We are connected successfully
				NetworkStream network_stream = mClient.GetStream();

				byte[] buffer = new byte[mClient.ReceiveBufferSize];
				
				//Now we are connected start asyn read operation
				network_stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);

				if (mBufferSream == null)
				{
					mBufferSream = new MemoryStream();
					mBufferSream.Capacity = MESSAGE_BUFFER_SIZE;
				}
				else
				{
					mBufferSream.SetLength(0);  // "clearing" buffer
				}

				mConnected = true;

				// send event
//				if (OnConnect != null)
//					OnConnect(new HapiEventArgs(HapiEventArgs.CONNECT, "Connected")); // "Connected");
				DispatchEvent(ConnectionSucceeded);
			}
			catch(Exception e)
			{
				Debug.Log("[HapiClient] ConnectCallback: " + e.Message);

				mConnected = false;

				// send event
//				if (OnConnectionFailed != null)
//					OnConnectionFailed(new HapiEventArgs(HapiEventArgs.CONNECT_FAILED, "Connection Failed: " + e.Message));
				DispatchEvent(ConnectionFailed);
			}
		}

		// Callback for Read operation
		private void ReadCallback(IAsyncResult result)
		{
			NetworkStream network_stream;
			
			try
			{
				network_stream = mClient.GetStream();
			}
			catch(Exception e)
			{
				Debug.Log("[HapiClient] ReadCallback GetStream error: " + e.Message);
				return;
			}

			byte[] buffer = result.AsyncState as byte[];

			int bytes_read = network_stream.EndRead(result);
			if (bytes_read > 0)
			{
				using(MemoryStream buf_stream = new MemoryStream(buffer, 0, bytes_read))
				{
					try
					{
						ProcessData(buf_stream);
					}
					catch(Exception ex)
					{
#if DEBUG
						//Debug.Log("[SnipeClient] ProcessData error: " + ex.Message);
#endif
					}
				}
			}

			//Then start reading from the network again.
			network_stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
      }

		protected void ProcessData(MemoryStream buf_stream)
		{
			
#if DEBUG
            //Debug.Log("[SnipeClient] portion", buf_stream.Length.ToString());
#endif
			if (mBufferSream == null)
				return;

			if (buf_stream != null)
			{
				long position = mBufferSream.Position;

				mBufferSream.Position = mBufferSream.Length;
				buf_stream.WriteTo(mBufferSream);

				mBufferSream.Position = position;
			}

			while (mBufferSream.Length - mBufferSream.Position > 0)
			{
				// if the length of the message is not known yet and the buffer contains data
				if (this.mMessageLength == 0 && (mBufferSream.Length - mBufferSream.Position) >= 7)
				{
					// in the beginning of a message the marker must be
					byte[] marker = new byte[4];
					mBufferSream.Read(marker, 0, 4);
					if ( !(marker[0] == MESSAGE_MARKER[0] &&
					       marker[1] == MESSAGE_MARKER[1] &&
					       marker[2] == MESSAGE_MARKER[2] &&
					       marker[3] == MESSAGE_MARKER[3]) )
					{
#if DEBUG
						Debug.Log("[SnipeClient] Message marker not found");
#endif

//						if (OnError != null)
//							OnError(new HapiEventArgs(HapiEventArgs.ERROR, "Message marker not found"));
						// DispatchEvent

						// TODO: handle the error !!!!
						// ...

						// if something wrong with the format then clear buffer of the socket and remove all temporary data,
						// i.e. just ignore all that we have at the moment and we'll wait new messages
						AccidentallyClearBuffer();
						return;
					}

					// first 2 bytes contain the length of the message
					mMessageLength = mBufferSream.ReadByte() * 256 + mBufferSream.ReadByte();
					
					// the 3rd byte contains compression flag (0/1)
					mCompressed = (mBufferSream.ReadByte() == 1);

					continue;
				}
				else if (mMessageLength > 0 && (mBufferSream.Length - mBufferSream.Position) >= mMessageLength)  // if the legth of the message is known and the whole message is already in the buffer
				{
#if DEBUG
                    //Debug.Log("[SnipeClient] Message found");
#endif
					// if the message is compressed
					if (mCompressed)
					{
						byte[] compressed_buffer = new byte[mMessageLength];
						mBufferSream.Read(compressed_buffer, 0, compressed_buffer.Length);

						byte[] decompressed_buffer = ZlibStream.UncompressBuffer(compressed_buffer);
						mMessageString = UTF8Encoding.UTF8.GetString( decompressed_buffer );

#if DEBUG
						//Debug.Log("[SnipeClient] decompressed mMessageString = " + mMessageString);
#endif
					}
					else
					{
						byte[] str_buf = new byte[mMessageLength];
						mBufferSream.Read(str_buf, 0, mMessageLength);
						mMessageString = UTF8Encoding.UTF8.GetString(str_buf);

#if DEBUG
						//Debug.Log("[SnipeClient] mMessageString = " + mMessageString);
#endif
					}
					
					mMessageLength = 0;

					// the message is read

					try
					{
						ExpandoObject response = (ExpandoObject)HaxeUnserializer.Run(mMessageString);

						if (response != null)
						{
							DispatchEvent(DataReceived, response);
						}
					}
					catch (Exception error)
					{
#if DEBUG
						Debug.Log("[SnipeClient] Deserialization error: " + error.Message);
#endif
						
//						if (OnError != null)
//							OnError(new HapiEventArgs(HapiEventArgs.ERROR, "Deserialization error: " + error.Message));

						// TODO: handle the error !!!!
						// ...
						
						// if something wrong with the format then clear buffer of the socket and remove all temporary data,
						// i.e. just ignore all that we have at the moment and we'll wait new messages
						AccidentallyClearBuffer();
						return;
					}
				}
				else  // not all of the message's bytes are in the buffer yet
				{
					// wait for the next portion of data
					break;

					// WARNING:
					// this shouldn't happen!!!!!!!!!!!!
				}
			}
		}

		public void SendRequest(string message_type)
		{
			SendRequest(message_type, null);
		}

		public void SendRequest(string message_type, ExpandoObject parameters)
		{
			if (parameters == null)
				parameters = new ExpandoObject();
			
			parameters["messageType"] = message_type;

#if DEBUG
			Debug.Log("[SnipeClient] SendRequest " + parameters.ToJSONString());
#endif

			SendRequest(parameters);
		}

		public void SendRequest(ExpandoObject parameters)
		{
			// mClient.Connected property gets the connection state of the Socket as of the LAST I/O operation (not current state!)
			// (http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.connected.aspx)
			// So we need to check the connection availability manually, and here is where we can do it

			if (this.Connected)
			{
				string message = HaxeSerializer.Run(parameters);

				byte[] buffer = UTF8Encoding.UTF8.GetBytes(message);
				mClient.GetStream().Write(buffer, 0, buffer.Length);
				mClient.GetStream().WriteByte(0);  // every message ends with zero

#if DEBUG
                //Debug.Log("[SnipeClient] sent " + message);
#endif
			}
			else if (mConnected && !(mClient != null && mClient.Connected))
			{
				// Disconnect detected
				mConnected = false;
				DispatchEvent(ConnectionLost);
				return;
			}
		}

		protected void AccidentallyClearBuffer()
		{
			if (mBufferSream != null)
				mBufferSream.SetLength(0);  // clearing buffer

			mMessageLength = 0;
			mMessageString = "";
		}

		public bool Connected
		{
			get
			{
				return mConnected && mClient != null && mClient.Connected;
			}
		}

		#region IDisposable implementation
		
		public void Dispose ()
		{
			Disconnect();
		}
		
		#endregion
	}

}