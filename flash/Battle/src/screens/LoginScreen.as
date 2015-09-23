package screens 
{
	import flash.display.Shape;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.text.*;
	
	/**
	 * ...
	 * @author Sergey Lukovkin (MiniIT, LTD)
	 */
	public class LoginScreen extends Sprite 
	{
		public static const REGISTER_CLICKED : String = "register_clicked";
		public static const LOGIN_CLICKED : String = "login_clicked";
		
		protected const LEFT_PADDING : int = 10;
		protected const TOP_PADDING : int = 30;
		protected const COLUMN_WIDTH : int = 150;
		protected const ROW_HEIGHT : int = 20;
		protected const ROW_GAP : int = 5;
		
		protected var mHeader : TextField;
		
		protected var mNameLabel : TextField;
		protected var mNameInputField : TextField;
		
		protected var mPasswordLabel : TextField;
		protected var mPasswordInputField : TextField;
		
		protected var mBtnLogin : Sprite;
		protected var mBtnRegister : Sprite;
		
		public function LoginScreen() 
		{
			super();
			
			mHeader = new TextField();
			mHeader.text = "------- LOGIN -------";
			mHeader.width = COLUMN_WIDTH * 2;
			this.addChild(mHeader);
			
			mNameLabel = new TextField();
			mNameLabel.text = "Login name:";
			mNameLabel.x = LEFT_PADDING;
			mNameLabel.y = TOP_PADDING;
			this.addChild(mNameLabel);
			
			mNameInputField = new TextField();
			mNameInputField.text = "username";
			mNameInputField.type = TextFieldType.INPUT;
			mNameInputField.background = true;
			mNameInputField.x = LEFT_PADDING + COLUMN_WIDTH;
			mNameInputField.y = TOP_PADDING;
			mNameInputField.width = COLUMN_WIDTH;
			mNameInputField.height = ROW_HEIGHT;
			this.addChild(mNameInputField);
			
			mPasswordLabel = new TextField();
			mPasswordLabel.text = "Password:";
			mPasswordLabel.x = LEFT_PADDING;
			mPasswordLabel.y = TOP_PADDING + ROW_HEIGHT + ROW_GAP;
			this.addChild(mPasswordLabel);
			
			mPasswordInputField = new TextField();
			mPasswordInputField.text = "password";
			mPasswordInputField.type = TextFieldType.INPUT;
			mPasswordInputField.background = true;
			mPasswordInputField.x = LEFT_PADDING + COLUMN_WIDTH;
			mPasswordInputField.y = TOP_PADDING + ROW_HEIGHT + ROW_GAP;
			mPasswordInputField.width = COLUMN_WIDTH;
			mPasswordInputField.height = ROW_HEIGHT;
			this.addChild(mPasswordInputField);
			
			mBtnRegister = ButtonFactory.createButton("Register", COLUMN_WIDTH - 10, ROW_HEIGHT);
			mBtnRegister.x = LEFT_PADDING;
			mBtnRegister.y = TOP_PADDING + (ROW_HEIGHT + ROW_GAP) * 2;
			mBtnRegister.addEventListener(MouseEvent.CLICK, onBtnRegisterClick);
			this.addChild(mBtnRegister);
			
			mBtnLogin = ButtonFactory.createButton("Login", COLUMN_WIDTH - 10, ROW_HEIGHT);
			mBtnLogin.x = LEFT_PADDING + COLUMN_WIDTH;
			mBtnLogin.y = TOP_PADDING + (ROW_HEIGHT + ROW_GAP) * 2;
			mBtnLogin.addEventListener(MouseEvent.CLICK, onBtnLoginClick);
			this.addChild(mBtnLogin);
		}
		
		protected function onBtnRegisterClick(e:MouseEvent):void 
		{
			dispatchEvent(new Event(REGISTER_CLICKED));
		}
		
		protected function onBtnLoginClick(e:MouseEvent):void 
		{
			dispatchEvent(new Event(LOGIN_CLICKED));
		}
		
		public function get username() : String
		{
			return mNameInputField.text;
		}
		
		public function get password() : String
		{
			return mPasswordInputField.text;
		}
	}

}