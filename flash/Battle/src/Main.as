package 
{
	import flash.Boot;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.text.TextField;
	import net.*;
	import screens.*;
	
	/**
	 * Simple Battle Game Example
	 */
	public class Main extends Sprite 
	{
		private var mLoginScreen : LoginScreen;
		private var mRegisterScreen : RegisterScreen;
		
		private var mServer : Server;
		
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
			
			mServer = new Server();
			mServer.addEventListener(GameServerEvent.CONNECTION_SUCCEEDED, onConnected);
			mServer.addEventListener(GameServerEvent.CONNECTION_FAILED, onConnectionFailed);
			mServer.addEventListener(GameServerEvent.CONNECTION_LOST, onConnectionLost);
			mServer.addEventListener(GameServerEvent.LOGIN_SUCCEEDED, onLogin);
			mServer.addEventListener(GameServerEvent.LOGIN_FAILED, onLoginFailed);
			mServer.addEventListener(GameServerEvent.REGISTRATION_FAILED, onRegistrationFailed);
			
			mServer.connect();
		}
		
		private function onConnectionFailed(e:GameServerEvent):void 
		{
			showError("Connection failed");
		}
		
		private function onConnectionLost(e:GameServerEvent):void 
		{
			showError("Connection lost");
		}
		
		private function onRegistrationFailed(e:GameServerEvent):void 
		{
			showError("Registration failed: " + e.params).addEventListener(Event.CLOSE,
				function(ev:*) : void
				{
					showLoginScreen();
				}
			);
		}
		
		private function onConnected(e:GameServerEvent):void 
		{
			// connected successfully
			// let's show login screen
			showLoginScreen();
		}
		
		private function onLogin(e:GameServerEvent):void 
		{
			// logged in successfully
			// let's go to battle screen
			
			this.removeChild(mLoginScreen);
			mLoginScreen = null;
			
			this.addChild(new BattleScreen(mServer));
		}
		
		private function onLoginFailed(e:GameServerEvent):void 
		{
			showError("Login failed: " + e.params.errorCode).addEventListener(Event.CLOSE,
				function(ev:*) : void
				{
					showLoginScreen();
				}
			);
		}
		
		private function showLoginScreen() : void
		{
			if (!mLoginScreen)
			{
				mLoginScreen = new LoginScreen();
				mLoginScreen.addEventListener(LoginScreen.REGISTER_CLICKED, onLoginScreenRegisterClicked);
				mLoginScreen.addEventListener(LoginScreen.LOGIN_CLICKED, onLoginScreenLoginClicked);
				this.addChild(mLoginScreen);
			}
			else
			{
				mLoginScreen.visible = true;
			}
		}
		
		private function showError(text : String) : ErrorScreen
		{
			return addChild(new ErrorScreen(text)) as ErrorScreen;
		}
		
		private function onLoginScreenLoginClicked(e:Event):void 
		{
			mLoginScreen.visible = false;
			
			mServer.loginMD5(mLoginScreen.username, mLoginScreen.password);
		}
		
		private function onLoginScreenRegisterClicked(e:Event):void 
		{
			mLoginScreen.visible = false;
			
			if (!mRegisterScreen)
			{
				mRegisterScreen = new RegisterScreen();
				mRegisterScreen.addEventListener(LoginScreen.REGISTER_CLICKED, onRegisterScreenRegisterClicked);
				this.addChild(mRegisterScreen);
			}
			else
			{
				mRegisterScreen.visible = true;
			}
		}
		
		private function onRegisterScreenRegisterClicked(e:Event):void 
		{
			mRegisterScreen.visible = false;
			
			mServer.registerUser(mRegisterScreen.username, mRegisterScreen.password);
		}
		
	}
	
}