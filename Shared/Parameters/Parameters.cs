using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace SteveBagnall.Trading.Shared.Parameters
{
	/// <summary>
	/// A sequence of DIFFERENT parameters - cannot be used to store different instances of the same parameter
	/// Are deemed to be equal if all parameters are equal and all the properties of the parameters are equal
	/// </summary>
	[Serializable()]
	public class Parameters : IEnumerable<Parameter>
	{
		private List<Parameter> _parameters = new List<Parameter>();

		public int Count
		{
			get { return _parameters.Count; }
		}

		public Parameters()
		{ }

		public Parameters(List<Parameter> Parameters)
		{
			foreach (Parameter parameter in Parameters)
			{
				if (_parameters.Contains(parameter))
					throw new ApplicationException("Must contain uniquely named parameters.");

				_parameters.Add(parameter);
			}
		}

		public Parameter this[int Index]
		{
			get { return _parameters[Index]; }
		}

		public Parameter Get(string Name)
		{
			Parameter parameter = _parameters.SingleOrDefault(p => p.Name == Name);
			
			if (parameter != default(Parameter))
				parameter.IsAccessed = true;
		
			return parameter;
		}

		private Parameter this[string Name]
		{
			get { return _parameters.SingleOrDefault(p => p.Name == Name); }
		}

		public void Add(Parameter Parameter)
		{
			if (!_parameters.Contains(Parameter))
				_parameters.Add(Parameter);
		}

		public void Remove(string ParameterName)
		{
			Parameter parameterToRemove = null;
			foreach (Parameter parameter in _parameters)
			{
				if (parameter.Name == ParameterName)
				{
					parameterToRemove = parameter;
					break;
				}
			}

			if (parameterToRemove != null)
				_parameters.Remove(parameterToRemove);
		}

		public void Remove(Parameter Parameter)
		{
			if (_parameters.Contains(Parameter))
				_parameters.Remove(Parameter);
		}

		public void AddRange(Parameters Parameters)
		{
			foreach (Parameter parameter in Parameters)
				Add(parameter);
		}

		public IEnumerator<Parameter> GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}

		public bool Equals(Parameters p)
		{
			if ((object)p == null)
				return false;

			// return true if the parameters and values are the same REGARDLESS OF ORDER

			if (this.Count() != p.Count())
				return false;
						
			foreach (Parameter parameter in this)
			{
				if (p.SingleOrDefault(e => e.Name == parameter.Name) == default(Parameter))
					return false;

				if ((parameter.PossibleValues != null) && (p[parameter.Name].PossibleValues != null))
				{
					if (parameter.PossibleValues.Count != p[parameter.Name].PossibleValues.Count)
						return false;

					foreach (object possibleValue in parameter.PossibleValues)
						if (!p[parameter.Name].PossibleValues.Contains(possibleValue))
							return false;
				}
				
				if (!(
					(p[parameter.Name].MinimumPerturbation == parameter.MinimumPerturbation)
					&& (p[parameter.Name].IsTesting == parameter.IsTesting)
					&& (p[parameter.Name].MaxValue == parameter.MaxValue)
					&& (p[parameter.Name].MinValue == parameter.MinValue)
					&& (p[parameter.Name].ParameterType == parameter.ParameterType)
					&& (p[parameter.Name].TestSequence == parameter.TestSequence)
					&& (p[parameter.Name].ValueType == parameter.ValueType)
					&& (Convert.ChangeType(p[parameter.Name].Value, p[parameter.Name].ValueType).Equals(Convert.ChangeType(parameter.Value, parameter.ValueType)))))
				{
					return false;
				}
			}

			return true;
		}

		[DebuggerStepThrough()]
		public override bool Equals(object obj)
		{
			if (!(obj is Parameters))
				return false;
			else
				return Equals(obj as Parameters);
		}

		public override int GetHashCode()
		{
			// hash code should be the same REGARDLESS OF ORDER

			int hashCode = 17;

			// TODO: how big is Int Max????
			foreach (Parameter parameter in this.OrderBy(p => p.Name))
			{
				hashCode = hashCode * 23 + parameter.Name.GetHashCode();
				hashCode = hashCode * 23 + parameter.MinimumPerturbation.GetHashCode();
				hashCode = hashCode * 23 + parameter.IsTesting.GetHashCode();
				hashCode = hashCode * 23 + parameter.MaxValue.GetHashCode();
				hashCode = hashCode * 23 + parameter.MinValue.GetHashCode();
				hashCode = hashCode * 23 + parameter.ParameterType.GetHashCode();

				foreach (object possibleValue in parameter.PossibleValues)
					hashCode = hashCode * 23 + possibleValue.GetHashCode();

				hashCode = hashCode * 23 + parameter.TestSequence.GetHashCode();
				hashCode = hashCode * 23 + parameter.ValueType.GetHashCode();
				hashCode = hashCode * 23 + Convert.ChangeType(parameter.Value, parameter.ValueType).GetHashCode();
			}

			return hashCode;
		}

		[DebuggerStepThrough()]
		public static bool operator ==(Parameters lhs, Parameters rhs)
		{
			if (System.Object.ReferenceEquals(lhs, rhs))
				return true;

			if (((object)lhs == null) || ((object)rhs == null))
				return false;

			return lhs.Equals(rhs);
		}

		[DebuggerStepThrough()]
		public static bool operator !=(Parameters lhs, Parameters rhs)
		{
			return !(lhs == rhs);
		}
	}
}
