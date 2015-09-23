package 
{
	import flash.Boot;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.text.TextField;
	import snipe.*;
	
	/**
	 * Simple SNIPE Client
	 */
	public class Main extends Sprite 
	{
		private const SERVER_HOST : String = "192.168.0.100";
		private const SERVER_PORT : int = 3010;
		
		private var mTextField : TextField;
		private var mClient : snipe.Client;
		
		public function Main():void 
		{
			if (stage)
				init();
			else
				addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event = null):void 
		{
			this.removeEventListener(Event.ADDED_TO_STAGE, init);
			
			mTextField = new TextField();
			mTextField.width = 800;
			mTextField.height = 600;
			this.addChild(mTextField);
			
			// snipe client is initially written in haxe, so we need to call this constructor function to do some initialization
			new flash.Boot();
			
			// Creating client
			log("Creating client...");
			
			mClient = new snipe.Client();
			mClient.addEventListener("onConnect",        onConnection);
			mClient.addEventListener("onConnectFailed",  onConnectionFailed);
			mClient.addEventListener("onDisconnect",     onDisconnect);
			mClient.addEventListener("onResponse",       onResponse);
			
			// Connecting to server
			log("Connecting to", SERVER_HOST, SERVER_PORT);
			
			mClient.connect(SERVER_HOST, SERVER_PORT);
		}
		
		private function onConnection(e:Event):void 
		{
			log("Connection established");
			log("Sending request...");
			
			mClient.clientRequest("user.login", { name : "testname", password : "" } );
		}
		
		private function onConnectionFailed(e:Event):void 
		{
			log("Connection failed");
		}
		
		private function onDisconnect(e:Event):void 
		{
			log("Connection lost");
		}
		
		private function onResponse(e:ServerEvent):void 
		{
			log("Response:", (e.params != null) ? e.params.errorCode : "---");
		}
		
		private function log(...args) : void
		{
			var s : String = "";
			for each(var a:* in args)
				s += String(a) + " ";
			mTextField.appendText(s + "\n");
		}
	}
	
}