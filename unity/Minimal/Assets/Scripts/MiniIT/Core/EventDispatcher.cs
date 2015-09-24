using System;
using System.Collections;
using System.Reflection;

namespace MiniIT
{
	public class EventDispatcher
	{
		public EventDispatcher()
		{
		}

		public void DispatchEvent(Delegate handler, string text)
		{
			if (text != null && text != "")
			{
				ExpandoObject data = new ExpandoObject();
				data["text"] = text;
				DispatchEvent(handler, data);
			}
			DispatchEvent(handler);
		}
		
		public void DispatchEvent(Delegate handler, ExpandoObject data = null)
		{
			Delegate event_handler = handler;  // local variable for thread safety
			if (event_handler != null)
			{
				//event_handler.DynamicInvoke(new DataEventArgs(data));
				foreach (var h in event_handler.GetInvocationList())
				{
					h.DynamicInvoke(new DataEventArgs(data));
				}
			}
		}

		public void DispatchEvent(string event_name, ExpandoObject data)
		{
			// http://stackoverflow.com/questions/198543/how-do-i-raise-an-event-via-reflection-in-net-c

			//EventInfo event_info = this.GetType().GetEvent(event_name);
			//Type t = this.GetType();
			//FieldInfo[] f = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			FieldInfo field = this.GetType().GetField(event_name, BindingFlags.Instance | BindingFlags.NonPublic);
			if (field != null)
			{
				var event_delegate = (MulticastDelegate)field.GetValue(this);
				if (event_delegate != null)
				{
					foreach (var handler in event_delegate.GetInvocationList())
					{
						handler.DynamicInvoke(new DataEventArgs(data)); // .Method.Invoke(handler.Target, new object[] { this, new DataEventArgs(data) });
					}
				}
			}
		}
	}
}
