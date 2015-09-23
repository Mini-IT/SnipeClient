package screens 
{
	/**
	 * ...
	 * @author Sergey Lukovkin (MiniIT, LTD)
	 */
	public class RegisterScreen extends LoginScreen 
	{
		
		public function RegisterScreen() 
		{
			super();
			
			mHeader.text = "------- REGISTER NEW USER -------";
			
			mBtnLogin.visible = false;
		}
		
	}

}