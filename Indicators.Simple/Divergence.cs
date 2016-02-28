using SteveBagnall.Trading.Indicators.Contracts;
using SteveBagnall.Trading.Indicators.Shared;
using SteveBagnall.Trading.Indicators.Values;
using SteveBagnall.Trading.Indicators.Values.AggregatedLines;
using SteveBagnall.Trading.Indicators.Values.Contracts;
using System;
using System.Collections.Generic;

namespace SteveBagnall.Trading.Indicators
{
	public class IndicatorDivergence : SimulatorOutputBase
	{
		private List<SimulatorOutputBase> _dependencies = new List<SimulatorOutputBase>();
		public override List<SimulatorOutputBase> Dependencies
		{
			get { return _dependencies; }
		}


		public IndicatorDivergence(IDivergentIndicator StudyIndicator, IDivergentIndicator DivergenceIndicator)
			: this(new IndicatorParameters(StudyIndicator, DivergenceIndicator))
		{ }

		public IndicatorDivergence(IndicatorParameters IndicatorParameters)
			: base(IndicatorParameters)
		{
			this.Dependencies.Add((SimulatorOutputBase)IndicatorParameters.List[0]);
			this.Dependencies.Add((SimulatorOutputBase)IndicatorParameters.List[1]);
		}

		public override string ToString()
		{
			throw new NotImplementedException();
		}

		public override void GetAll(IIndicatorValues Ind)
		{
			IDivergentIndicator studyIndicator = (IDivergentIndicator)this.IndicatorParameters.List[0];
			IDivergentIndicator divergenceIndicator = (IDivergentIndicator)this.IndicatorParameters.List[1];

			if (!Ind.Divergence(studyIndicator, divergenceIndicator).IsPopulated)
			{
				int oldCurrentBar = Ind.Bar.CurrentBar;
				for (int i = 1; i <= Ind.Bar.MaxBar; i++)
				{
					Ind.Bar.CurrentBar = i;

					object prototype = null;
					if (i == 1)
						prototype = new Divergence();
					else
						prototype = (Divergence)Ind.Divergence(studyIndicator, divergenceIndicator)[1].Clone();

					Get(ref prototype, Ind);

					Ind.Divergence(studyIndicator, divergenceIndicator)[0] = (Divergence)prototype;

					Ind.Divergence(studyIndicator, divergenceIndicator).IsPopulated = true; // set here so instance is guaranteed to exits
				}

				Ind.Bar.CurrentBar = oldCurrentBar;
			}
		}

		public override void Get(ref object Prototype, IIndicatorValues Ind)
		{
			IDivergentIndicator studyIndicator = (IDivergentIndicator)this.IndicatorParameters.List[0];
			IDivergentIndicator divergenceIndicator = (IDivergentIndicator)this.IndicatorParameters.List[1];

			TrendLine studyPeakToPeak = studyIndicator.GetPeakToPeakLine(0, Ind);
			TrendLine studyDipToDip = studyIndicator.GetDipToDipLine(0, Ind);
			TrendLine divergencePeakToPeak = divergenceIndicator.GetPeakToPeakLine(0, Ind);
			TrendLine divergenceDipToDip = divergenceIndicator.GetDipToDipLine(0, Ind);

			(Prototype as Divergence).StudyPeakToPeakValue = (studyPeakToPeak == null) ? (decimal?)null : studyPeakToPeak.GetValueAt(Ind.Bar[0].Number);
			(Prototype as Divergence).StudyPeakToPeakGradient = (studyPeakToPeak == null) ? (decimal?)null : studyPeakToPeak.Gradient;
			(Prototype as Divergence).StudyDipToDipValue = (studyDipToDip == null) ? (decimal?)null : studyDipToDip.GetValueAt(Ind.Bar[0].Number);
			(Prototype as Divergence).StudyDipToDipGradient = (studyDipToDip == null) ? (decimal?)null : studyDipToDip.Gradient;
			(Prototype as Divergence).DivergencePeakToPeakValue = (divergencePeakToPeak == null) ? (decimal?)null : divergencePeakToPeak.GetValueAt(Ind.Bar[0].Number);
			(Prototype as Divergence).DivergencePeakToPeakGradient = (divergencePeakToPeak == null) ? (decimal?)null : divergencePeakToPeak.Gradient;
			(Prototype as Divergence).DivergenceDipToDipValue = (divergenceDipToDip == null) ? (decimal?)null : divergenceDipToDip.GetValueAt(Ind.Bar[0].Number);
			(Prototype as Divergence).DivergenceDipToDipInstantGradient = (divergenceDipToDip == null) ? (decimal?)null : divergenceDipToDip.Gradient;
		}

		protected override String GetIndicatorHeader(IndicatorParameters IndicatorParameters)
		{
			IDivergentIndicator studyIndicator = (IDivergentIndicator)IndicatorParameters.List[0];
			IDivergentIndicator divergenceIndicator = (IDivergentIndicator)IndicatorParameters.List[1];

			return String.Format("{0} P2P|{0} D2D|{1} P2P|{1} D2D|Up|Down|Reversal||", 
				studyIndicator.ToString(), 
				divergenceIndicator.ToString());
		}

		protected override string GetPresentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			Divergence divergence = Data.Divergence(IndicatorParameters)[Instant.ExposureDate];

			string upValue = Convert.ToString((Data as IIndicatorValues).Bar[Instant.ExposureDate].Close * 0.99M);
			string downValue = Convert.ToString((Data as IIndicatorValues).Bar[Instant.ExposureDate].Close * 1.01M);

			return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|",
				divergence.StudyPeakToPeakValue,
				divergence.StudyDipToDipValue,
				divergence.DivergencePeakToPeakValue,
				divergence.DivergenceDipToDipValue,
				(divergence.IsUp) ? upValue : "",
				(divergence.IsDown) ? downValue : "",
				(int)divergence.Direction,
				divergence.Direction);
		}

		protected override String GetAbsentDetail(IOutputInstant Instant, IIndicatorValues Data, IndicatorParameters IndicatorParameters)
		{
			return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|",
				"", "", "", "", "", "", "", "");
		}
	}
}
