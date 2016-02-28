using System;

namespace SteveBagnall.Trading.Indicators.Shared
{
    [Flags()]
    public enum PriceLevelOptions
    {
        NotSet = 0,
        PriceLevels = 1,
        DoubleZeros = 1 << 1,
    }
}
