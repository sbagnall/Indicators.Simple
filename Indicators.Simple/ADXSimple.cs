using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorADXSimple : ADXBase
	{
		public override string ToString()
		{
			return String.Format("Smp.ADX({0})", this.IndicatorParameters);
		}

		public IndicatorADXSimple(int Period, decimal ThresholdADX)
			: this(new IndicatorParameters(Period, ThresholdADX))
		{
		}

		public IndicatorADXSimple(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			decimal thresholdADX = (decimal)this.IndicatorParameters.List[1];

			if (!Ind.ADXSimple(period, thresholdADX).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new ADX(thresholdADX);
					else
						prototype = (ADX)Ind.ADXSimple(period, thresholdADX)[1].Clone();

					Get(ref prototype, Ind);

					Ind.ADXSimple(period, thresholdADX)[0] = (ADX)prototype;

					Ind.ADXSimple(period, thresholdADX).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			int period = (int)this.IndicatorParameters.List[0];
			decimal thresholdADX = (decimal)this.IndicatorParameters.List[1];

			Decimal plusDM = 0.0M;
			Decimal minusDM = 0.0M;
			Decimal plusSDI = 0.0M;
			Decimal minusSDI = 0.0M;
			Decimal plusDI = 0.0M;
			Decimal minusDI = 0.0M;
			Decimal dx = 0.0M;
			Decimal adx = 0.0M;

			if (Ind.Bar.CurrentBar > period)
			{
				plusDM = (Ind.Bar[0].High > Ind.Bar[1].High) ? Ind.Bar[0].High - Ind.Bar[1].High : 0.0M;
				minusDM = (Ind.Bar[0].Low < Ind.Bar[1].Low) ? Ind.Bar[1].Low - Ind.Bar[0].Low : 0.0M;

				if (plusDM > minusDM)
					minusDM = 0.0M;

				if (minusDM > plusDM)
					plusDM = 0.0M;

				if (minusDM == plusDM)
				{
					plusDM = 0.0M;
					minusDM = 0.0M;
				}

				decimal tr = Ind.TrueRange(this.TrueRangePeriod)[0].Value;

				if (tr != 0.0M)
				{
					plusSDI = (plusDM * 100.0M) / tr;
					minusSDI = (minusDM * 100.0M) / tr;
				}
				else
				{
					plusSDI = 0.0M;
					minusSDI = 0.0M;
				}

				List<decimal> plusSDIs = new List<decimal>();
				List<decimal> minusSDIs = new List<decimal>();
				List<decimal> DXs = new List<decimal>();
				for (int i = 0 + 1; i < period; i++)
				{
					plusSDIs.Add(Ind.ADXSimple(period, thresholdADX)[i].PlusSDI);
					minusSDIs.Add(Ind.ADXSimple(period, thresholdADX)[i].MinusSDI);
					DXs.Add(Ind.ADXSimple(period, thresholdADX)[i].DX);
				}

				plusSDIs.Add(plusSDI);
				minusSDIs.Add(minusSDI);

				plusDI = plusSDIs.Average();
				minusDI = minusSDIs.Average();
			
				dx = ((plusDI + minusDI) == 0.0M) ? 0.0M : (Math.Abs(plusDI - minusDI) / (plusDI + minusDI)) * 100.0M;

				DXs.Add(dx);
				adx = DXs.Average();
			}

			(Prototype as ADX).PlusSDI = plusSDI; 
			(Prototype as ADX).MinusSDI = minusSDI;
			(Prototype as ADX).PlusDI = plusDI;
			(Prototype as ADX).MinusDI = minusDI;
			(Prototype as ADX).DX = dx;
			(Prototype as ADX).ADXValue = adx;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0} -DI|+DI|ADX|Trendiness||", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			ADX adx = Data.ADXSimple(IndicatorParameters)[Instant.ExposureDate];

			if (adx != null)
				return String.Format("{0}|{1}|{2}|{3}|{4}|", adx.MinusDI, adx.PlusDI, adx.ADXValue, (int)adx.Trendiness, adx.Trendiness);
			else
				return String.Format("{0}|{1}|{2}|{3}|{4}|", "", "", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|{3}|{4}|", "", "", "", "", "");
		}
	}
}
