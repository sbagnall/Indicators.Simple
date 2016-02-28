using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Shared;
using SteveBagnall.Trading.Shared.Configuration.MarketConfig;

namespace SteveBagnall.Trading.Indicators.Contracts
{
    public interface IIndicatorValues
    {
        Symbol Pair { get; }
        Market Market { get; }

        IIndicatorHistory<Bar> Bar { get; }
       
        IIndicatorHistory<ADX> ADXExponential(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<ADX> ADXExponential(int Period, decimal ThresholdADX);
        IIndicatorHistory<ADXRelative> ADXExponentialRelative(int Period);
        IIndicatorHistory<ADXRelative> ADXExponentialRelative(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<ADX> ADXSimple(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<ADX> ADXSimple(int Period, decimal ThresholdADX);
        IIndicatorHistory<ADXRelative> ADXSimpleRelative(int Period);
        IIndicatorHistory<ADXRelative> ADXSimpleRelative(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<ATR> ATR(int Period);
        IIndicatorHistory<ATR> ATR(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<CCI> CCI(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<CCI> CCI(int Period, int SmoothingPeriod, decimal ThresholdValue);
        IIndicatorHistory<Chop> Chop(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<Chop> Chop(int Period, int SmoothingPeriod, decimal Threshold);
        IIndicatorHistory<Complexity> Complexity(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<Complexity> Complexity(int Period, int SmoothingPeriod);
        IIndicatorHistory<CTI> CTI(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<CTI> CTI(int Period, int SmoothingPeriod);
        IIndicatorHistory<Divergence> Divergence(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<Divergence> Divergence(IDivergentIndicator StudyIndicator, IDivergentIndicator DivergenceIndicator);
        IIndicatorHistory<DoubleZeros> DoubleZeros(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<DoubleZeros> DoubleZeros(int ATRPeriod, decimal ATRMultiple, decimal ProximityFraction);
        IIndicatorHistory<EMAPair> EmaPair(int FastPeriod);
        IIndicatorHistory<EMAPair> EmaPair(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<EMAPair> EmaPair(int FastPeriod, int SlowPeriod);
        IIndicatorHistory<Stochastic> FastStochastic(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<Stochastic> FastStochastic(int Period, int SmoothingPeriod, decimal ThresholdPercentage);
        IIndicatorHistory<KeltnerChannel> KeltnerChannel(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<KeltnerChannel> KeltnerChannel(int Period, int ATRPeriod, decimal ATRMultiple, decimal MarginThreshold, decimal ProximityMultiple);
        IIndicatorHistory<MamaFama> MamaFama(int Period);
        IIndicatorHistory<MamaFama> MamaFama(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<MESA> MESA();
        IIndicatorHistory<MESA> MESA(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<MomentumAbsolute> MomentumAbsolute(int Period);
        IIndicatorHistory<MomentumAbsolute> MomentumAbsolute(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<MomentumRelative> MomentumRelative(int Period);
        IIndicatorHistory<MomentumRelative> MomentumRelative(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<PFE> PFE(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<PFE> PFE(int Period, int SmoothingPeriod, decimal ThresholdPercentage);
        IIndicatorHistory<RSI> RSI(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<RSI> RSI(int Period, decimal ThresholdPercentage);
        IIndicatorHistory<StandardDeviation> SD(int Period);
        IIndicatorHistory<StandardDeviation> SD(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<Stochastic> SlowStochastic(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<Stochastic> SlowStochastic(int Period, int FirstSmoothingPeriod, int SecondSmoothingPeriod, decimal ThresholdPercentage);
        IIndicatorHistory<SMAPair> SMAPair(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<SMAPair> SMAPair(int FastPeriod, int SlowPeriod);
        IIndicatorHistory<Trends> Trends(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<Trends> Trends(IDivergentIndicator DivergentIndicator);
        IIndicatorHistory<TrueRange> TrueRange(int Period);
        IIndicatorHistory<TrueRange> TrueRange(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<Volatility> Volatility(int Period);
        IIndicatorHistory<Volatility> Volatility(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<VolatilityChannel> VolatilityChannel(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<VolatilityChannel> VolatilityChannel(int Period, int VolatilityPeriod, decimal VolatilityMultiple, decimal MarginThreshold, decimal ProximityMultiple);
        IIndicatorHistory<VPFEChannel> VPFEChannel(IndicatorParameters IndicatorParameters);
        IIndicatorHistory<VPFEChannel> VPFEChannel(int Period, int VPeriod, decimal VMultiple, int PFEPeriod, int PFESmoothingPeriod, decimal PFEThresholdPercentage, decimal PFEMultiple, decimal MarginThreshold, decimal ProximityMultiple);

    }
}
