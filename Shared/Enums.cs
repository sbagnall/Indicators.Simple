namespace SteveBagnall.Trading.Shared
{
    public enum ChartType
	{
		NotSet = 0,
		Close,
		Open,
		High,
		Low,
		Candle,
	}

	public enum FourierType
	{
		NotSet = 0,
		DFT,
		FFT,
	}
	

	

	public enum BinSize
	{
		NotSet = 0,
		OneMonth,
		OneWeek,
		OneDay,
		EightHours,
		FourHours,
		TwoHours,
		OneHour,
		HalfAnHour,
		FifteenMinutes,
		TenMinutes,
		FiveMinutes,
		ThreeMinutes,
		OneMinute,
	}

	public enum Symbol
	{
		NotSet = 0,
		AUD_CAD,
		AUD_CHF,
		AUD_JPY,
		AUD_NZD,
		AUD_USD,
		AUX_AUD,
		BCO_USD,
		CAD_CHF,
		CAD_JPY,
		CHF_JPY,
		ETX_EUR,
		EUR_AUD,
		EUR_CAD,
		EUR_CHF,
		EUR_CZK,
		EUR_DKK,
		EUR_GBP,
		EUR_HUF,
		EUR_JPY,
		EUR_NOK,
		EUR_NZD,
		EUR_PLN,
		EUR_SEK,
		EUR_TRY,
		EUR_USD,
		FRX_EUR,
		GBP_AUD,
		GBP_CAD,
		GBP_CHF,
		GBP_JPY,
		GBP_NZD,
		GBP_USD,
		GRX_EUR,
		HKX_HKD,
		JPX_JPY,
		NSX_USD,
		NZD_CAD,
		NZD_CHF,
		NZD_JPY,
		NZD_USD,
		SGD_JPY,
		SPX_USD,
		UDX_USD,
		UKX_GBP,
		USD_CAD,
		USD_CHF,
		USD_CZK,
		USD_DKK,
		USD_HKD,
		USD_HUF,
		USD_JPY,
		USD_MXN,
		USD_NOK,
		USD_PLN,
		USD_SEK,
		USD_SGD,
		USD_TRY,
		USD_ZAR,
		WTI_USD,
		XAG_USD,
		XAU_AUD,
		XAU_CHF,
		XAU_EUR,
		XAU_GBP,
		XAU_USD,
		ZAR_JPY,
	}

	
}
