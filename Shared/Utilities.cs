using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace SteveBagnall.Trading.Shared
{
    public static class Utilities
	{
		//public static void FileCopy(String SourcePath, String DestinationPath)
		//{
		//    Int32 buffersize = 1024 * 1024;
		//    FileStream input = new FileStream(SourcePath, FileMode.Open, FileAccess.Read, FileShare.None, 8, FileOptions.Asynchronous | FileOptions.SequentialScan);
		//    FileStream output = new FileStream(DestinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8, FileOptions.Asynchronous | FileOptions.SequentialScan);

		//    Int32 readsize = -1;
		//    Byte[] readbuffer = new Byte[buffersize];
		//    IAsyncResult asyncread;
		//    Byte[] writebuffer = new Byte[buffersize];
		//    IAsyncResult asyncwrite;

		//    output.SetLength(input.Length);

		//    readsize = input.Read(readbuffer, 0, readbuffer.Length);
		//    readbuffer = Interlocked.Exchange(ref writebuffer, readbuffer);

		//    while (readsize > 0)
		//    {
		//        asyncwrite = output.BeginWrite(writebuffer, 0, readsize, null, null);
		//        asyncread = input.BeginRead(readbuffer, 0, readbuffer.Length, null, null);

		//        output.EndWrite(asyncwrite);
		//        readsize = input.EndRead(asyncread);
		//        readbuffer = Interlocked.Exchange(ref writebuffer, readbuffer);
		//    }
		//}

		public static string FormatWith(this string format, object source)
		{
			return FormatWith(format, null, source);
		}

		public static string FormatWith(this string format, IFormatProvider provider, object source)
		{
			if (format == null)
				throw new ArgumentNullException("format");

			Regex r = new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
			  RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

			List<object> values = new List<object>();
			string rewrittenFormat = r.Replace(format, delegate(Match m)
			{
				Group startGroup = m.Groups["start"];
				Group propertyGroup = m.Groups["property"];
				Group formatGroup = m.Groups["format"];
				Group endGroup = m.Groups["end"];

				values.Add((propertyGroup.Value == "0")
				  ? source
				  : DataBinder.Eval(source, propertyGroup.Value));

				return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value
				  + new string('}', endGroup.Captures.Count);
			});

			return string.Format(provider, rewrittenFormat, values.ToArray());
		}

		public static decimal RoundToFraction(decimal Value, decimal Fraction)
		{
			Fraction = (Fraction > 1.0M) ? 1.0M : Fraction;
			int multiplier = (int)(1.0M / Fraction);
			return (decimal)Math.Round(multiplier * (double)Value, System.MidpointRounding.AwayFromZero) / multiplier;
		}

		public static Decimal RoundToSigFigs(decimal Value, int NumSigFigures, out int RoundingPosition, out decimal Scale)
		{
			// this method will return a rounded Decimal value at a number of signifigant figures.
			// the sigFigures parameter must be between 0 and 15, exclusive.

			RoundingPosition = default(int);   // The rounding position of the value at a number of sig figures.
			Scale = default(Decimal);           // Optionally used scaling value, for rounding whole numbers or decimals past 15 places

			// handle exceptional cases
			if (Value == 0.0M) { return Value; }
			if (NumSigFigures < 1 || NumSigFigures > 14) { throw new ArgumentOutOfRangeException("The sigFigures argument must be between 0 and 15 exclusive."); }

			// The resulting rounding position will be negative for rounding at whole numbers, and positive for decimal places.
			RoundingPosition = NumSigFigures - 1 - (int)(Math.Floor(Math.Log10(Math.Abs((double)Value))));

			// try to use a rounding position directly, if no scale is needed.
			// this is because the scale mutliplication after the rounding can introduce error, although 
			// this only happens when you're dealing with really tiny numbers, i.e 9.9e-14.
			if (RoundingPosition > 0 && RoundingPosition < 15)
			{
				return Math.Round(Value, RoundingPosition, MidpointRounding.AwayFromZero);
			}
			else
			{
				Scale = (decimal)Math.Pow(10, Math.Ceiling(Math.Log10(Math.Abs((double)Value))));
				return Math.Round(Value / Scale, NumSigFigures, MidpointRounding.AwayFromZero) * Scale;
			}
		}


        //public static int GetDecimalHashCode(decimal value)
        //{
        //    int[] bits = decimal.GetBits(value);
        //    int hash = 17;
        //    foreach (int x in bits)
        //    {
        //        hash = hash * 31 + x;
        //    }
        //    return hash;
        //}

        public static double Median(double[] aArray, int IndexStart, int Length)
        {
            List<double> aTmp = new List<double>();

            for (int i = IndexStart; i < (IndexStart + Length); i++)
                aTmp.Add(aArray[i]);

            aTmp.Sort();

            if (Length % 2 != 0)
                return aTmp[(Length - 1) / 2];
            else
            {
                int nTmp = (int)(Length / 2.0);
                return (aTmp[nTmp - 1] + aTmp[nTmp]) / 2.0;
            }
        }

        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
		{
			// Check if the target directory exists, if not, create it.
			if (Directory.Exists(target.FullName) == false)
				Directory.CreateDirectory(target.FullName);

			// Copy each file into it’s new directory.
			foreach (FileInfo fi in source.GetFiles())
				fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);

			// Copy each subdirectory using recursion.
			foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
			{
				DirectoryInfo nextTargetSubDir =
					target.CreateSubdirectory(diSourceSubDir.Name);
				CopyDirectory(diSourceSubDir, nextTargetSubDir);
			}
		}

        ///
        ///Calculates standard deviation of numbers in an ArrayList
        ///
        public static double StandardDeviation(List<double> num)
        {
            double SumOfSqrs = 0;
            double avg = num.Average();
            for (int i = 0; i < num.Count; i++)
            {
                SumOfSqrs += Math.Pow(((double)num[i] - avg), 2);
            }
            double n = (double)num.Count;
            return Math.Sqrt(SumOfSqrs / (n - 1));
        }

        public static double SquareError(OHLCV Prediction, OHLCV Actual)
        {
            return Math.Pow((double)Prediction.Close - (double)Actual.Close, 2);
        }

        public static Decimal UlcerIndex(List<Decimal> num)
		{
			Decimal max = Decimal.MinValue;
			Decimal cumulative = 0.0M;
			List<Decimal> r2s = new List<Decimal>();

			foreach (Decimal value in num)
			{
				cumulative += value;

				if (cumulative > max)
					max = cumulative;

				if (max != 0.0M)
					r2s.Add((Decimal)Math.Pow((double)(100.0M * ((cumulative - max) / max)), 2.0));
				else
					r2s.Add((Decimal)Math.Pow((double)(100.0M * ((cumulative - max))), 2.0));
			}

			Decimal totalR2 = 0.0M;

			foreach (Decimal r2 in r2s)
				totalR2 += r2;

			return (r2s.Count == 0) ? 0.0M : (Decimal)Math.Sqrt((double)(totalR2 / (Decimal)r2s.Count));
		}
        
		 /// <summary>
        /// Gets the covariance of the two variables.
        /// </summary>
        public static double Covariance(List<double> X, List<double> Y)
		{
           	if (X.Count != Y.Count)
				throw new ApplicationException("Need to be the same size.");

			if (X.Count < 2)
				throw new ApplicationException("insufficient data.");

			int n = X.Count;
            double mx = X.Average();
            double my = Y.Average();
            
            double C = 0.0;
            for (int i = 0; i < n; i++) {
                C += (X[i] - mx) * (Y[i] - my);
            }
            C = C / n;

            return C;
        }

		public static double Correlation(List<double> X, List<double> Y)
		{
			if (X.Count != Y.Count)
				throw new ApplicationException("Need to be the same size.");

			if (X.Count < 3)
				throw new ApplicationException("insufficient data.");

#if DEBUG
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("X|Y|");
			for (int i = 0; i < X.Count; i++)
				sb.AppendLine(String.Format("{0}|{1}|", X[i], Y[i]));

#endif

			return Covariance(X, Y) / (StandardDeviation(X) * StandardDeviation(Y));
		}

		public static object DeepClone(object source)
		{
			MemoryStream m = new MemoryStream();
			BinaryFormatter b = new BinaryFormatter();
			b.Serialize(m, source);
			m.Position = 0;
			return b.Deserialize(m);

		}

		
	}
}
