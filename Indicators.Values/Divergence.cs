using SteveBagnall.Trading.Indicators.Values.Contracts;
using SteveBagnall.Trading.Indicators.Values.Shared;
using SteveBagnall.Trading.Shared;
using System;

namespace SteveBagnall.Trading.Indicators.Values
{
    public class Divergence : ICloneable, ITrendReversalValue
    {
        public decimal? StudyPeakToPeakValue = null;
        public decimal? StudyPeakToPeakGradient = null;
        public decimal? StudyDipToDipValue = null;
        public decimal? StudyDipToDipGradient = null;
        public decimal? DivergencePeakToPeakValue = null;
        public decimal? DivergencePeakToPeakGradient = null;
        public decimal? DivergenceDipToDipValue = null;
        public decimal? DivergenceDipToDipInstantGradient = null;

        public bool IsLHAgainstHH = false;
        public bool IsHHAgainstLH = false;
        public bool IsLHAgainstHL = false;
        public bool IsHHagainstLL = false;
        public bool IsLLAgainstHH = false;
        public bool IsHLAgainstLH = false;
        public bool IsLLAgainstHL = false;
        public bool IsHLAgainstLL = false;

        public bool IsUp
        {
            get
            {
                return ((IsHHAgainstLH || IsHHagainstLL || IsHLAgainstLH || IsHLAgainstLL)
                    && (!IsLHAgainstHH && !IsLHAgainstHL && !IsLLAgainstHH && !IsLLAgainstHL));
            }
        }

        public bool IsDown
        {
            get
            {
                return ((IsLHAgainstHH || IsLHAgainstHL || IsLLAgainstHH || IsLLAgainstHL)
                    && (!IsHHAgainstLH && !IsHHagainstLL && !IsHLAgainstLH && !IsHLAgainstLL));
            }
        }

        public Direction Direction { get { return (this.IsUp) ? Direction.Up : ((this.IsDown) ? Direction.Down : Direction.Unknown); } }

        public Divergence()
        { }

        public Divergence(
            decimal? StudyPeakToPeakValue,
            decimal? StudyPeakToPeakGradient,
            decimal? StudyDipToDipValue,
            decimal? StudyDipToDipGradient,
            decimal? DivergencePeakToPeakValue,
            decimal? DivergencePeakToPeakGradient,
            decimal? DivergenceDipToDipValue,
            decimal? DivergenceDipToDipGradient)
        {
            this.StudyPeakToPeakValue = StudyPeakToPeakValue;
            this.StudyPeakToPeakGradient = StudyPeakToPeakGradient;
            this.StudyDipToDipValue = StudyDipToDipValue;
            this.StudyDipToDipGradient = StudyDipToDipGradient;
            this.DivergencePeakToPeakValue = DivergencePeakToPeakValue;
            this.DivergencePeakToPeakGradient = DivergencePeakToPeakGradient;
            this.DivergenceDipToDipValue = DivergenceDipToDipValue;
            this.DivergenceDipToDipInstantGradient = DivergenceDipToDipGradient;

            if ((StudyPeakToPeakGradient > 0) && (DivergencePeakToPeakGradient < 0))
                IsLHAgainstHH = true;
            if ((StudyPeakToPeakGradient < 0) && (DivergencePeakToPeakGradient > 0))
                IsHHAgainstLH = true;
            if ((StudyDipToDipGradient > 0) && (DivergencePeakToPeakGradient < 0))
                IsLHAgainstHL = true;
            if ((StudyDipToDipGradient < 0) && (DivergencePeakToPeakGradient > 0))
                IsHHagainstLL = true;
            if ((StudyPeakToPeakGradient > 0) && (DivergenceDipToDipGradient < 0))
                IsLLAgainstHH = true;
            if ((StudyPeakToPeakGradient < 0) && (DivergenceDipToDipGradient > 0))
                IsHLAgainstLH = true;
            if ((StudyDipToDipGradient > 0) && (DivergenceDipToDipGradient < 0))
                IsLLAgainstHL = true;
            if ((StudyDipToDipGradient < 0) && (DivergenceDipToDipGradient > 0))
                IsHLAgainstLL = true;
        }

        public object Clone()
        {
            return new Divergence(
                this.StudyPeakToPeakValue,
                this.StudyPeakToPeakGradient,
                this.StudyDipToDipValue,
                this.StudyDipToDipGradient,
                this.DivergencePeakToPeakValue,
                this.DivergencePeakToPeakGradient,
                this.DivergenceDipToDipValue,
                this.DivergenceDipToDipInstantGradient);
        }
    }
}
