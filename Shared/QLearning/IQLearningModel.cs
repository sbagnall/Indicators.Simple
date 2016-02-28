using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteveBagnall.Trading.Shared.QLearning
{
	public interface IQLearningModel
	{
		void ProcessStateAction(IQState FromState, IQAction NextAction, out Decimal Reward, out IQState NextState);
	}
}
