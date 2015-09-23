package screens 
{
	import flash.display.*;
	import flash.text.*;
	
	/**
	 * Simple button factory
	 * @author Sergey Lukovkin (MiniIT, LTD)
	 */
	public class ButtonFactory 
	{
		public static function createButton(text : String, w : Number, h : Number, color : uint = 0x7F92FF) : Sprite
		{
			var button : Sprite = new Sprite();
			button.buttonMode = true;
			button.mouseChildren = false;
			
			var shape : Shape = new Shape();
			shape.graphics.lineStyle(1, 0);
			shape.graphics.beginFill(color)
			shape.graphics.drawRect(0, 0, w, h);
			shape.graphics.endFill();
			button.addChild(shape);
			
			var textfield : TextField = new TextField();
			textfield.name = "label";
			textfield.text = text;
			textfield.selectable = false;
			textfield.x = (w - textfield.textWidth) * 0.5;
			textfield.y = (h - textfield.textHeight) * 0.5;
			button.addChild(textfield);
			
			return button;
		}
		
	}

}