

// errorCodes, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// ErrorCodes
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ErrorCodes : MonoBehaviour
{
	private static string ERROR_PREFIX = "ERROR:";

	private static string FIX_PREFIX = "ENTER FIX: ";

	private static string[] NUMBER_STR = new string[16]
	{
		"0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
		"A", "B", "C", "D", "E", "F"
	};

	public KMBombInfo BombInfo;

	public KMBombModule BombModule;

	public KMAudio Audio;

	public KMSelectable[] numberButtons;

	public KMSelectable sendButton;

	public TextMesh[] numberButtonText;

	public TextMesh sendButtonText;

	private Color defaultTextColor;

	public TextMesh errorText;

	public TextMesh fixText;

	private static int moduleIdCounter = 1;

	private int moduleId;

	private bool isSolved;

	private bool lightsOn;

	private string errorCodeString = string.Empty;

	private string userEnteredFix = string.Empty;

	private string solution = string.Empty;

	private void Start()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		moduleId = moduleIdCounter++;
		KMBombModule bombModule = BombModule;
		bombModule.OnActivate = (KMModuleActivateEvent)Delegate.Combine((Delegate)(object)bombModule.OnActivate, (Delegate)new KMModuleActivateEvent(Activate));
		errorText.text = string.Empty;
		fixText.text = string.Empty;
		defaultTextColor = sendButtonText.color;
		SetAllButtonTextColor(Color.black);
		bool flag = BatteryCountEven();
		Log("Battery Count is " + ((!flag) ? "ODD" : "EVEN"));
		bool flag2 = SerialNumberContainsVowel();
		Log("Serial Number " + ((!flag2) ? "DOES NOT" : "DOES") + " contain a vowel.");
		int[] array = new int[4]
		{
			Random.Range(0, 102),
			Random.Range(0, 102),
			Random.Range(0, 102),
			Random.Range(0, 102)
		};
		errorCodeString = ERROR_PREFIX;
		for (int i = 0; i < array.Length; i++)
		{
			errorCodeString = errorCodeString + " " + array[i].ToString("X2");
		}
		Log("Error code string: " + errorCodeString.Substring(ERROR_PREFIX.Length).Trim());
		int num = ((!flag2) ? ((!flag) ? array[3] : array[2]) : ((!flag) ? array[1] : array[0]));
		Log("Active error code (hex): " + num.ToString("X2"));
		Log("Active error code (dec): " + num);
		int num2 = 101 - num;
		Log("Fix code (dec): 101-" + num + "=" + num2);
		if (flag2)
		{
			solution = ((!flag) ? ToOctal(num2) : num2.ToString("D3"));
		}
		else
		{
			solution = ((!flag) ? ToBinary(num2) : num2.ToString("X2"));
		}
		string empty = string.Empty;
		empty = ((!flag2) ? ((!flag) ? "binary" : "hex") : ((!flag) ? "oct" : "dec"));
		Log("Final solution (" + empty + "): " + solution);
	}

	private string ToOctal(int value)
	{
		string text = Convert.ToString(value, 8);
		while (text.Length < 3)
		{
			text = "0" + text;
		}
		return text;
	}

	private string ToBinary(int value)
	{
		string text = Convert.ToString(value, 2);
		while (text.Length < 7)
		{
			text = "0" + text;
		}
		return text;
	}

	private bool BatteryCountEven()
	{
		int num = 0;
		List<string> list = BombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_BATTERIES, (string)null);
		foreach (string item in list)
		{
			Dictionary<string, int> dictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(item);
			num += dictionary["numbatteries"];
		}
		Log("Battery Count is " + num);
		return num % 2 == 0;
	}

	private bool SerialNumberContainsVowel()
	{
		string text = string.Empty;
		List<string> list = BombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_SERIAL_NUMBER, (string)null);
		foreach (string item in list)
		{
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(item);
			text += dictionary["serial"];
		}
		text = text.ToUpper();
		Log("Serial Number is " + text);
		return text.Contains("A") || text.Contains("E") || text.Contains("I") || text.Contains("O") || text.Contains("U");
	}

	private void Awake()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		KMSelectable obj = sendButton;
		obj.OnInteract = (OnInteractHandler)Delegate.Combine((Delegate)(object)obj.OnInteract, (Delegate)(OnInteractHandler)delegate
		{
			HandleSendPress();
			return false;
		});
		for (int i = 0; i < numberButtons.Length; i++)
		{
			int j = i;
			KMSelectable obj2 = numberButtons[i];
			obj2.OnInteract = (OnInteractHandler)Delegate.Combine((Delegate)(object)obj2.OnInteract, (Delegate)(OnInteractHandler)delegate
			{
				HandleNumericPress(j);
				return false;
			});
		}
	}

	private void Activate()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		lightsOn = true;
		SetAllButtonTextColor(defaultTextColor);
		errorText.text = errorCodeString;
		fixText.text = FIX_PREFIX;
	}

	private void HandleNumericPress(int index)
	{
		if (!isSolved && lightsOn && userEnteredFix.Length < 7)
		{
			Audio.PlayGameSoundAtTransform((SoundEffect)0, ((Component)numberButtons[index]).transform);
			numberButtons[index].AddInteractionPunch(1f);
			userEnteredFix += NUMBER_STR[index];
			fixText.text = FIX_PREFIX + userEnteredFix;
		}
	}

	private void HandleSendPress()
	{
		if (!isSolved && lightsOn)
		{
			Log("User entered: " + userEnteredFix);
			if (solution == userEnteredFix)
			{
				Audio.PlayGameSoundAtTransform((SoundEffect)0, ((Component)sendButton).transform);
				sendButton.AddInteractionPunch(1f);
				Log("Correct. Module disarmed.");
				isSolved = true;
				BombModule.HandlePass();
			}
			else
			{
				sendButton.AddInteractionPunch(1f);
				Log("Incorrect. Strike.");
				userEnteredFix = string.Empty;
				fixText.text = FIX_PREFIX;
				BombModule.HandleStrike();
			}
		}
	}

	private void SetAllButtonTextColor(Color color)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		TextMesh[] array = numberButtonText;
		foreach (TextMesh val in array)
		{
			val.color = color;
		}
		sendButtonText.color = color;
	}

	private void Log(string msg)
	{
		Debug.LogFormat("[Error Codes #{0}] " + msg, new object[1] { moduleId });
	}
}
