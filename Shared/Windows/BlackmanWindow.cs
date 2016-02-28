using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteveBagnall.Trading.Shared.Windows
{
    public class BlackmanWindow : IWindow
    {
        private const double DEFAULT_ALPHA = 0.16;

        private double _alpha = DEFAULT_ALPHA;
        private double _a0;
        private double _a1;
        private double _a2;

        public BlackmanWindow() : this(DEFAULT_ALPHA)
        { }

        public BlackmanWindow(double Alpha)
        {
            _alpha = Alpha;
            _a0 = (1 - _alpha) / 2.0;
            _a1 = 0.5;
            _a2 = _alpha / 2.0;
        }

        public double ValueAt(int N, int n)
        {
            return _a0 - (_a1 * Math.Cos((2 * Math.PI * n) / (N - 1))) + _a2 * Math.Cos((4 * Math.PI * n) / (N - 1));
        }
    }
}
