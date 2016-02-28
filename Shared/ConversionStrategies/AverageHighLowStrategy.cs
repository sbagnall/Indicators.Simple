using System;

namespace SteveBagnall.Trading.Shared.ConversionStrategies
{
    public class AverageHighLowStrategy : OHLCVToDoubleBase
    {
        public override double Convert(OHLCV Value)
        {
            return ((double)Value.High + (double)Value.Low) / 2.0;
        }
    }
}
