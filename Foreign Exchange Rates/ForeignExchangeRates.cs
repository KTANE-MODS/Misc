

// ForeignExchangeRates, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// ForeignExchangeRates
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ForeignExchangeRates : MonoBehaviour
{
	private enum LEDType
	{
		OFF,
		NO_INTERNET,
		INTERNET_ON
	}

	public KMSelectable[] buttons;

	private int correctIndex;

	private bool isActivated;

	private bool hasRetreivedExchangeRate;

	private bool isReadyForInput;

	private CountryCode baseCountry;

	private CountryCode targetCountry;

	private int currencyAmount;

	private int answer;

	private ExchangeRates exchangeRates;

	private static float CHANCE_FOR_ISO_DISPLAY = 0.2f;

	private static float CHANCE_FOR_NO_INTERNET = 0.3f;

	private static string CURRENCY_API_ENDPOINT = "http://api.fixer.io";

	private static CountryCode[] COUNTRY_CODES = new CountryCode[31]
	{
		new CountryCode("AUD", "036"),
		new CountryCode("BGN", "975"),
		new CountryCode("BRL", "986"),
		new CountryCode("CAD", "124"),
		new CountryCode("CHF", "756"),
		new CountryCode("CNY", "156"),
		new CountryCode("DKK", "208"),
		new CountryCode("EUR", "978"),
		new CountryCode("GBP", "826"),
		new CountryCode("HKD", "344"),
		new CountryCode("HRK", "191"),
		new CountryCode("HUF", "348"),
		new CountryCode("IDR", "360"),
		new CountryCode("ILS", "376"),
		new CountryCode("INR", "356"),
		new CountryCode("JPY", "392"),
		new CountryCode("KRW", "410"),
		new CountryCode("MXN", "484"),
		new CountryCode("MYR", "458"),
		new CountryCode("NOK", "578"),
		new CountryCode("NZD", "554"),
		new CountryCode("PHP", "608"),
		new CountryCode("PLN", "985"),
		new CountryCode("RON", "946"),
		new CountryCode("RUB", "643"),
		new CountryCode("SEK", "752"),
		new CountryCode("SGD", "702"),
		new CountryCode("THB", "764"),
		new CountryCode("TRY", "949"),
		new CountryCode("USD", "840"),
		new CountryCode("ZAR", "710")
	};

	private void Start()
	{
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Expected O, but got Unknown
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Expected O, but got Unknown
		baseCountry = COUNTRY_CODES[Random.Range(0, COUNTRY_CODES.Length)];
		targetCountry = baseCountry;
		currencyAmount = Random.Range(1, 1000);
		while (targetCountry.code == baseCountry.code)
		{
			targetCountry = COUNTRY_CODES[Random.Range(0, COUNTRY_CODES.Length)];
		}
		int num = ((countBatteries() > 1) ? 3 : 0);
		int num2 = ((num != 3) ? 3 : 0);
		if (Random.value <= CHANCE_FOR_ISO_DISPLAY)
		{
			for (int i = 0; i < 3; i++)
			{
				setButtonLabel(num + i, baseCountry.ISO4217[i].ToString());
			}
		}
		else
		{
			for (int j = 0; j < 3; j++)
			{
				setButtonLabel(num + j, baseCountry.code[j].ToString());
			}
		}
		for (int k = 0; k < 3; k++)
		{
			setButtonLabel(num2 + k, targetCountry.code[k].ToString());
		}
		string text = currencyAmount.ToString("000");
		for (int l = 0; l < 3; l++)
		{
			setButtonLabel(6 + l, text[l].ToString());
		}
		for (int m = 0; m < buttons.Length; m++)
		{
			setButtonLED(m, LEDType.OFF);
			int buttonIndex = m;
			KMSelectable obj = buttons[m];
			obj.OnInteract = (OnInteractHandler)Delegate.Combine((Delegate)(object)obj.OnInteract, (Delegate)(OnInteractHandler)delegate
			{
				OnPress(buttonIndex);
				return false;
			});
		}
		KMBombModule component = ((Component)this).GetComponent<KMBombModule>();
		component.OnActivate = (KMModuleActivateEvent)Delegate.Combine((Delegate)(object)component.OnActivate, (Delegate)new KMModuleActivateEvent(ActivateModule));
	}

	private int countBatteries()
	{
		int num = 0;
		List<string> list = ((Component)this).GetComponent<KMBombInfo>().QueryWidgets(KMBombInfo.QUERYKEY_GET_BATTERIES, (string)null);
		foreach (string item in list)
		{
			Dictionary<string, int> dictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(item);
			num += dictionary["numbatteries"];
		}
		return num;
	}

	private void setButtonLabel(int buttonIndex, string label)
	{
		((Component)buttons[buttonIndex]).GetComponentInChildren<TextMesh>().text = label;
	}

	private void setButtonLED(int buttonIndex, LEDType led)
	{
		KMSelectable val = buttons[buttonIndex];
		Transform val2 = ((Component)val).transform.Find("Key");
		((Component)val2.Find("LED_Off")).gameObject.SetActive(led == LEDType.OFF);
		((Component)val2.Find("LED_Wrong")).gameObject.SetActive(led == LEDType.NO_INTERNET);
		((Component)val2.Find("LED_Correct")).gameObject.SetActive(led == LEDType.INTERNET_ON);
	}

	private void ActivateModule()
	{
		isActivated = true;
		((MonoBehaviour)this).StartCoroutine(loadingLEDSequence());
		if (Random.value <= CHANCE_FOR_NO_INTERNET)
		{
			((MonoBehaviour)this).StartCoroutine(updateForeignExchangeRateAnswer());
		}
		else
		{
			((MonoBehaviour)this).StartCoroutine(updateISO4217Answer());
		}
	}

	private IEnumerator loadingLEDSequence()
	{
		int buttonIndex = 0;
		while (!isReadyForInput)
		{
			setButtonLED(buttonIndex, LEDType.OFF);
			setButtonLED((buttonIndex + 1) % buttons.Length, LEDType.NO_INTERNET);
			buttonIndex++;
			if (buttonIndex == buttons.Length)
			{
				buttonIndex = 0;
			}
			yield return (object)new WaitForSeconds(0.2f);
		}
	}

	private int calculateAnswer(float exRate)
	{
		float num = (float)currencyAmount * exRate;
		string text = Mathf.Floor(num).ToString();
		if (num >= 10f)
		{
			return int.Parse(text[1].ToString());
		}
		return 0;
	}

	private IEnumerator updateForeignExchangeRateAnswer()
	{
		Debug.Log((object)"connecting..");
		yield return (object)new WaitForSeconds(Random.Range(0f, 5f));
		string response = string.Empty;
		int attemptsLeft = 5;
		while (attemptsLeft > 0)
		{
			WWW query = new WWW(CURRENCY_API_ENDPOINT + "/latest?symbols=" + targetCountry.code + "&base=" + baseCountry.code);
			yield return query;
			yield return (object)new WaitForSeconds(2f);
			attemptsLeft--;
			if (query.isDone && string.IsNullOrEmpty(query.error) && query.size > 0)
			{
				response = query.text;
				break;
			}
		}
		if (!string.IsNullOrEmpty(response))
		{
			yield return (object)new WaitForSeconds(Random.Range(0f, 20f));
			isReadyForInput = true;
			hasRetreivedExchangeRate = true;
			for (int i = 0; i < buttons.Length; i++)
			{
				setButtonLED(i, LEDType.INTERNET_ON);
			}
			exchangeRates = JsonConvert.DeserializeObject<ExchangeRates>(response);
			float rate = exchangeRates.rates[targetCountry.code];
			answer = calculateAnswer(rate);
		}
		else
		{
			((MonoBehaviour)this).StartCoroutine(updateISO4217Answer());
		}
	}

	private IEnumerator updateISO4217Answer()
	{
		yield return (object)new WaitForSeconds(Random.Range(0f, 30f));
		isReadyForInput = true;
		hasRetreivedExchangeRate = false;
		for (int i = 0; i < buttons.Length; i++)
		{
			setButtonLED(i, LEDType.NO_INTERNET);
		}
		answer = int.Parse(targetCountry.ISO4217[targetCountry.ISO4217.Length - 2].ToString());
	}

	private void OnPress(int buttonIndex)
	{
		if (!isActivated || !isReadyForInput)
		{
			((Component)this).GetComponent<KMBombModule>().HandleStrike();
		}
		else if (buttonIndex + 1 == answer || (answer == 0 && buttonIndex == 0))
		{
			((Component)this).GetComponent<KMBombModule>().HandlePass();
		}
		else
		{
			((Component)this).GetComponent<KMBombModule>().HandleStrike();
		}
	}
}
