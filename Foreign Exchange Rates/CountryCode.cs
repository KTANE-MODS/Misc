

// ForeignExchangeRates, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// CountryCode
internal struct CountryCode
{
	public string code;

	public string ISO4217;

	public CountryCode(string code, string ISO4217)
	{
		this.code = code;
		this.ISO4217 = ISO4217;
	}
}
