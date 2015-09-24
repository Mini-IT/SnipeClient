
namespace MiniIT
{
	public class DataEventArgs //: SimpleEventArgs
	{
		protected ExpandoObject mData;
		
		public ExpandoObject Data
		{
			get { return mData; }
			set { mData = value; }
		}

		/*
		public DataEventArgs (string event_type) : base(event_type)
		{
			mData = null;
		}
		
		public DataEventArgs (string event_type, ExpandoObject data) : base(event_type)
		{
			mData = data;
		}
		
		public override SimpleEventArgs Clone()
		{
			return new DataEventArgs(mEventType, mData.Clone());
		}
		*/

		public DataEventArgs ()
		{
			mData = null;
		}
		
		public DataEventArgs (ExpandoObject data)
		{
			mData = data;
		}
		
		public DataEventArgs Clone()
		{
			return new DataEventArgs(mData.Clone());
		}

		#region Empty

		private static DataEventArgs mEmptyInstance;

		public static DataEventArgs Empty
		{
			get
			{
				if (mEmptyInstance == null)
					mEmptyInstance = new DataEventArgs();

				return mEmptyInstance;
			}
		}

		#endregion
	}
}

