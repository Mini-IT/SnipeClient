var LibraryWebSockets = {
$webSocketInstances: [],

SocketCreate: function(url)
{
	var str = Pointer_stringify(url);
	var socket = {
		socket: new WebSocket(str),
		buffer: new Uint8Array(0),
		error: null,
		messages: []
	}
	socket.socket.onmessage = function (e) {
		// Todo: handle other data types?
		if (e.data instanceof Blob)
		{
			var reader = new FileReader();
			reader.addEventListener("loadend", function() {
				var array = new Uint8Array(reader.result);
				socket.messages.push(array);
			});
			reader.readAsArrayBuffer(e.data);
		}
		else if (typeof e.data === "string")
		{
			var reader = new FileReader();
			reader.addEventListener("loadend", function() {
				var array = new Uint8Array(reader.result);
				socket.messages.push(array);
			});
			var blob = new Blob([e.data]);
			reader.readAsArrayBuffer(blob);
		}
		else if (e.data instanceof ArrayBuffer)
		{
			socket.messages.push(new Uint8Array(e.data));
		}
	};
	socket.socket.onclose = function (e) {
		if (e.code != 1000)
		{
			if (e.reason != null && e.reason.length > 0)
				socket.error = e.reason;
			else
			{
				switch (e.code)
				{
					case 1001: 
						socket.error = "Endpoint going away.";
						break;
					case 1002: 
						socket.error = "Protocol error.";
						break;
					case 1003: 
						socket.error = "Unsupported message.";
						break;
					case 1005: 
						socket.error = "No status.";
						break;
					case 1006: 
						socket.error = "Abnormal disconnection.";
						break;
					case 1009: 
						socket.error = "Data frame too large.";
						break;
					default:
						socket.error = "Error "+e.code;
				}
			}
		}
	};
	var instance = webSocketInstances.push(socket) - 1;
	return instance;
},

SocketState: function (socketInstance)
{
	var socket = webSocketInstances[socketInstance];
	return socket.socket.readyState;
},

SocketError: function (socketInstance)
{
	var socket = webSocketInstances[socketInstance];
	
  	if (socket.error == null)
 		return "";
	
	var buffer = _malloc(lengthBytesUTF8(socket.error) + 1);
	writeStringToMemory(socket.error, buffer);
	return buffer;
},

SocketSend: function (socketInstance, buf_ptr, length)
{
	var socket = webSocketInstances[socketInstance];
	socket.socket.send (intArrayToString(HEAPU8.subarray(buf_ptr, buf_ptr+length)));
},

SocketRecvLength: function(socketInstance)
{
	var socket = webSocketInstances[socketInstance];
	if (socket.messages.length == 0)
		return 0;
	return socket.messages[0].length;
},

SocketRecv: function (socketInstance, buf_ptr, buf_length)
{
	var socket = webSocketInstances[socketInstance];
	if (socket.messages.length == 0)
		return 0;
	if (socket.messages[0].length > buf_length)
		return 0;
	HEAPU8.set(socket.messages[0], buf_ptr);
	socket.messages = socket.messages.slice(1);
	return 1;
},

SocketClose: function (socketInstance)
{
	var socket = webSocketInstances[socketInstance];
	socket.socket.close();
}
};

autoAddDeps(LibraryWebSockets, '$webSocketInstances');
mergeInto(LibraryManager.library, LibraryWebSockets);
