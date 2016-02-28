using System;
using System.Configuration;

namespace SteveBagnall.Trading.Shared.Configuration.MarketConfig
{
    public class Market : ConfigurationElement
	{
		[ConfigurationProperty("pair", IsRequired = true)]
		public Symbol Pair
		{
			get { return (Symbol)this["pair"]; }
			set { this["pair"] = value; }
		}

		[ConfigurationProperty("pipValue", IsRequired = true)]
		public Decimal PipValue
		{
			get { return (Decimal)this["pipValue"]; }
			set { this["pipValue"] = value; }
		}

		[ConfigurationProperty("decimalPlaces", IsRequired = true)]
		public int DecimalPlaces
		{
			get { return (int)this["decimalPlaces"]; }
			set { this["decimalPlaces"] = value; }
		}

		[ConfigurationProperty("guaranteedCost", IsRequired = true)]
		public Decimal GuaranteedCost
		{
			get { return (Decimal)this["guaranteedCost"]; }
			set { this["guaranteedCost"] = value; }
		}

		[ConfigurationProperty("spread", IsRequired = true)]
		public Decimal Spread
		{
			get { return (Decimal)this["spread"]; }
			set { this["spread"] = value; }
		}

		[ConfigurationProperty("minimumDistance", IsRequired = true)]
		public Decimal MinimumDistance
		{
			get { return (Decimal)this["minimumDistance"]; }
			set { this["minimumDistance"] = value; }
		}

		[ConfigurationProperty("minBetInQuoteCurrency", IsRequired = false)]
		public Decimal MinBetInQuoteCurrency
		{
			get { return (Decimal)this["minBetInQuoteCurrency"]; }
			set { this["minBetInQuoteCurrency"] = value; }
		}

		[ConfigurationProperty("minBetInGBP", IsRequired = false)]
		public Decimal MinBetInGBP
		{
			get { return (Decimal)this["minBetInGBP"]; }
			set { this["minBetInGBP"] = value; }
		}
	}
}
