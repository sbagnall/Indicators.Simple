using System.Configuration;

namespace SteveBagnall.Trading.Shared.Configuration.MarketConfig
{
    public class MarketData : ConfigurationSection
	{
		private static MarketData settings = ConfigurationManager.GetSection("marketData") as MarketData;
		public static MarketData Settings
		{
			get { return MarketData.settings; }
		}

		[ConfigurationProperty("marketsNoCost", IsRequired = true)]
		public MarketCollection MarketsNoCost
		{
			get { return (MarketCollection)this["marketsNoCost"]; }
			set { this["marketsNoCost"] = value; }
		}

		[ConfigurationProperty("marketsWithCost", IsRequired = true)]
		public MarketCollection MarketsWithCost
		{
			get { return (MarketCollection)this["marketsWithCost"]; }
			set { this["marketsWithCost"] = value; }
		}
	}
}
