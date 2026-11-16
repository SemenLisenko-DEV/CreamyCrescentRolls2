using System;
using System.Collections.Generic;
using System.IO;
using CreamyCrescentRolls.RoomGame.Settings;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class GameSettings : MonoBehaviour
{
    [SerializeField] private List<FloatSetting> _floatSettings = new();
    [SerializeField] private List<IntSetting> _intSettings = new();
    [SerializeField] private List<BoolSetting> _boolSettings = new();
    [SerializeField] private List<StringSetting> _stringSettings = new();
    [SerializeField] private string _fileName = "settings.json";
    [SerializeField] private bool _persistBetweenScenes = true;

    public static GameSettings Instance { get; private set; }
    public static event Action SettingsApplied;

    private SettingsState _state;
    private string _savePath;
    private bool _busy;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[GameSettings] A settings instance already exists; duplicate disabled.", this);
            enabled = false;
            return;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(_fileName) || _fileName != Path.GetFileName(_fileName)
                || _fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
                || _fileName == "." || _fileName == "..")
                throw new ArgumentException("Settings file name must be a plain file name.");

            _state = new SettingsState(GetDefinitions());
            _savePath = Path.Combine(Application.persistentDataPath, _fileName);
        }
        catch (Exception exception)
        {
            Debug.LogError($"[GameSettings] Invalid configuration: {exception.Message}", this);
            _state = null;
            enabled = false;
            return;
        }

        if (File.Exists(_savePath))
        {
            try
            {
                _state.Restore(SettingsStorage.Load(_savePath));
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[GameSettings] Could not load settings; defaults retained. {exception.Message}", this);
            }
        }

        Instance = this;
        if (_persistBetweenScenes && transform.parent == null)
            DontDestroyOnLoad(gameObject);
        else if (_persistBetweenScenes)
            Debug.LogWarning("[GameSettings] Place this component on a root object to persist between scenes.", this);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public static T GetSetting<T>(string name)
    {
        if (Instance == null || Instance._state == null)
            throw new InvalidOperationException("GameSettings has not been initialized.");

        return Instance._state.Get<T>(name);
    }

    public static bool TryGetSetting<T>(string name, out T value)
    {
        value = default;
        return Instance != null && Instance._state != null && Instance._state.TryGet(name, out value);
    }

    public static bool TryGetPendingSetting<T>(string name, out T value)
    {
        value = default;
        return Instance != null && Instance._state != null && Instance._state.TryGet(name, out value, true);
    }

    public static bool SetSetting<T>(string name, T value)
    {
        if (Instance == null || !Instance.CanEdit())
            return false;

        try
        {
            Instance._state.SetPending(name, value);
            return true;
        }
        catch (ArgumentException exception)
        {
            Debug.LogWarning($"[GameSettings] {exception.Message}", Instance);
            return false;
        }
    }

    public void ApplySettings() => TryApplySettings();

    public bool TryApplySettings()
    {
        if (!CanEdit())
            return false;

        _busy = true;
        try
        {
            try
            {
                SettingsStorage.Save(_state.CapturePending(), _savePath);
            }
            catch (Exception exception)
            {
                Debug.LogError($"[GameSettings] Save failed; applied values retained. {exception.Message}", this);
                return false;
            }

            if (_state.CommitPending())
                NotifySettingsApplied();

            return true;
        }
        finally
        {
            _busy = false;
        }
    }

    public void CancelChanges()
    {
        if (!CanEdit())
            return;

        _state.CancelChanges();
    }

    public void ResetToDefaults()
    {
        if (!CanEdit())
            return;

        _state.ResetToDefaults();
    }

    private bool CanEdit() => Instance == this && _state != null && !_busy;

    private IEnumerable<SettingDefinition> GetDefinitions()
    {
        foreach (var setting in _floatSettings) yield return setting;
        foreach (var setting in _intSettings) yield return setting;
        foreach (var setting in _boolSettings) yield return setting;
        foreach (var setting in _stringSettings) yield return setting;
    }

    private void NotifySettingsApplied()
    {
        var listeners = SettingsApplied;
        if (listeners == null)
            return;

        foreach (Action listener in listeners.GetInvocationList())
        {
            try { listener(); }
            catch (Exception exception) { Debug.LogException(exception, this); }
        }
    }
}
