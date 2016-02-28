using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteveBagnall.Trading.Shared.QLearning
{
	public class DelayedQLearning : QLearningBase
	{
		private Decimal _epsilon;
		public Decimal Epsilon
		{
			get { return _epsilon; }
			private set { _epsilon = value; }
		}

		private Decimal _delta;
		public Decimal Delta
		{
			get { return _delta; }
			private set { _delta = value; }
		}

		private Decimal _epsilon1;
		public Decimal Epsilon1
		{
			get { return _epsilon1; }
			private set 
			{
				if (value < 0 || value > 1)
					throw new ApplicationException("Epsilon 1 must be between 0 and 1.");

				_epsilon1 = value; 
			}
		}

		private decimal _numSamplesNeeded;
		/// <summary>
		/// m
		/// </summary>
		public decimal NumSamplesNeeded
		{
			get { return _numSamplesNeeded; }
			private set
			{
				if (value < 0)
					throw new ApplicationException("NumMostRecentSamples must be positive.");

				_numSamplesNeeded = value;
			}
		}

		private Dictionary<IQState, Dictionary<IQAction, bool>> _learnFlags = new Dictionary<IQState,Dictionary<IQAction,bool>>();
		/// <summary>
		/// LEARN(s, a)
		/// </summary>
		public Dictionary<IQState, Dictionary<IQAction, bool>> LearnFlags
		{
			get { return _learnFlags; }
			private set { _learnFlags = value; }
		}
		
		private Dictionary<IQState, Dictionary<IQAction, Decimal>> _attemptedUpdates = new Dictionary<IQState, Dictionary<IQAction, decimal>>();
		/// <summary>
		/// U(s, a)
		/// </summary>
		public Dictionary<IQState, Dictionary<IQAction, Decimal>> AttemptedUpdates
		{
			get { return _attemptedUpdates; }
			private set { _attemptedUpdates = value; }
		}

		private Dictionary<IQState, Dictionary<IQAction, int>> _numSamplesInNextUpdate = new Dictionary<IQState, Dictionary<IQAction, int>>();
		/// <summary>
		/// l(s, a)
		/// </summary>
		public Dictionary<IQState, Dictionary<IQAction, int>> NumSamplesInNextUpdate
		{
			get { return _numSamplesInNextUpdate; }
			private set { _numSamplesInNextUpdate = value; }
		}

		private Dictionary<IQState, Dictionary<IQAction, int>> _timeOfLastAttemptedUpdate = new Dictionary<IQState, Dictionary<IQAction, int>>();
		/// <summary>
		/// t(s, a)
		/// </summary>
		public Dictionary<IQState, Dictionary<IQAction, int>> TimeOfLastAttemptedUpdate
		{
			get { return _timeOfLastAttemptedUpdate; }
			private set { _timeOfLastAttemptedUpdate = value; }
		}
		
		private int _timeOfMostRecentQChange = 0;
		/// <summary>
		/// t*
		/// </summary>
		public int TimeOfMostRecentQChange
		{
			get { return _timeOfMostRecentQChange; }
			private set { _timeOfMostRecentQChange = value; }
		}

		public DelayedQLearning(IQLearningModel Model, List<IQState> States, List<IQAction> Actions, Decimal Gamma, Decimal Epsilon, Decimal Delta)
			: base(Model, States, Actions, Gamma)
		{
			this.Epsilon = Epsilon;
			this.Delta = Delta;

			foreach (IQState state in States)
			{
				foreach (IQAction action in Actions)
				{
					if (!this.AttemptedUpdates.ContainsKey(state))
						this.AttemptedUpdates.Add(state, new Dictionary<IQAction, decimal>());
					this.AttemptedUpdates[state].Add(action, 0.0M);
					if (!this.NumSamplesInNextUpdate.ContainsKey(state))
						this.NumSamplesInNextUpdate.Add(state, new Dictionary<IQAction, int>());
					this.NumSamplesInNextUpdate[state].Add(action, 0);
					if (!this.TimeOfLastAttemptedUpdate.ContainsKey(state))
						this.TimeOfLastAttemptedUpdate.Add(state, new Dictionary<IQAction, int>());
					this.TimeOfLastAttemptedUpdate[state].Add(action, 0);
					if (!this.LearnFlags.ContainsKey(state))
						this.LearnFlags.Add(state, new Dictionary<IQAction, bool>());
					this.LearnFlags[state].Add(action, true);
				}
			}

			decimal epsilon1 = GetEpsilon1(Epsilon, Gamma); ;
			this.Epsilon1 = epsilon1;
			this.NumSamplesNeeded = GetNumSamplesNeeded(Gamma, epsilon1, States.Count, Actions.Count, Delta);
		}

		public override decimal InitialQ
		{
			get { return (1.0M / (1.0M - this.Gamma)); }
		}

		protected override void Transaction(IQState FromState, IQAction NextAction, Decimal Reward, IQState NextState)
		{
			if (this.LearnFlags[FromState][NextAction])
			{
				this.AttemptedUpdates[FromState][NextAction] += (Reward + (this.Gamma * GetMaxQ(NextState)));
				this.NumSamplesInNextUpdate[FromState][NextAction] += 1;

				if (this.NumSamplesInNextUpdate[FromState][NextAction] >= this.NumSamplesNeeded)
				{
					if ((this.QValues[FromState][NextAction] - (this.AttemptedUpdates[FromState][NextAction] / this.NumSamplesNeeded))
						>= (2.0M * this.Epsilon1))
					{
						this.QValues[FromState][NextAction] = (this.AttemptedUpdates[FromState][NextAction] / this.NumSamplesNeeded) + this.Epsilon1;
						this.TimeOfMostRecentQChange = this.Time;
					}
					else if (this.TimeOfLastAttemptedUpdate[FromState][NextAction] >= this.TimeOfMostRecentQChange)
					{
						this.LearnFlags[FromState][NextAction] = false;
					}

					this.TimeOfLastAttemptedUpdate[FromState][NextAction] = this.Time;
					this.AttemptedUpdates[FromState][NextAction] = 0.0M;
					this.NumSamplesInNextUpdate[FromState][NextAction] = 0;
				}
			}
			else if (this.TimeOfLastAttemptedUpdate[FromState][NextAction] < this.TimeOfMostRecentQChange)
			{
				this.LearnFlags[FromState][NextAction] = true;
			}
		}

		private Decimal GetEpsilon1(Decimal Epsilon, Decimal Gamma)
		{
			return (Epsilon * (1.0M - Gamma)) / 9.0M;
		}

		private Decimal GetNumSamplesNeeded(decimal Gamma, decimal Epsilon1, int NumStates, int NumActions, decimal Delta)
		{
			decimal k = 1.0M / ((1.0M - Gamma) * Epsilon1);

			return (decimal)(Math.Log((double)((3.0M * NumStates * NumActions * (1.0M + (NumStates * NumActions * k))) / Delta)))
				/ (2.0M * (decimal)Math.Pow((double)Epsilon1, 2.0) * (decimal)Math.Pow((double)(1.0M - Gamma), 2.0));
		}
	}
}
