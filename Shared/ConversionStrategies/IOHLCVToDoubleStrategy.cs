using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Shared.ConversionStrategies
{
    public interface IOHLCVToDoubleStrategy
    {
        double Convert(OHLCV Value);

        List<double> Convert(List<OHLCV> Values);
    }
}
