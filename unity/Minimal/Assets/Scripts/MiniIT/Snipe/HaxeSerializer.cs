/// <summary>
/// Haxe serializer.
/// Haxe Serialization Format : http://haxe.org/manual/serialization/format
/// </summary>


using System;
using System.Collections;
using System.Text;
using MiniIT;

namespace MiniIT.Snipe
{
	public class HaxeSerializer
	{
		public static bool USE_CACHE = false;
		public static bool USE_ENUM_INDEX = false;

		public static string Run(object v)
		{
			HaxeSerializer s = new HaxeSerializer();
			s.Serialize(v);
			return s.mResultStringBuilder.ToString();
		}

		protected StringBuilder mResultStringBuilder;
		protected ArrayList mCache;
		protected Hashtable mStringsHash;  // strings hash table
		protected int mStringsCount;       // hashed strings count

		public bool UseCache;
		public bool UseEnumIndex;

		public HaxeSerializer()
		{
			this.mResultStringBuilder = new StringBuilder();
			this.mCache = new ArrayList();
			this.UseCache = USE_CACHE;
			this.UseEnumIndex = USE_ENUM_INDEX;
			this.mStringsHash = new Hashtable();
			this.mStringsCount = 0;
		}
		
	//	public override string ToString ()
	//	{
	//		return this.mResultStringBuilder.ToString();
	//	}

		// serialization of cached reference
		protected bool SerializeRef(object v)
		{
			int count = this.mCache.Count;
			for(int i = 0; i < count; i++)
			{
				if (this.mCache[i].Equals(v))
				{
					this.mResultStringBuilder.Append("r");
					this.mResultStringBuilder.Append(i);
					return true;
				}
			}

			this.mCache.Add(v);
			return false;
		}

		protected void SerializeString(string s)
		{
			if (this.mStringsHash.ContainsKey(s))
			{
				this.mResultStringBuilder.Append("R");
				this.mResultStringBuilder.Append(this.mStringsHash[s]);
			}
			else
			{
				this.mStringsHash.Add(s, this.mStringsCount++);
				this.mResultStringBuilder.Append("y");
				s = System.Uri.EscapeDataString(s);  //s = HttpUtility.UrlEncode(s, System.Text.ASCIIEncoding.ASCII);  // WWW.EscapeURL(s);
				this.mResultStringBuilder.Append(s.Length);
				this.mResultStringBuilder.Append(":");
				this.mResultStringBuilder.Append(s);
			}
		}

		protected void SerializeStruct(ExpandoObject obj)
		{
			this.mResultStringBuilder.Append("o");

			foreach (string key in obj.GetDynamicMemberNames())
			{
				SerializeString(key);
				Serialize(obj[key]);
			}

			this.mResultStringBuilder.Append("g");
		}

		public void Serialize(object v)
		{
			if(v == null)
			{
				this.mResultStringBuilder.Append("n");
			}
			else if (v is string || v is char)
			{
				SerializeString((string)v);
			}
			else if (v is ExpandoObject)
			{
				if (this.UseCache && SerializeRef(v))
					return;

				SerializeStruct((ExpandoObject)v);
			}
			else if (v is IList)
			{
				if (v is IEnumerable) // Array)
					this.mResultStringBuilder.Append("a");
				else
					this.mResultStringBuilder.Append("l");

				// NOTE: in arrays if there are several consecutive nulls, we can store u5 instead of nnnnn

				foreach (object item in (IList)v)
				{
					Serialize(item);
				}

				this.mResultStringBuilder.Append("h");
			}
			else if (v is byte[])
			{
				string base64string = Convert.ToBase64String((byte[])v);

				this.mResultStringBuilder.Append("s");
				this.mResultStringBuilder.Append(base64string.Length);
				this.mResultStringBuilder.Append(":");
				this.mResultStringBuilder.Append(base64string);
			}
			else
			{
				switch (Type.GetTypeCode(v.GetType()))
				{
					case TypeCode.Byte:
					case TypeCode.SByte:
					case TypeCode.UInt16:
					case TypeCode.UInt32:
					case TypeCode.UInt64:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
						this.mResultStringBuilder.Append( v.Equals(0) ? "z" : "i" + v.ToString() );
						break;

//					case TypeCode.Decimal:
//					case TypeCode.Double:
					case TypeCode.Single:
						if (float.IsNaN((float)v))
							this.mResultStringBuilder.Append("k");
						else if (float.IsNegativeInfinity((float)v))
							this.mResultStringBuilder.Append("m");
						else if (float.IsPositiveInfinity((float)v))
							this.mResultStringBuilder.Append("p");
						else
							this.mResultStringBuilder.Append("d" + ((float)v).ToString());
						break;
					
					case TypeCode.Double:
						if (double.IsNaN((double)v))
							this.mResultStringBuilder.Append("k");
						else if (double.IsNegativeInfinity((double)v))
							this.mResultStringBuilder.Append("m");
						else if (double.IsPositiveInfinity((double)v))
							this.mResultStringBuilder.Append("p");
						else
							this.mResultStringBuilder.Append("d" + ((double)v).ToString());
						break;

					case TypeCode.Boolean:
						this.mResultStringBuilder.Append(Convert.ToBoolean(v) ? "t" : "f");
						break;

					//default:
						
				}
			}
		}
	}
}