

// conditionalButtons, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// conditionalButtons
using System;
using System.Collections.Generic;
using System.Linq;
using KModkit;
using UnityEngine;

public class conditionalButtons : MonoBehaviour
{
	private enum COLORS_T
	{
		BLACK,
		BLUE,
		DARK_GREEN,
		LIGHT_GREEN,
		ORANGE,
		PINK,
		PURPLE,
		RED,
		WHITE,
		YELLOW
	}

	public KMBombInfo BombInfo;

	public KMAudio Audio;

	public List<KMSelectable> Buttons;

	public List<Material> ALL_COLORS;

	public Material DefaultMaterial;

	private static int moduleIdCounter = 1;

	private int moduleId;

	private bool isSolved;

	private bool firstClick = true;

	private List<KMSelectable> ButtonsToPress = new List<KMSelectable>();

	private readonly List<List<COLORS_T>> BUTTON_ORDERS = new List<List<COLORS_T>>
	{
		new List<COLORS_T>
		{
			COLORS_T.BLACK,
			COLORS_T.BLUE,
			COLORS_T.DARK_GREEN,
			COLORS_T.LIGHT_GREEN,
			COLORS_T.ORANGE,
			COLORS_T.PINK,
			COLORS_T.PURPLE,
			COLORS_T.RED,
			COLORS_T.WHITE,
			COLORS_T.YELLOW
		},
		new List<COLORS_T>
		{
			COLORS_T.WHITE,
			COLORS_T.RED,
			COLORS_T.LIGHT_GREEN,
			COLORS_T.PURPLE,
			COLORS_T.BLACK,
			COLORS_T.DARK_GREEN,
			COLORS_T.ORANGE,
			COLORS_T.BLUE,
			COLORS_T.PINK,
			COLORS_T.YELLOW
		},
		new List<COLORS_T>
		{
			COLORS_T.ORANGE,
			COLORS_T.YELLOW,
			COLORS_T.RED,
			COLORS_T.BLUE,
			COLORS_T.PURPLE,
			COLORS_T.LIGHT_GREEN,
			COLORS_T.DARK_GREEN,
			COLORS_T.WHITE,
			COLORS_T.BLACK,
			COLORS_T.PINK
		},
		new List<COLORS_T>
		{
			COLORS_T.ORANGE,
			COLORS_T.PURPLE,
			COLORS_T.LIGHT_GREEN,
			COLORS_T.DARK_GREEN,
			COLORS_T.WHITE,
			COLORS_T.BLUE,
			COLORS_T.RED,
			COLORS_T.PINK,
			COLORS_T.YELLOW,
			COLORS_T.BLACK
		},
		new List<COLORS_T>
		{
			COLORS_T.BLUE,
			COLORS_T.LIGHT_GREEN,
			COLORS_T.PURPLE,
			COLORS_T.ORANGE,
			COLORS_T.RED,
			COLORS_T.WHITE,
			COLORS_T.YELLOW,
			COLORS_T.PINK,
			COLORS_T.BLACK,
			COLORS_T.DARK_GREEN
		},
		new List<COLORS_T>
		{
			COLORS_T.PINK,
			COLORS_T.YELLOW,
			COLORS_T.DARK_GREEN,
			COLORS_T.BLUE,
			COLORS_T.LIGHT_GREEN,
			COLORS_T.BLACK,
			COLORS_T.ORANGE,
			COLORS_T.PURPLE,
			COLORS_T.WHITE,
			COLORS_T.RED
		},
		new List<COLORS_T>
		{
			COLORS_T.RED,
			COLORS_T.BLUE,
			COLORS_T.YELLOW,
			COLORS_T.DARK_GREEN,
			COLORS_T.PINK,
			COLORS_T.BLACK,
			COLORS_T.PURPLE,
			COLORS_T.ORANGE,
			COLORS_T.LIGHT_GREEN,
			COLORS_T.WHITE
		},
		new List<COLORS_T>
		{
			COLORS_T.WHITE,
			COLORS_T.DARK_GREEN,
			COLORS_T.PURPLE,
			COLORS_T.RED,
			COLORS_T.PINK,
			COLORS_T.LIGHT_GREEN,
			COLORS_T.BLACK,
			COLORS_T.ORANGE,
			COLORS_T.BLUE,
			COLORS_T.YELLOW
		},
		new List<COLORS_T>
		{
			COLORS_T.WHITE,
			COLORS_T.BLUE,
			COLORS_T.DARK_GREEN,
			COLORS_T.BLACK,
			COLORS_T.YELLOW,
			COLORS_T.PINK,
			COLORS_T.RED,
			COLORS_T.LIGHT_GREEN,
			COLORS_T.PURPLE,
			COLORS_T.ORANGE
		},
		new List<COLORS_T>
		{
			COLORS_T.PINK,
			COLORS_T.BLACK,
			COLORS_T.LIGHT_GREEN,
			COLORS_T.BLUE,
			COLORS_T.PURPLE,
			COLORS_T.DARK_GREEN,
			COLORS_T.WHITE,
			COLORS_T.YELLOW,
			COLORS_T.RED,
			COLORS_T.ORANGE
		}
	};

	private void Awake()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		moduleId = moduleIdCounter++;
		foreach (KMSelectable button in Buttons)
		{
			KMSelectable obj = button;
			obj.OnInteract = (OnInteractHandler)Delegate.Combine((Delegate)(object)obj.OnInteract, (Delegate)(OnInteractHandler)delegate
			{
				PressButton(button);
				return false;
			});
		}
	}

	private void PressButton(KMSelectable currButton)
	{
		if (isSolved)
		{
			return;
		}
		if (firstClick)
		{
			firstClick = false;
			foreach (KMSelectable button in Buttons)
			{
				((Renderer)((Component)button).GetComponent<MeshRenderer>()).material = DefaultMaterial;
			}
		}
		currButton.AddInteractionPunch(0.4f);
		Audio.PlayGameSoundAtTransform((SoundEffect)0, ((Component)this).transform);
		if ((Object)(object)ButtonsToPress.First() == (Object)(object)currButton)
		{
			ButtonsToPress.Remove(currButton);
			if (ButtonsToPress.Count() == 0)
			{
				isSolved = true;
				((Component)this).GetComponent<KMBombModule>().HandlePass();
			}
		}
		else
		{
			((Component)this).GetComponent<KMBombModule>().HandleStrike();
		}
	}

	private bool CheckBlack()
	{
		return BombInfo.GetBatteryCount() > BombInfo.GetBatteryHolderCount();
	}

	private bool CheckBlue()
	{
		return BombInfo.GetBatteryCount() >= 2;
	}

	private bool CheckDarkGreen()
	{
		int count = BombInfo.GetModuleNames().Count;
		int count2 = BombInfo.GetSolvableModuleIDs().Count;
		return count % 2 == 1 || count - count2 > 0;
	}

	private bool CheckLightGreen()
	{
		return BombInfo.GetPortCount(Port.DVI) > 0 && BombInfo.GetSerialNumberNumbers().Any((int n) => n >= 3 && n <= 8);
	}

	private bool CheckOrange()
	{
		return BombInfo.GetPortCount(Port.PS2) > 0;
	}

	private bool CheckPink()
	{
		int batteryCount = BombInfo.GetBatteryCount();
		return BombInfo.IsIndicatorPresent(Indicator.BOB) && batteryCount % 2 == 1;
	}

	private bool CheckPurple()
	{
		List<char> possibleChars = new List<char> { 'X', 'Y', 'Z' };
		return BombInfo.GetModuleNames().Contains("The Button") || BombInfo.GetSerialNumberLetters().Any((char c) => possibleChars.Contains(c));
	}

	private bool CheckRed()
	{
		return BombInfo.IsIndicatorOn(Indicator.NSA);
	}

	private bool CheckWhite()
	{
		return true;
	}

	private bool CheckYellow()
	{
		List<char> possibleChars = new List<char> { 'H', 'K', 'L' };
		return BombInfo.GetSerialNumberLetters().Any((char c) => possibleChars.Contains(c));
	}

	private void DetermineButtons()
	{
		ALL_COLORS.Shuffle();
		List<COLORS_T> list = new List<COLORS_T>();
		Dictionary<COLORS_T, KMSelectable> ColorToButton = new Dictionary<COLORS_T, KMSelectable>();
		for (int i = 0; i < Buttons.Count; i++)
		{
			((Renderer)((Component)Buttons[i]).GetComponent<MeshRenderer>()).material = ALL_COLORS[i];
			COLORS_T cOLORS_T = (COLORS_T)Enum.Parse(typeof(COLORS_T), ((Object)ALL_COLORS[i]).name.ToUpper().Replace(" ", "_"));
			list.Add(cOLORS_T);
			ColorToButton.Add(cOLORS_T, Buttons[i]);
		}
		Debug.LogFormat("[Conditional Buttons #{0}] All Colors {1}", new object[2]
		{
			moduleId,
			string.Join(",", ALL_COLORS.Select((Material c) => ((Object)c).name).ToArray())
		});
		Debug.LogFormat("[Conditional Buttons #{0}] before sorting {1}", new object[2]
		{
			moduleId,
			string.Join(",", list.Select((COLORS_T c) => c.ToString()).ToArray())
		});
		Debug.LogFormat("[Conditional Buttons #{0}] order {1}", new object[2]
		{
			moduleId,
			string.Join(",", BUTTON_ORDERS[BombInfo.GetSerialNumberNumbers().Last()].Select((COLORS_T c) => c.ToString()).ToArray())
		});
		List<COLORS_T> list2 = BUTTON_ORDERS[BombInfo.GetSerialNumberNumbers().Last()].Intersect(list).ToList();
		Debug.LogFormat("[Conditional Buttons #{0}] after sorting {1}", new object[2]
		{
			moduleId,
			string.Join(",", list2.Select((COLORS_T c) => c.ToString()).ToArray())
		});
		string text = string.Empty;
		foreach (COLORS_T item in list2)
		{
			bool flag = false;
			if (item switch
			{
				COLORS_T.BLACK => CheckBlack(), 
				COLORS_T.BLUE => CheckBlue(), 
				COLORS_T.DARK_GREEN => CheckDarkGreen(), 
				COLORS_T.LIGHT_GREEN => CheckLightGreen(), 
				COLORS_T.ORANGE => CheckOrange(), 
				COLORS_T.PINK => CheckPink(), 
				COLORS_T.PURPLE => CheckPurple(), 
				COLORS_T.RED => CheckRed(), 
				COLORS_T.WHITE => CheckWhite(), 
				COLORS_T.YELLOW => CheckYellow(), 
				_ => throw new IndexOutOfRangeException("Invalid color"), 
			})
			{
				text = text + item.ToString() + " ";
				ButtonsToPress.Add(ColorToButton[item]);
			}
		}
		if (ButtonsToPress.Count == 0)
		{
			ButtonsToPress.AddRange(from color in list2.GetRange(0, 3)
				select ColorToButton[color]);
			text = string.Concat((from c in list2.GetRange(0, 3)
				select c.ToString()).ToArray());
		}
		Debug.LogFormat("[Conditional Buttons #{0}] order is {1}", new object[2] { moduleId, text });
	}

	private void Start()
	{
		if (ButtonsToPress.Count() == 0 && !isSolved)
		{
			Debug.LogFormat("[Conditional Buttons #{0}] modules are {1}", new object[2]
			{
				moduleId,
				string.Join(" ", BombInfo.GetModuleIDs().ToArray())
			});
			DetermineButtons();
		}
	}

	private void Update()
	{
	}
}
