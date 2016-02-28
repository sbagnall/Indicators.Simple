using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteveBagnall.Trading.Shared.Windows
{
    public class BartlettHannWindow : IWindow
    {
        private const double DEFAULT_A0 = 0.62;
        private const double DEFAULT_A1 = 0.48;
        private const double DEFAULT_A2 = 0.38;

        private double _a0 = DEFAULT_A0;
        private double _a1 = DEFAULT_A1;
        private double _a2 = DEFAULT_A2;

        public BartlettHannWindow() : this(DEFAULT_A0, DEFAULT_A1, DEFAULT_A2)
        { }

        public BartlettHannWindow(double a0, double a1, double a2)
        {
            _a0 = a0;
            _a1 = a1;
            _a2 = a2;
        }


        public double ValueAt(int N, int n)
        {
            return _a0 - (_a1 * Math.Abs((n / (N - 1)) - (0.5))) - _a2 * Math.Cos((2.0 * Math.PI * n) / (N - 1));
        }
    }
}
