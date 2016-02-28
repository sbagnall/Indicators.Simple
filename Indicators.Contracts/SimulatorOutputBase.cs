using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace SteveBagnall.Trading.Indicators.Contracts
{
    public abstract class SimulatorOutputBase : ISimulatorOutput, IIndicator
	{
		public List<IOutputInstant> Instants { get; private set; }

		public IndicatorParameters IndicatorParameters { get; private set; }

		public abstract List<SimulatorOutputBase> Dependencies { get; }

		[DebuggerStepThrough()]
		public SimulatorOutputBase(IndicatorParameters IndicatorParameters)
		{
			this.IndicatorParameters = IndicatorParameters;
		}

		public override abstract string ToString();

		public void Initialise(List<IOutputInstant> AllInstants)
		{
			this.Instants = AllInstants;
		}

		public abstract void Get(ref object Prototype, IIndicatorValues Ind);

		public abstract void GetAll(IIndicatorValues Ind);

		protected abstract string GetIndicatorHeader(IndicatorParameters IndicatorParameters);

		protected virtual string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data)
		{
			return GetPresentDetail(Instant, Data, this.IndicatorParameters);
		}

		protected abstract string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters);

		protected virtual string GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data)
		{
			return GetAbsentDetail(Instant, Data, this.IndicatorParameters);
		}

		protected abstract string GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters);

		public string GetHeader()
		{
			return GetIndicatorHeader(this.IndicatorParameters);
		}

		public string GetDetail(IOutputInstant Instant, params IIndicatorValues[] Data)
		{
			StringBuilder sb = new StringBuilder();

			foreach (IIndicatorValues item in Data)
			{
				if (this.Instants.Contains(Instant))
				{
					string presentData;
					if (this.IndicatorParameters != null)
						presentData = GetPresentDetail(Instant, item, this.IndicatorParameters);
					else
						presentData = GetPresentDetail(Instant, item);

					if (presentData != null)
						sb.Append(presentData);
				}
				else
				{
					string absentData;
					if (this.IndicatorParameters != null)
						absentData = GetAbsentDetail(Instant, item);
					else
						absentData = GetAbsentDetail(Instant, item, this.IndicatorParameters);

					if (absentData != null)
						sb.Append(absentData);
				}
			}

			return sb.ToString();
		}

		[DebuggerStepThrough()]
		public bool Equals(SimulatorOutputBase s)
		{
			if ((object)s == null)
				return false;

			return ((this.IndicatorParameters == s.IndicatorParameters)
				&& (this.GetType() == s.GetType()));
		}

		[DebuggerStepThrough()]
		public override bool Equals(object obj)
		{
			if (!(obj is SimulatorOutputBase))
				return false;
			else
				return Equals(obj as SimulatorOutputBase);
		}

		[DebuggerStepThrough()]
		public override int GetHashCode()
		{
			int hashCode = 17;
			hashCode = hashCode * 23 + this.GetType().GetHashCode();
			hashCode = hashCode * 23 + this.IndicatorParameters.GetHashCode();
			return hashCode;
		}

		[DebuggerStepThrough()]
		public static bool operator ==(SimulatorOutputBase lhs, SimulatorOutputBase rhs)
		{
			if (System.Object.ReferenceEquals(lhs, rhs))
				return true;

			if (((object)lhs == null) || ((object)rhs == null))
				return false;

			return lhs.Equals(rhs);
		}

		[DebuggerStepThrough()]
		public static bool operator !=(SimulatorOutputBase lhs, SimulatorOutputBase rhs)
		{
			return !(lhs == rhs);
		}
	}
}
