using System;

namespace SteveBagnall.Trading.Shared.ConversionStrategies
{
    public class HighPriceStrategy : OHLCVToDoubleBase
    {
        public override double Convert(OHLCV Value)
        {
            return (double)Value.High;
        }
    }
}
