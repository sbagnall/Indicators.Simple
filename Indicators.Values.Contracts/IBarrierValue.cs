using SteveBagnall.Trading.Indicators.Values.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values.Contracts
{
    public interface IBarrierValue
	{
		int BarrierUniqueKey { get; }

		Type Type { get; }

		BarrierDirection BarrierDirection { get; }

		decimal CongestionDistance { get; set; }

		decimal ProximateDistance { get; set; }

		decimal FailureDistanceMultiple { get; }

		bool IsCongested { get; set; }

		CongestionBoundary CongestedBoundary { get; set; }


		decimal GetValueAt(int BarNumber);

		decimal GetProximityUpperBoundaryAt(int BarNumber);

		decimal GetProximityLowerBoundaryAt(int BarNumber);

		decimal GetFailureUpperBoundaryAt(int BarNumber);

		decimal GetFailureLowerBoundaryAt(int BarNumber);
	}
}
