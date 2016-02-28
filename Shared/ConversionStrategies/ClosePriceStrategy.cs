using System;

namespace SteveBagnall.Trading.Shared.ConversionStrategies
{
    public class ClosePriceStrategy : OHLCVToDoubleBase
    {
        public override double Convert(OHLCV Value)
        {
            return (double)Value.Close;
        }
    }
}
