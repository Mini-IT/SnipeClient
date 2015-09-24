
using System;

namespace MiniIT
{
	public class SimpleEventArgs // : System.EventArgs
	{
		protected string mEventType;

		public string EventType
		{
			get { return mEventType; }
		}

		public SimpleEventArgs (string event_type)
		{
			mEventType = event_type;
		}

		public virtual SimpleEventArgs Clone()
		{
			return new SimpleEventArgs(mEventType);
		}

		public static SimpleEventArgs Empty
		{
			get { return new SimpleEventArgs(""); }
		}
	}
}

