using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteveBagnall.Trading.Shared.QLearning
{
	public abstract class QLearningBase
	{
		IQLearningModel _model;
		public IQLearningModel Model
		{
			get { return _model; }
			private set { _model = value; }
		}

		List<IQState> _states;
		public List<IQState> States
		{
			get { return _states; }
			private set { _states = value; }
		}

		List<IQAction> _actions;
		public List<IQAction> Actions
		{
			get { return _actions; }
			private set { _actions = value; }
		}

		private Decimal _gamma;
		public Decimal Gamma
		{
			get { return _gamma; }
			private set
			{
				if (value < 0 || value > 1)
					throw new ApplicationException("Gamma must be between 0 and 1.");

				_gamma = value;
			}
		}

		private Dictionary<IQState, Dictionary<IQAction, Decimal>> _qValues = new Dictionary<IQState, Dictionary<IQAction, decimal>>();
		/// <summary>
		/// Q(s, a)
		/// </summary>
		public Dictionary<IQState, Dictionary<IQAction, Decimal>> QValues
		{
			get { return _qValues; }
			private set { _qValues = value; }
		}

		private int _time = 0;
		/// <summary>
		/// t
		/// </summary>
		public int Time
		{
			get { return _time; }
			private set { _time = value; }
		}

		public QLearningBase(IQLearningModel Model, List<IQState> States, List<IQAction> Actions, Decimal Gamma)
		{
			this.Model = Model;

			this.States = States;
			this.Actions = Actions;
			this.Gamma = Gamma;

			foreach (IQState state in States)
			{
				foreach (IQAction action in Actions)
				{
					if (!this.QValues.ContainsKey(state))
						this.QValues.Add(state, new Dictionary<IQAction, decimal>());
					this.QValues[state].Add(action, this.InitialQ);
				}
			}
		}

		protected abstract void Transaction(IQState FromState, IQAction NextAction, Decimal Reward, IQState NextState);

		public abstract decimal InitialQ { get; }

		public void PerformTransition(IQState FromState)
		{
			++Time;

			IQAction nextAction = GetMaxAction(FromState);

			Decimal reward;
			IQState nextState;
			this.Model.ProcessStateAction(FromState, nextAction, out reward, out nextState);

			Transaction(FromState, nextAction, reward, nextState);
		}

		protected IQAction GetMaxAction(IQState State)
		{
			decimal v;
			IQAction action;
			GetMax(State, out v, out action);
			return action;
		}

		protected decimal GetMaxQ(IQState State)
		{
			decimal v;
			IQAction action;
			GetMax(State, out v, out action);
			return v;
		}

		protected void GetMax(IQState State, out Decimal V, out IQAction NextAction)
		{
			Decimal maxQ = Decimal.MinValue;
			IQAction maxAction = null;

			// TODO: there is a potential optimisation using a priority queue
			foreach (IQAction action in this.Actions)
			{
				if (this.QValues[State][action] > maxQ)
				{
					maxQ = this.QValues[State][action];
					maxAction = action;
				}
			}

			V = maxQ;

			if (maxQ != this.InitialQ)
				NextAction = maxAction;
			else
				NextAction = this.Actions[this.Time % this.Actions.Count];
		}
	}
}
