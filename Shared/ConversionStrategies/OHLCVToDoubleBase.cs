using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Shared.ConversionStrategies
{
    public abstract class OHLCVToDoubleBase : IOHLCVToDoubleStrategy
    {
        public abstract double Convert(OHLCV Value);

        public List<double> Convert(List<OHLCV> Values)
        {
            List<double> retVal = new List<double>();

            foreach (OHLCV value in Values)
                retVal.Add(Convert(value));

            return retVal;
        }
    }
}
