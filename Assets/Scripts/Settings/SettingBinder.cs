using UnityEngine;

public class SettingBinder : MonoBehaviour
{
    [SerializeField] private string _settingName = string.Empty;

    public void SetFloat(float value) => GameSettings.SetSetting(_settingName, value);
    public void SetInt(int value) => GameSettings.SetSetting(_settingName, value);
    public void SetBool(bool value) => GameSettings.SetSetting(_settingName, value);
    public void SetString(string value) => GameSettings.SetSetting(_settingName, value);
}
