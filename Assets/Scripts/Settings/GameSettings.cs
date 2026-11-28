using CreamyCrescentRolls.RoomGame.JsonSaver;
using CreamyCrescentRolls.RoomGame.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-100)]
public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    public Action SettingsApplied;

    [SerializeField] private List<SettingBinder> _binds = new();
    private List<Setting> _settings = new();
    [SerializeField] private string _saveName;

    [SerializeField] private UnityEvent _onHaveChanges;
    [SerializeField] private UnityEvent _closeSettings;

    private bool _haveChanges = false;
    
    private string _savePath
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, _saveName);
        }
    }

    public void Awake()
    {
        Instance = this;

        Load();

        LoadBinds();
    }

    private void Start()
    {
        ApplyChanges();
    }

    public void ChangeSetting(string name, float value)
    {
        Setting setting = GetSetting(name);
        OnSomethingChanged();
        setting.floatValue = value;
    }

    public void ChangeSetting(string name, int value)
    {
        Setting setting = GetSetting(name);
        OnSomethingChanged();
        setting.intValue = value;
    }

    public void ChangeSetting(string name, string value)
    {
        Setting setting = GetSetting(name);
        OnSomethingChanged();
        setting.stringValue = value;
    }

    public void ChangeSetting(string name, bool value)
    {
        Setting setting = GetSetting(name);
        OnSomethingChanged();
        setting.boolValue = value;
    }

    public Setting GetSetting(string name)
    {
        Setting setting = _settings.Find((x) => x.name == name);

        if (setting == null)
        {
            Debug.Log($"[GameSettings] Setting with name '{name}' arent loaded/exists");

            setting = new();
            setting.name = name;
            setting.isNew = true;

            _settings.Add(setting);
        }
        else
        {
            setting.isNew = false;  
        }
        return setting;
    }

    public void OnSomethingChanged()
    {
        _haveChanges = true;
    }

    public void TryCloseSettings()
    {
        if (_haveChanges)
        {
            _onHaveChanges.Invoke();
            return;
        }

        _closeSettings.Invoke();
    }

    public void ApplyChanges()
    {
        Save();
        SettingsApplied?.Invoke();
        _haveChanges = false;
    }

    public void DeclineChanges()
    {
        Load();
        SettingsApplied?.Invoke();
        _haveChanges = false;
    }

    public void LoadBinds()
    {
        foreach (var bind in _binds)
        {
            bind.Load();
        }

        _haveChanges = false;
    }

    public void Load()
    {
        if (!JsonSaver.Load(_savePath, out _settings))
        {
            _settings = new();
            Debug.LogWarning("[GameSettings] Failed to load settings");
            return;
        }
    }

    public void Save()
    {
        if (!JsonSaver.Save(_settings, _savePath))
        {
            Debug.LogWarning("[GameSettings] Failed to save settings");
            return;
        }
    }

}
