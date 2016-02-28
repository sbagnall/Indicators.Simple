using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
    public class IndicatorMESA : SimulatorOutputBase
	{
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>(); 
		public override List<SimulatorOutputBase> Dependencies 
		{
			get { return _dependencies; } 
		}

		public override string ToString()
		{
			return String.Format("MESA({0})", this.IndicatorParameters);
		}

		public IndicatorMESA()
			: this(new IndicatorParameters())
		{ }

		public IndicatorMESA(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			if (!Ind.MESA().IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new MESA();
					else
						prototype = (MESA)Ind.MESA()[1].Clone();

					Get(ref prototype, Ind);

					Ind.MESA()[0] = (MESA)prototype;

					Ind.MESA().IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			decimal smooth = 0.0M;
			decimal detrender = 0.0M;
			decimal q1 = 0.0M;
			decimal i1 = 0.0M;
			decimal jI = 0.0M;
			decimal jQ = 0.0M;
			decimal i2 = 0.0M;
			decimal q2 = 0.0M;
			decimal re = 0.0M;
			decimal im = 0.0M;
			decimal period = 0.0M;
			decimal smoothPeriod = 0.0M;
			decimal phase = 0.0M;
			decimal deltaPhase = 0.0M;

			if (Ind.Bar.CurrentBar > 6)
			{
				smooth = ((4.0M * ((Ind.Bar[0].High + Ind.Bar[0].Low) / 2.0M)) 
					+ (3.0M * ((Ind.Bar[1].High + Ind.Bar[1].Low) / 2.0M))
					+ (2.0M * ((Ind.Bar[2].High + Ind.Bar[2].Low) / 2.0M))
					+ ((Ind.Bar[3].High + Ind.Bar[3].Low) / 2.0M)
					) / 10.0M;

				detrender = ((0.0962M * smooth) + (0.5769M * Ind.MESA()[2].Smooth)
					- (0.5769M * Ind.MESA()[4].Smooth)
					- (0.0962M * Ind.MESA()[6].Smooth)
					) * ((0.075M * Ind.MESA()[1].Period) + 0.54M);

				// Compute InPhase and Quadrature components
				q1 = ((0.0962M * detrender) + (0.5769M * Ind.MESA()[2].Detrender)
					- (0.5769M * Ind.MESA()[4].Detrender)
					- (0.0962M * Ind.MESA()[6].Detrender)
					) * ((0.075M * Ind.MESA()[1].Period) + 0.54M);

				i1 = Ind.MESA()[3].Detrender;

				// Advance the phase of I1 and Q1 by 90º
				jI = ((0.0962M * i1) + (0.5769M * Ind.MESA()[2].I1)
					- (0.5769M * Ind.MESA()[4].I1)
					- (0.0962M * Ind.MESA()[6].I1)
					) * ((0.075M * Ind.MESA()[1].Period) + 0.54M);

				jQ = ((0.0962M * q1) + (0.5769M * Ind.MESA()[2].Q1) 
					- (0.5769M * Ind.MESA()[4].Q1)
					- (0.0962M * Ind.MESA()[6].Q1)
					) * ((0.075M * Ind.MESA()[1].Period) + 0.54M);

				// Phasor addition for 3-bar averaging
				i2 = i1 - jQ;
				q2 = q1 + jI;

				// Smooth the I and Q components before applying the discriminator
				i2 = (0.2M * i2) + (0.8M * Ind.MESA()[1].I2);
				q2 = (0.2M * q2) + (0.8M * Ind.MESA()[1].Q2);

				// Homodyne Discriminator
				re = (i2 * Ind.MESA()[1].I2) + (q2 + Ind.MESA()[1].Q2);
				im = (i2 + Ind.MESA()[1].Q2) - (q2 * Ind.MESA()[1].I2);

				re = (0.2M * re) + (0.8M * Ind.MESA()[1].Re);
				im = (0.2M * im) + (0.8M * Ind.MESA()[1].Im);

				if ((im != 0.0M) && (re != 0.0M))
					period = (360.0M * (decimal)Math.PI) / (180.0M * (decimal)(Math.Atan(Math.Abs((double)im / (double)re))));

				period = (0.2M * period) + (0.8M * Ind.MESA()[1].Period);

				smoothPeriod = (0.33M * period) + (0.67M * Ind.MESA()[1].SmoothPeriod);

				if (i1 != 0.0M)
					phase = (180.0M * (decimal)Math.Atan(Math.Abs((double)q1 / (double)i1))) / (decimal)Math.PI;
			
				deltaPhase = Ind.MESA()[1].Phase - phase;

				if (deltaPhase < 1.0M)
					deltaPhase = 1.0M;
			}
			
			(Prototype as MESA).Smooth = smooth;
			(Prototype as MESA).Period = period;
			(Prototype as MESA).SmoothPeriod = smoothPeriod;
			(Prototype as MESA).Detrender = detrender;
			(Prototype as MESA).I1 = i1;
			(Prototype as MESA).Q1 = q1;
			(Prototype as MESA).I2 = i2;
			(Prototype as MESA).Q2 = q2;
			(Prototype as MESA).Re = re;
			(Prototype as MESA).Im = im;
			(Prototype as MESA).Phase = phase;
			(Prototype as MESA).DeltaPhase = deltaPhase;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0} Period|Smooth Period|Detrender|Phase|Delta Phase|", this.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			MESA mesa = Data.MESA(IndicatorParameters)[Instant.ExposureDate];

			if (mesa != null)
				return String.Format("{0}|{1}|{2}|{3}|{4}|", mesa.Period, mesa.SmoothPeriod, mesa.Detrender, mesa.Phase, mesa.DeltaPhase);
			else
				return String.Format("{0}|{1}|{2}|{3}|{4}|", "", "", "", "", "");
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|{3}|{4}|", "", "", "", "", "");
		}
	}
}
