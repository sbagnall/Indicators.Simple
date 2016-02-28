using System.Collections;
using System.Diagnostics;
using System.Text;

namespace SteveBagnall.Trading.Indicators.Shared
{
    public class IndicatorParameters
	{
		private object[] _list;
		public object[] List
		{
			[DebuggerStepThrough()]
			get { return _list; }
			[DebuggerStepThrough()]
			set { _list = value; }
		}

		[DebuggerStepThrough()]
		public IndicatorParameters()
		{
			//_hasPeriod = false;
			this.List = new object[]{};
		}

		[DebuggerStepThrough()]
		public IndicatorParameters(params object[] List)
		{
			this.List = List;
		}

		[DebuggerStepThrough()]
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < this.List.Length; i++)
			{
				string value = "";
				if (this.List[i] is IList) 
				{
					value = "[";

					for (int j = 0; j < (this.List[i] as IList).Count; j++)
					{
						object obj = (this.List[i] as IList)[j];
						value += obj.ToString();

						if (j != ((this.List[i] as IList).Count - 1))
							value += ",";
					}

					value += "]";
				}
				else
				{
					value = (this.List[i] == null) ? "NULL" : this.List[i].ToString();
				}

				sb.Append(value);

				if (i != (this.List.Length - 1))
					sb.Append(",");
			}

			return sb.ToString();
		}

		[DebuggerStepThrough()]
		public bool Equals(IndicatorParameters p)
		{
			if ((object)p == null)
				return false;

			if (this.List.Length != p.List.Length)
				return false;

			for (int i = 0; i < this.List.Length; i++)
			{
				if ((this.List[i] == null) && (p.List[i] != null))
					return false;
				else if ((this.List[i] != null) && (p.List[i] == null))
					return false;
				else if ((this.List[i] != null) && (p.List[i] != null))
				{
					if (this.List[i].GetHashCode() != p.List[i].GetHashCode())
						return false;
				}
			}

			return true;
		}

		[DebuggerStepThrough()]
		public override bool Equals(object obj)
		{
			if (!(obj is IndicatorParameters))
				return false;
			else
				return Equals(obj as IndicatorParameters);
		}

		[DebuggerStepThrough()]
		public override int GetHashCode()
		{
			int hashCode = 17;

			for (int i = 0; i < this.List.Length; i++)
				if (this.List[i] != null)
					hashCode = hashCode * 23 + this.List[i].GetHashCode();

			return hashCode;
		}

		[DebuggerStepThrough()]
		public static bool operator ==(IndicatorParameters lhs, IndicatorParameters rhs)
		{
			if (System.Object.ReferenceEquals(lhs, rhs))
				return true;

			if (((object)lhs == null) || ((object)rhs == null))
				return false;

			return lhs.Equals(rhs);
		}

		[DebuggerStepThrough()]
		public static bool operator !=(IndicatorParameters lhs, IndicatorParameters rhs)
		{
			return !(lhs == rhs);
		}
	}
}
