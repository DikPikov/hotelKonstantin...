using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private Text ResolutionInfo;
    [SerializeField] private Text QualityInfo;
    [SerializeField] private Text SensitivityInfo;
    [SerializeField] private Toggle FullScreen;
    [SerializeField] private Toggle PostProcessing;
    [SerializeField] private Toggle AlwaysShowInterface;
    [SerializeField] private Slider Audio;
    [SerializeField] private Slider Sensitivity;

    [SerializeField] private Settings Settings;

    private Config Config = null;

    private Vector2Int[] Resolutions = new Vector2Int[0];
    private int Resolution = 0;

    private void Start()
    {
        Settings.OnChanges += UpdateInfo;
        UpdateInfo();

        Resolutions = new Vector2Int[0];
        for(int i = 0; i < Screen.resolutions.Length; i++)
        {
            Vector2Int resolution = new Vector2Int(Screen.resolutions[i].width, Screen.resolutions[i].height);

            Resolutions = StaticTools.ExcludingExpandMassive(Resolutions, resolution);
        }
    }

    public void UpdateInfo()
    {
        Config = Settings._Config.Clone();

        ResolutionInfo.text = $"{Config.XResolution}x{Config.YResolution}";
        switch (Config.Quality)
        {
            case 0:
                QualityInfo.text = $"НИЗКОЕ";
                break;
            case 1:
                QualityInfo.text = $"СРЕДНЕЕ";
                break;
            case 2:
                QualityInfo.text = $"ВЫСОКОЕ";
                break;
        }

        SensitivityInfo.text = $"ЧУВСТВИТЕЛЬНОСТЬ: {Config.Sensitivity}";
        FullScreen.isOn = Config.FullScreen;
        PostProcessing.isOn = Config.PostProcessing;
        AlwaysShowInterface.isOn = Config.AlwaysShowInterface;
        Audio.value = Config.Audio;
        Sensitivity.value = Config.Sensitivity / 4f;

        for(int i = 0; i < Resolutions.Length; i++)
        {
            if (Config.XResolution == Resolutions[i].x && Config.YResolution == Resolutions[i].y)
            {
                Resolution = i;
                break;
            }
        }
    }

    public void Apply()
    {
        Settings._Config = Config;
    }

    public void SetResolution()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Resolution = (Resolution + 1) % Resolutions.Length;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Resolution--;
            if(Resolution < 0)
            {
                Resolution = Resolutions.Length - 1;
            }
        }

        Config.XResolution = Resolutions[Resolution].x;
        Config.YResolution = Resolutions[Resolution].y;

        ResolutionInfo.text = $"{Config.XResolution}x{Config.YResolution}";
    }

    public void SetQuality()
    {
        Config.Quality = (Config.Quality + 1) % 3;
        switch (Config.Quality)
        {
            case 0:
                QualityInfo.text = $"НИЗКОЕ";
                break;
            case 1:
                QualityInfo.text = $"СРЕДНЕЕ";
                break;
            case 2:
                QualityInfo.text = $"ВЫСОКОЕ";
                break;
        }
    }

    public void SetSensitivity(float value)
    {
        Config.Sensitivity = value * 4;
        SensitivityInfo.text = $"ЧУВСТВИТЕЛЬНОСТЬ: {Config.Sensitivity}";
    }

    public void SetAudio(float value)
    {
        Config.Audio = value;
    }

    public void SetShowInterface(bool state)
    {
        Config.AlwaysShowInterface = state;
    }

    public void SetFullScreen(bool state)
    {
        Config.FullScreen = state;
    }

    public void SetPostProcess(bool state)
    {
        Config.PostProcessing = state;
    }
}
