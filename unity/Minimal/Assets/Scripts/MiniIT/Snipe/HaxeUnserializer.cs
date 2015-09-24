/// <summary>
/// Haxe unserializer.
/// Haxe Serialization Format : http://haxe.org/manual/serialization/format
/// </summary>


using System;
using System.Collections;
using System.Text;
using MiniIT;

namespace MiniIT.Snipe
{
	public class HaxeUnserializer
	{
		protected const string BASE64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789%:";

		public static object Run(string s)
		{
			return new HaxeUnserializer(s).Unserialize();
		}

		protected String mString;
		protected int mPosition;
		protected ArrayList mCache;
		protected ArrayList mStringsCache;

		public HaxeUnserializer(string str)
		{
			this.mString = str;
			this.mPosition = 0;
			this.mCache = new ArrayList();
			this.mStringsCache = new ArrayList();
		}
		
		protected void UnserializeObject(ref ExpandoObject o)
		{
			while(true)
			{
				if (mPosition >= mString.Length)
					throw new Exception("Invalid object");
				if (mString[mPosition] == 'g')
						break;
				string k = (string)Unserialize();
				//if (! k is string)
				//	throw "Invalid object key";
				object v = Unserialize();
				o[k] = v;
			}
			mPosition++;
		}

		protected int ReadDigits()
		{
			int k = 0;
			bool s = false;
			int fpos = mPosition;
			while(true)
			{
				char c = mString[mPosition];
				if (c == 45)
				{
					if (mPosition != fpos)
						break;
					s = true;
					mPosition++;
					continue;
				}
				c = (char)(c - 48);
				if (c < 0 || c > 9)
					break;
				k = k * 10 + c;
				mPosition++;
			}
			if (s)	
				k *= -1;
			return k;
		}

		public object Unserialize()
		{
			switch ((int)mString[mPosition++])
			{
				case 110:  // 'n'
					return null;
					//break;

				case 116:  // 't'
					return true;
					//break;

				case 102:  // 'f'
					return false;
					//break;

				case 122:  // 'z'
					return 0;
					//break;

				case 105:  // 'i'
					return ReadDigits();
					//break;

				case 100:  // 'd'
					int p1 = mPosition;
					while(true)
					{
						int c = mString[mPosition];
						if ((c >= 43 && c < 58) || c == 101 || c == 69)
								mPosition++;
						else
							break;
					}
					return Convert.ToSingle(mString.Substring(p1,mPosition - p1));
					//break;

				case 121:  // 'y'
					int len = ReadDigits();
					if (mString[mPosition++] != ':' || mString.Length - mPosition < len)
						throw new Exception("Invalid string length");
					string s = mString.Substring(mPosition, len);
					mPosition += len;
					s = System.Uri.UnescapeDataString(s);
					mStringsCache.Add(s);
					return s;
					//break;
					
				case 107:  // 'k'
					return float.NaN;
					//break;
				case 109:  // 'm'
					return float.NegativeInfinity;;
					//break;
				case 112:  // 'p'
					return float.PositiveInfinity;
					//break;

				case 97:  // 'a'
					ArrayList a = new ArrayList();
					this.mCache.Add(a);
					while(true)
					{
						int c2 = mString[mPosition];
						if (c2 == 104)
						{
							mPosition++;
							break;
						}
						if (c2 == 117)
						{
							mPosition++;
							int n = ReadDigits();
							a[a.Count + n - 1] = null;
						}
						else
						{
							a.Add(Unserialize());
						}
					}
					return a;
					//break;

				case 111:  // 'o'
					ExpandoObject o = new ExpandoObject();
					mCache.Add(o);
					UnserializeObject(ref o);
					return o;
					//break;

				case 114:  // 'r'
					int n2 = ReadDigits();
					if (n2 < 0 || n2 >= mCache.Count)
						throw new Exception("Invalid reference");
					return mCache[n2];
					//break;

				case 82:  // 'R'
					int n3 = ReadDigits();
					if (n3 < 0 || n3 >= mStringsCache.Count)
						throw new Exception("Invalid string reference");
					return mStringsCache[n3];
					//break;

				case 120:  // 'x'
					throw (Exception) Unserialize();
					//break;

				case 99:  // 'c'
//					string name  = this.Unserialize();
//					var cl : Class = this.resolver.resolveClass(name);
//					if(cl == null) throw "Class not found " + name;
//					var o2 : * = Type.createEmptyInstance(cl);
//					this.cache.push(o2);
//					this.unserializeObject(o2);
//					return o2;
					break;

				case 119:  // 'w'
//					string name2 = Unserialize();
//					var edecl : Class = this.resolver.resolveEnum(name2);
//					if(edecl == null) throw "Enum not found " + name2;
//					return this.unserializeEnum(edecl,this.unserialize());
					break;

				case 106:  // 'j'
//					var name3 : String = this.unserialize();
//					var edecl2 : Class = this.resolver.resolveEnum(name3);
//					if(edecl2 == null) throw "Enum not found " + name3;
//					this.pos++;
//					var index : int = this.readDigits();
//					var tag : String = Type.getEnumConstructs(edecl2)[index];
//					if(tag == null) throw "Unknown enum index " + name3 + "@" + index;
//					return this.unserializeEnum(edecl2,tag);
					break;

				case 108:  // 'l'
					ArrayList l = new ArrayList();
					mCache.Add(l);
					while (mString[mPosition] != 104)
						l.Add(Unserialize());
					mPosition++;
					return l;
					//break;

				case 98:  // 'b'
					Hashtable h = new Hashtable();
					mCache.Add(h);
					while(mString[mPosition] != 104)   // 'h' = 104
					{
						string s2 = (string)Unserialize();
						h[s2] = Unserialize();
					}
					mPosition++;
					return h;
					//break;

				case 113:  // 'q'
					Hashtable h2 = new Hashtable();
					mCache.Add(h2);
					int c3 = mString[mPosition++];
					while (c3 == 58)  // ':'
					{
						int i = ReadDigits();
						h2[i] = Unserialize();
						c3 = mString[mPosition++];
					}
					if (c3 != 104)  // 'h'
						throw new Exception("Invalid IntHash format");
					return h2;
					//break;

				case 118:  // 'v'
					DateTime d = Convert.ToDateTime(mString.Substring(mPosition,19));
					mCache.Add(d);
					mPosition += 19;
					return d;
					//break;

				case 115:  // 's' -  base64 encoded bytes
					int len2 = ReadDigits();
					if (mString[mPosition++] != ':' || mString.Length - mPosition < len2)
						throw new Exception("Invalid bytes length");

					byte[] bytes = Convert.FromBase64String(mString.Substring(mPosition, len2));
					mPosition += len2;
					mCache.Add(bytes);
					return bytes;
					//break;

//				default:
//					break;
			}

			mPosition--;
			throw new Exception("Invalid char " + mString[mPosition] + " at position " + mPosition);
		}
	}
}