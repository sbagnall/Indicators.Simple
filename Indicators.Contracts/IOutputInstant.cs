using SteveBagnall.Trading.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Contracts
{
    public interface IOutputInstant
	{
		Symbol Pair { get; }
		DateTime ExposureDate { get; }
	}
}
