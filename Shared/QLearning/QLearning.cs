using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteveBagnall.Trading.Shared.QLearning
{
	public class QLearning : QLearningBase
	{
		private Dictionary<IQState, Dictionary<IQAction, int>> _numQ = new Dictionary<IQState, Dictionary<IQAction, int>>();
		/// <summary>
		/// noQ(s, a)
		/// </summary>
		public Dictionary<IQState, Dictionary<IQAction, int>> NumQ
		{
			get { return _numQ; }
			private set { _numQ = value; }
		}

		public QLearning(IQLearningModel Model, List<IQState> States, List<IQAction> Actions, Decimal Gamma) 
			: base(Model, States, Actions, Gamma)
		{
			foreach (IQState state in States)
			{
				foreach (IQAction action in Actions)
				{
					if (!this.NumQ.ContainsKey(state))
						this.NumQ.Add(state, new Dictionary<IQAction, int>());
					this.NumQ[state].Add(action, 0);
				}
			}
		}

		public override decimal InitialQ
		{
			get { return 0.0M; }
		}

		protected override void Transaction(IQState FromState, IQAction NextAction, Decimal Reward, IQState NextState)
		{
			decimal total = (Reward + (this.Gamma * GetMaxQ(NextState)));

			++this.NumQ[FromState][NextAction];
			decimal alphaQ = GetAlphaQ(this.NumQ[FromState][NextAction]);

			this.QValues[FromState][NextAction] = ((1.0M - alphaQ) * this.QValues[FromState][NextAction]) 
				+ (GetAlphaQ(this.NumQ[FromState][NextAction]) * (Reward + (this.Gamma * GetMaxQ(NextState))));
		}

		private decimal GetAlphaQ(int Length)
		{
			return ( 1.0M / (decimal)Length);                    
		}
	}
}
