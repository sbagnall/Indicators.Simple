using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SteveBagnall.Trading.Shared.Parameters
{
	public class ParameterEngine
	{
		private static ParameterEngine instance;

		private Parameters OriginalParameters { get; set; }

		private Parameters _mutableParameters = new Parameters();
		public Parameters MutableParameters 
		{
			get { return _mutableParameters; }
			set
			{
				_mutableParameters = value;

				// reset current parameters
				_currentParameters = null;
			}
		}

		private ParameterEngine() 
		{
			this.OriginalParameters = new Parameters();
		}

		public static ParameterEngine Instance
		{
			get
			{
				if (instance == null)
					instance = new ParameterEngine();

				return instance;
			}
		}

		public void Register(IParametized ParametizedObject)
		{
			foreach (Parameter parameter in ParametizedObject.Parameters)
			{
				this.OriginalParameters.Add(parameter);

				if (parameter.IsTesting)
					this.MutableParameters.Add((Parameter)Utilities.DeepClone(parameter));
			}

			// reset current parameters
			_currentParameters = null;
		}

		private Parameters _currentParameters = null;
		public Parameters CurrentParameters
		{
			get
			{
				if (_currentParameters == null)
				{
					_currentParameters = new Parameters();

					foreach (Parameter parameter in this.OriginalParameters)
					{
						if (this.MutableParameters.Contains(parameter))
							_currentParameters.Add(this.MutableParameters.Single(p => p.Name == parameter.Name));
						else
							_currentParameters.Add(parameter);
					}
				}

				return _currentParameters;
			}
		}
	}
}

	