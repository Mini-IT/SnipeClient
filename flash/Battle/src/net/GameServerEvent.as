package net 
{
	import flash.events.Event;
	
	public class GameServerEvent extends Event
	{
		public var params : Object;
		
		public function GameServerEvent(type:String, params:Object=null, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			super(type, bubbles, cancelable);
			
			this.params = params;
		}
		
		// override clone so the event can be redispatched
        public override function clone():Event
		{
            return new GameServerEvent(type, params, bubbles, cancelable);
        }
		
		public static const CONNECTION_SUCCEEDED         : String = "server_connection";             // connection succeeded
		public static const CONNECTION_FAILED            : String = "server_connection_lost";        // connection failed
		public static const CONNECTION_LOST              : String = "server_connection_lost";        // connection lost (disconnect)
		public static const LOGIN_SUCCEEDED              : String = "server_login";                  // login succeeded
		public static const LOGIN_FAILED                 : String = "server_login_failed";           // login failed
		public static const LOGIN_WAIT_DISCONNECT        : String = "server_login_wait_disconnect";  // this user is logged in from elsewhere, we need to wait until the server is disconnecting the other client
		public static const LOGOUT                       : String = "server_logout";                 // logged out successfully
		public static const REGISTRATION_FAILED          : String = "server_registration_failed";    // registration failed
		
		public static const BATTLE_STARTED               : String = "server_battle_started";         // battle started
		public static const BATTLE_START_FAILED          : String = "server_battle_start_failed";    // battle start failed
		public static const BATTLE_TURN                  : String = "server_battle_turn";            // battle turn performed
		public static const BATTLE_FINISHED              : String = "server_battle_finished";        // battle finished
	}

}