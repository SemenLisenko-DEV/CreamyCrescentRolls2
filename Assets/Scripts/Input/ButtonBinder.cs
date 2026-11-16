using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonBinder : MonoBehaviour
{
    [Serializable]
    private class ButtonBind
    {
        public string buttonName;

        public UnityEvent onPress;
    }

    [SerializeField] private List<ButtonBind> _buttonBinds = new();

    private List<Coroutine> _binds = new();

    private void Awake()
    {
        InitializeBinds();
    }

    private IEnumerator ButtonCoroutine(ButtonBind buttonBind)
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetButtonDown(buttonBind.buttonName));

            buttonBind.onPress.Invoke();

            yield return new WaitUntil(() => Input.GetButtonUp(buttonBind.buttonName));
        }
    }

    private void InitializeBinds()
    {
        if (_binds.Count != 0)
            ClearBinds();

        foreach (var bind in _buttonBinds)
        {
            _binds.Add(StartCoroutine(ButtonCoroutine(bind)));
        }
    }

    private void ClearBinds()
    {
        foreach (var bind in _binds)
        {
            if(bind != null)
                StopCoroutine(bind);
        }

        _binds.Clear();
    }
}
