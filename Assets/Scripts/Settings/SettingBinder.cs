using CreamyCrescentRolls.RoomGame.Settings;
using System;
using UnityEngine;
using UnityEngine.Events;

public class SettingBinder : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private UnityEvent<float> _loadFloatSettings;
    [SerializeField] private UnityEvent<string> _loadStringSettings;
    [SerializeField] private UnityEvent<int> _loadIntSettings;
    [SerializeField] private UnityEvent<bool> _loadBoolSettings;

    [Serializable]
    private class DefaultValues
    {
        public float floatValue;
        public int intValue;
        public string stringValue;
        public bool boolValue;
    }

    [SerializeField] private DefaultValues _defaultValues;

    private void OnDestroy()
    {
        GameSettings.Instance.SettingsApplied -= Load;
    }

    public void Load()
    {
        GameSettings.Instance.SettingsApplied += Load;
        Setting setting = GameSettings.Instance.GetSetting(_name);

        if(setting.isNew)
        {
            setting.floatValue = _defaultValues.floatValue;
            setting.intValue = _defaultValues.intValue;
            setting.stringValue = _defaultValues.stringValue;
            setting.boolValue = _defaultValues.boolValue;
        }

        _loadFloatSettings.Invoke(setting.floatValue);
        _loadStringSettings.Invoke(setting.stringValue);
        _loadIntSettings.Invoke(setting.intValue);
        _loadBoolSettings.Invoke(setting.boolValue);
    }

    public void ChangeSetting(float value) => GameSettings.Instance.ChangeSetting(_name, value);

    public void ChangeSetting(int value) => GameSettings.Instance.ChangeSetting(_name, value);

    public void ChangeSetting(string value) => GameSettings.Instance.ChangeSetting(_name, value);

    public void ChangeSetting(bool value) => GameSettings.Instance.ChangeSetting(_name, value);
}
