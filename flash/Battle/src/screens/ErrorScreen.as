package screens 
{
	import flash.display.Sprite;
	import flash.events.*;
	import flash.text.*;
	
	/**
	 * ...
	 * @author Sergey Lukovkin (MiniIT, LTD)
	 */
	public class ErrorScreen extends Sprite 
	{
		protected var mHeader : TextField;
		protected var mBtnBack : Sprite;
		
		public function ErrorScreen(text : String) 
		{
			super();
			
			mHeader = new TextField();
			mHeader.text = text;
			mHeader.width = 300;
			this.addChild(mHeader);
			
			mBtnBack = ButtonFactory.createButton("Back", 140, 20);
			mBtnBack.x = 10;
			mBtnBack.y = 50;
			mBtnBack.addEventListener(MouseEvent.CLICK, onBtnBackClick);
			this.addChild(mBtnBack);
		}
		
		private function onBtnBackClick(e:Event):void 
		{
			dispatchEvent(new Event(Event.CLOSE));
			
			// remove the screen from parent (and from stage)
			if (this.parent)
				this.parent.removeChild(this);
		}
	}

}