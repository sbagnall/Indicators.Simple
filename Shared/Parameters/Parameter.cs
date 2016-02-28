using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace SteveBagnall.Trading.Shared.Parameters
{
	/// <summary>
	/// Are deemed to be equal if the parameter name only is equal
	/// </summary>
	[Serializable()]
	public class Parameter
	{
		private const decimal GEOMETRIC_PERTURBATION = 0.01M;
		private const int NUM_SAMPLES_PER_PARAMETER = 20;

		public string Name { get; private set; }
		public ParameterType ParameterType { get; private set; }

		public Type ValueType { get; private set; }
		public object Value { get; set; }
		public bool IsTesting { get; private set; }

		/// <summary>
		/// Set to true when Get is called in Parameters to access the parameter
		/// - saves testing objects who register parameters without using them
		/// </summary>
		public bool IsAccessed { get; set; }

		// continuous properties
		public decimal MinValue { get; private set; }
		public decimal MaxValue { get; private set; }
		public TestSequence TestSequence { get; private set; }
		public decimal MinimumPerturbation { get; private set; }
		public decimal GeometricRatio { get; private set; }

		// discrete properties
		public List<object> PossibleValues { get; private set; }
			

		public Parameter(string Name, Type ValueType, object Value, bool IsTesting, decimal MinValue, decimal MaxValue, TestSequence TestSequence)
		{
			this.ParameterType = ParameterType.Continuous;

			if ((Convert.ToDecimal(Value) < MinValue) || (Convert.ToDecimal(Value) > MaxValue))
				throw new ApplicationException("Value should be within range.");

			this.Name = Name;
			this.ValueType = ValueType;
			this.Value = Value;
			this.IsTesting = IsTesting;
			this.MinValue = MinValue;
			this.MaxValue = MaxValue;
			this.TestSequence = TestSequence;
			
			this.MinimumPerturbation = (MaxValue - MinValue) / (decimal)NUM_SAMPLES_PER_PARAMETER;

			decimal b = 0.0M;
			for (b = 1.0M + GEOMETRIC_PERTURBATION
				; ((int)Math.Floor(Math.Log((double)MaxValue, (double)b) - Math.Log((double)MinValue, (double)b)) + 1) > NUM_SAMPLES_PER_PARAMETER
				; b += GEOMETRIC_PERTURBATION)
				{}

			this.GeometricRatio = b;
		}

		public Parameter(string Name, Type ValueType, object Value, bool IsTesting, object[] PossibleValues)
		{
			this.ParameterType = ParameterType.Discrete;

			this.Name = Name;
			this.ParameterType = ParameterType;
			this.ValueType = ValueType;
			this.Value = Value;
			this.IsTesting = IsTesting;
			this.PossibleValues = new List<object>(PossibleValues) ;
		}

		private class FlagCombination
		{
			public List<IList<int>> Combinations { get; private set; }

			public FlagCombination()
			{
				this.Combinations = new List<IList<int>>();
			}

			/// <summary>
			/// This needs to be a list and not a bit-map as it is passed as a Action delegate to 
			/// <see cref="CombinatoricsUtilities.GetCombinations<T>(this IList<T>, Action<IList<T>>, int?, bool)"/>
			/// </summary>
			/// <param name="Combination"></param>
			public void AddCombination(IList<int> Combination)
			{
				this.Combinations.Add((IList<int>)Utilities.DeepClone(Combination));
			}
		}

		private void GetCombination(
			List<int> Flags,
			int NumInEachCombination,
			out FlagCombination FlagCombination)
		{
			FlagCombination = new FlagCombination();
			Flags.GetCombinations(
				FlagCombination.AddCombination,
				NumInEachCombination,
				false);
		}

		public Parameter(string Name, Type ValueType, object Value, bool IsTesting, Enum[] EnumeratedValues)
		{
			this.ParameterType = ParameterType.Discrete;

			this.Name = Name;
			this.ParameterType = ParameterType;
			this.ValueType = ValueType;
			this.Value = Value;
			this.IsTesting = IsTesting;
			this.PossibleValues = new List<object>();

			if (ValueType.GetCustomAttributes(typeof(FlagsAttribute), false).Count() > 0)
			{
				List<int> values = new List<int>();
				
				for (int i = 0; i < EnumeratedValues.Length; i++)
					if (!Convert.ToInt32(EnumeratedValues[i]).Equals(0))
						values.Add((int)Convert.ToInt32(EnumeratedValues[i]));

				for (int i = values.Count; i > 0; i--)
				{
					FlagCombination allCombinations = null;
					GetCombination(values, i, out allCombinations);

					if (allCombinations != null)
					{
						for (int combinationIndex = 0; combinationIndex < allCombinations.Combinations.Count; combinationIndex++)
						{
							IList<int> combination = allCombinations.Combinations[combinationIndex];

							int combinedValue = 0;

							foreach (int value in combination)
								combinedValue ^= value;

							if (!this.PossibleValues.Contains(combinedValue))
								this.PossibleValues.Add(combinedValue);
						}
					}
				}
			}
			else
			{
				foreach (Enum value in EnumeratedValues)
				{
					ulong intValue = Convert.ToUInt64(value);
					if (intValue != 0)
						this.PossibleValues.Add(value);
				}
			}
		}


		public bool Equals(Parameter p)
		{
			if ((object)p == null)
				return false;

			return (this.Name == p.Name);
		}

		[DebuggerStepThrough()]
		public override bool Equals(object obj)
		{
			if (!(obj is Parameter))
				return false;
			else
				return Equals(obj as Parameter);
		}

		public override int GetHashCode()
		{
			int hashCode = 17;

			hashCode = hashCode * 23 + this.Name.GetHashCode();

			return hashCode;
		}

		[DebuggerStepThrough()]
		public static bool operator ==(Parameter lhs, Parameter rhs)
		{
			if (System.Object.ReferenceEquals(lhs, rhs))
				return true;

			if (((object)lhs == null) || ((object)rhs == null))
				return false;

			return lhs.Equals(rhs);
		}

		[DebuggerStepThrough()]
		public static bool operator !=(Parameter lhs, Parameter rhs)
		{
			return !(lhs == rhs);
		}
	}
}
