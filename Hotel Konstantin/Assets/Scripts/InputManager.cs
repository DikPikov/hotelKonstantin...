﻿using UnityEngine;
using System.IO;
using System;

public class InputManager : MonoBehaviour
{
    private static InputManager Instance = null;
    public static InputManager _Instance
    {
        get
        {
            if(Instance == null)
            {
                Instance = new GameObject("InputManager").AddComponent<InputManager>();
            }

            return Instance;
        }
    }

    public enum ButtonState { Down, Hold, Up }
    public enum AxisEnum {Horizontal, Vertical}
    public enum ButtonEnum {Menu, Run, Interact, OpenExtraInfo, Item1, Item2, Item3, DropItem, Reload }

    [SerializeField] private KeyMapData KeyMap;
#if UNITY_EDITOR
    [SerializeField] private bool UpdateKeyMap;
#endif

    public KeyMapData _KeyMap
    {
        get { return KeyMap; }
        set
        {
            KeyMap = value;
            UpdateKeys();
            SaveKeyMap();
        }
    }

    private Axis Horizontal = null;
    private Axis Vertical = null;

    private Button Menu = null;
    private Button Run = null;
    private Button Interact = null;
    private Button OpenExtraInfo = null;
    private Button Item1 = null;
    private Button Item2 = null;
    private Button Item3 = null;
    private Button DropItem = null;
    private Button Reload = null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        string path = Path.Combine(Application.dataPath, "keyConfig.txt");
        if (File.Exists(path))
        {
            KeyMap = JsonUtility.FromJson<KeyMapData>(File.ReadAllText(path).Replace("\n", ""));
        }
        else
        {
            KeyMap = new KeyMapData();
        }

        UpdateKeys();
    }

    private void OnApplicationQuit()
    {
        SaveKeyMap();
        Instance = null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (UpdateKeyMap)
        {
            UpdateKeys();
            SaveKeyMap();

            UpdateKeyMap = false;
        }
    }
#endif

    public void SaveKeyMap() => File.WriteAllText(Path.Combine(Application.dataPath, "keyConfig.txt"), JsonUtility.ToJson(KeyMap).Replace(",",",\n"));

    private void UpdateKeys()
    {
        Horizontal = new Axis(KeyMap.Horizontal);
        Vertical = new Axis(KeyMap.Vertical);

        Menu = new Button(KeyMap.Menu);
        Run = new Button(KeyMap.Run);
        Interact = new Button(KeyMap.Interact);
        OpenExtraInfo = new Button(KeyMap.OpenExtraInfo);

        Item1 = new Button(KeyMap.Item1);
        Item2 = new Button(KeyMap.Item2);
        Item3 = new Button(KeyMap.Item3);

        DropItem = new Button(KeyMap.DropItem);

        Reload = new Button(KeyMap.Reload);
    }

    public static float GetAxis(AxisEnum axis)
    {
        switch (axis)
        {
            case  AxisEnum.Horizontal:
                return _Instance.Horizontal.GetValue();
            case AxisEnum.Vertical:
                return _Instance.Vertical.GetValue();
        }

        Debug.LogError($"аксиса <color=white>{axis}</color> не существует");

        return 0;
    }

    private bool GetButtonState(ButtonEnum button, ButtonState state)
    {
        switch (button)
        {
            case ButtonEnum.Menu:
                return Menu.CheckState(state);
            case ButtonEnum.Run:
                return Run.CheckState(state);
            case ButtonEnum.Interact:
                return Interact.CheckState(state);
            case ButtonEnum.OpenExtraInfo:
                return OpenExtraInfo.CheckState(state);
            case ButtonEnum.Item1:
                return Item1.CheckState(state);
            case ButtonEnum.Item2:
                return Item2.CheckState(state);
            case ButtonEnum.Item3:
                return Item3.CheckState(state);
            case ButtonEnum.DropItem:
                return DropItem.CheckState(state);
            case ButtonEnum.Reload:
                return Reload.CheckState(state);
        }

        Debug.LogError($"кнопка <color=white>{button}</color> не найдена");

        return false;
    }

    public static bool GetButtonDown(ButtonEnum button) => _Instance.GetButtonState(button, ButtonState.Down);

    public static bool GetButton(ButtonEnum button) => _Instance.GetButtonState(button, ButtonState.Hold);

    public static bool GetButtonUp(ButtonEnum button) => _Instance.GetButtonState(button, ButtonState.Up);

    public class Axis
    {
        private KeyCode[] PositiveKeys = new KeyCode[0];
        private KeyCode[] NegativeKeys = new KeyCode[0];

        public Axis(string keys)
        {
            KeyCode[][] parsed = ParseKeys(keys);
            PositiveKeys = parsed[0];
            NegativeKeys = parsed[1];
        }

        public float GetValue()
        {
            float value = 0;

            foreach(KeyCode key in PositiveKeys)
            {
                if (Input.GetKey(key))
                {
                    value += 1;
                    break;
                }
            }

            foreach (KeyCode key in NegativeKeys)
            {
                if (Input.GetKey(key))
                {
                    value -= 1;
                    break;
                }
            }

            return value;
        }

        public static KeyCode[][] ParseKeys(string keys)
        {
            KeyCode[] positiveKeys = new KeyCode[0];
            KeyCode[] negativeKeys = new KeyCode[0];

            bool positive = true;
            string key = "";

            foreach (char letter in keys)
            {
                switch (letter)
                {
                    case '+':
                        positive = true;
                        break;
                    case '-':
                        positive = false;
                        break;
                    case ' ':
                        if (positive)
                        {
                            positiveKeys = StaticTools.ExpandMassive(positiveKeys, GetKeyCode(key));

                            key = "";
                        }
                        else
                        {
                            negativeKeys = StaticTools.ExpandMassive(negativeKeys, GetKeyCode(key));

                            key = "";
                            positive = false;
                        }
                        break;
                    default:
                        key += letter;
                        break;
                }
            }

            if (key.Length > 0)
            {
                if (positive)
                {
                    positiveKeys = StaticTools.ExpandMassive(positiveKeys, GetKeyCode(key));
                }
                else
                {
                    negativeKeys = StaticTools.ExpandMassive(negativeKeys, GetKeyCode(key));
                }
            }

            return new KeyCode[][] { positiveKeys, negativeKeys };
        }
    }

    public class Button
    {
        private KeyCode[] Keys = new KeyCode[0];

        public Button(string keys) => Keys = ParseKeys(keys);

        public bool CheckState(ButtonState state)
        {
            switch (state)
            {
                case ButtonState.Down:
                    foreach (KeyCode key in Keys)
                    {
                        if (Input.GetKeyDown(key))
                        {
                            return true;
                        }
                    }
                    break;
                case ButtonState.Hold:
                    foreach (KeyCode key in Keys)
                    {
                        if (Input.GetKey(key))
                        {
                            return true;
                        }
                    }
                    break;
                case ButtonState.Up:
                    foreach (KeyCode key in Keys)
                    {
                        if (Input.GetKeyUp(key))
                        {
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        public static KeyCode[] ParseKeys(string keysInfo)
        {
            KeyCode[] keys = new KeyCode[0];
            string key = "";

            foreach (char letter in keysInfo)
            {
                if (letter == ' ')
                {
                    keys = StaticTools.ExpandMassive(keys, GetKeyCode(key));
                    key = "";
                }
                else
                {
                    key += letter;
                }
            }

            if (key.Length > 0)
            {
                keys = StaticTools.ExpandMassive(keys, GetKeyCode(key));
            }

            return keys;
        }
    }

    private static KeyCode GetKeyCode(string key)
    {
        foreach(KeyCode code in Enum.GetValues(typeof(KeyCode)))
        {
            if(code.ToString().ToLower() == key.ToLower())
            {
                return code;
            }
        }

        Debug.LogError($"клавиша <color=white>{key}</color> недействительна");

        return KeyCode.None;
    }
}

[Serializable]
public class KeyMapData
{
    public string Horizontal = "+D +RightArrow -A -LeftArrow";
    public string Vertical = "+W +UpArrow -S -DownArrow";

    public string Menu = "Escape F1";
    public string OpenExtraInfo = "Tab Space";
    public string Item1 = KeyCode.Alpha1.ToString();
    public string Item2 = KeyCode.Alpha2.ToString();
    public string Item3 = KeyCode.Alpha3.ToString();

    public string DropItem = "Q G";

    public string Run = "LeftShift RightShift";

#if UNITY_ANDROID
    public string Interact = "E F";
#else
    public string Interact = "E F Mouse0";
#endif

    public string Reload = "R";
}