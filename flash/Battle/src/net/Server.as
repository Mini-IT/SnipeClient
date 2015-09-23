package net 
{
	import com.adobe.crypto.MD5;
	import flash.events.*;
	import flash.utils.setTimeout;
	import snipe.*;
	
	/**
	 * Battle game server communacator
	 * 
	 * @author Sergey Lukovkin (MiniIT, LTD)
	 */
	public class Server extends EventDispatcher
	{
		private const SERVER_HOST : String = "192.168.0.100";
		private const SERVER_PORT : int = 3010;
		
		private var mClient : snipe.Client;
		
		private var mLoginName : String = "";
		private var mLoginPasswordMD5 : String = "";   // MD5 hash of login password
		private var mRegPassword : String;             // password created during registration
		private var mUserID : String = "";
		
		public function Server() 
		{	
			mClient = new snipe.Client();
			mClient.addEventListener("onConnect",        onConnection);
			mClient.addEventListener("onConnectFailed",  onConnectionFailed);
			mClient.addEventListener("onDisconnect",     onDisconnect);
			mClient.addEventListener("onResponse",       onResponse);
		}
		
		public function connect() : void
		{
			if (mClient != null && !mClient.connected)
				mClient.connect(SERVER_HOST, SERVER_PORT);
		}
		
		public function request(cmd : String, args : * = null) : void
		{
			if (mClient != null && mClient.connected)
				mClient.clientRequest(cmd, args);
		}
		
		public function login(name:String, pwd:String) : void
		{
			loginMD5(name, pwd.length > 0 ? MD5.hash(pwd) : "");
		}
		
		public function loginMD5(name:String, md5pwd:String) : void
		{
			if (this.isLoggedIn && mLoginName == name)
				return;
			
			mLoginName = name;
			mLoginPasswordMD5 = md5pwd;
			
			request("user.login", {name: name, password : md5pwd});
		}
		
		public function registerUser(name:String, password:String) : void
		{
			mLoginName = name;
			mRegPassword = password;
			var data : * =
			{
				name     : name,
				password : password,
				register : true
			};
			
			request("user.register", data);
		}
		
		//-----------------------------
		
		public function battleStart() : void
		{
			request("battle.start");
		}
		
		public function battleTurn(hit_type : int) : void
		{
			request("battle.turn", { type : hit_type });
		}
		
		//-----------------------------
		
		private function onConnection(event:Event):void
		{
			this.dispatchEvent( new GameServerEvent(GameServerEvent.CONNECTION_SUCCEEDED) );
		}
		
		private function onConnectionFailed(evt:Event):void
		{
			this.dispatchEvent( new GameServerEvent(GameServerEvent.CONNECTION_FAILED) );
		}
		
		private function onDisconnect(evt: Event):void
		{
			this.dispatchEvent( new GameServerEvent(GameServerEvent.CONNECTION_LOST) );
		}
		
		public function onResponse(event : ServerEvent) : void
		{
			// ignore "error" and "debug" messages
			if (event.eventType == "error" || event.eventType == "debug")
				return;
			
			var data : * = event.params;
			switch (event.eventType)
			{
				case("user.login"):
					onLogin(data);
					break;
				
				case("user.register"):
					onRegister( data );
					break;
				
				case("battle.start"):
					if (data.errorCode == "ok")
						dispatchEvent(new GameServerEvent(GameServerEvent.BATTLE_STARTED));
					else
						dispatchEvent(new GameServerEvent(GameServerEvent.BATTLE_START_FAILED, data));
					break;
				
				case("battle.turn"):
					dispatchEvent(new GameServerEvent(GameServerEvent.BATTLE_TURN));
					break;
					
				case("battle.finish"):
					dispatchEvent(new GameServerEvent(GameServerEvent.BATTLE_FINISHED));
					break;
			}
		}

		private function onLogin(data : *) : void
		{
			if (data.errorCode == "ok")
			{
				if(data.hasOwnProperty("name") && data.name != "")
					mLoginName = data.name;
					
				mUserID = data.id;
				
				this.dispatchEvent( new GameServerEvent(GameServerEvent.LOGIN_SUCCEEDED, data) );
			}
			else if (data.errorCode == "userDisconnecting") // server is trying to disconnect other seccion, we should try to login later
			{
				this.dispatchEvent( new GameServerEvent(GameServerEvent.LOGIN_WAIT_DISCONNECT) );
				
				// retry to login after some delay
				setTimeout(loginMD5, 500, mLoginName, mLoginPasswordMD5);
			}
			else
			{
				// login failed
				
				this.dispatchEvent( new GameServerEvent(GameServerEvent.LOGIN_FAILED, data) );
			}
		}
		
		private function onRegister(data : *) : void
		{
			if (data.errorCode == "ok")
			{
				// user registered successfully
				// now we need to login
				
				if (mRegPassword == '')
					login(mLoginName, mRegPassword);
				else
					loginMD5(mLoginName, mRegPassword);
				
			}
			else
			{
				this.dispatchEvent( new GameServerEvent(GameServerEvent.REGISTRATION_FAILED, data.errorCode) );
			}
		}
		
		//------------------------
		
		public function get isConnected() : Boolean
		{
			return mClient != null && mClient.connected;
		}
		
		public function get isLoggedIn() : Boolean
		{
			return isConnected && mUserID != "";
		}
	}

}