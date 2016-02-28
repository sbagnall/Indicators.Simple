using System;

namespace SteveBagnall.Trading.Shared.ConversionStrategies
{
    public class OpenPriceStrategy : OHLCVToDoubleBase
    {
        public override double Convert(OHLCV Value)
        {
            return (double)Value.Open;
        }
    }
}
