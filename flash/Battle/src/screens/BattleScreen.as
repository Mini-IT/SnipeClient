package screens 
{
	import flash.display.Sprite;
	import flash.events.*;
	import flash.text.*;
	import net.*;
	
	/**
	 * ...
	 * @author Sergey Lukovkin (MiniIT, LTD)
	 */
	public class BattleScreen extends Sprite 
	{
		private var mServer : Server;
		
		protected var mHeader : TextField;
		protected var mLog : TextField;
		
		protected var mBtnHit : Sprite;
		protected var mBtnKick : Sprite;
		
		public function BattleScreen(server : Server) 
		{
			super();
			
			mServer = server;
			mServer.addEventListener(GameServerEvent.BATTLE_STARTED, onBattleStarted);
			mServer.addEventListener(GameServerEvent.BATTLE_START_FAILED, onBattleStartFailed);
			mServer.addEventListener(GameServerEvent.BATTLE_TURN, onBattleTurn);
			mServer.addEventListener(GameServerEvent.BATTLE_FINISHED, onBattleFinished);
			
			mHeader = new TextField();
			mHeader.text = "--------- BATTLE ---------";
			mHeader.width = 300;
			this.addChild(mHeader);
			
			mLog = new TextField();
			mLog.background = true;
			mLog.y = 20;
			mLog.width = 300;
			mLog.height = 300;
			this.addChild(mLog);
			
			log("Starting battle");
			mServer.battleStart();
		}
		
		private function onBattleStarted(e:GameServerEvent):void 
		{
			log("Battle started");
			
			// create buttons for battle turn
			mBtnHit = ButtonFactory.createButton("Hit", 200, 50);
			mBtnHit.x = 50;
			mBtnHit.y = 400;
			mBtnHit.addEventListener(MouseEvent.CLICK, onBtnHitClick);
			this.addChild(mBtnHit);
			
			mBtnKick = ButtonFactory.createButton("Kick", 200, 50);
			mBtnKick.x = 500;
			mBtnKick.y = 400;
			mBtnKick.addEventListener(MouseEvent.CLICK, onBtnKickClick);
			this.addChild(mBtnKick);
		}
		
		private function onBattleStartFailed(e:GameServerEvent):void 
		{
			// TODO: handle battle start fail
		}
		
		private function onBattleTurn(e:GameServerEvent):void 
		{
			log("Battle turn");
			
			mBtnHit.visible = true;
			mBtnKick.visible = true;
		}
		
		private function onBattleFinished(e:GameServerEvent):void 
		{
			log("Battle finished");
			
			mBtnHit.visible = false;
			mBtnKick.visible = false;
		}
		
		private function onBtnHitClick(e:Event):void 
		{
			mBtnHit.visible = false;
			mBtnKick.visible = false;
			mServer.battleTurn(0);
		}
		
		private function onBtnKickClick(e:Event):void 
		{
			mBtnHit.visible = false;
			mBtnKick.visible = false;
			mServer.battleTurn(1);
		}
		
		private function log(...args) : void
		{
			var s : String = "";
			for each(var a:* in args)
				s += String(a) + " ";
			mLog.appendText(s + "\n");
		}
	}

}