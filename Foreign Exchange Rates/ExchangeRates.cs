

// ForeignExchangeRates, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// ExchangeRates
using System.Collections.Generic;

internal struct ExchangeRates
{
	public string _base;

	public string date;

	public Dictionary<string, float> rates;
}
