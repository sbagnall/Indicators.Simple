using System;

namespace SteveBagnall.Trading.Shared.ConversionStrategies
{
    public class LowPriceStrategy : OHLCVToDoubleBase
    {
        public override double Convert(OHLCV Value)
        {
            return (double)Value.Low;
        }
    }
}
