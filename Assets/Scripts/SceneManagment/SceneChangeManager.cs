using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public UnityEvent onStartLoad;

    [SerializeField] private float _loadTime;

    public void LoadScene(string sceneName) => StartCoroutine(LoadScene_C(sceneName));

    private IEnumerator LoadScene_C(string sceneName)
    {
        onStartLoad.Invoke();

        yield return new WaitForSecondsRealtime(_loadTime);

        SceneManager.LoadSceneAsync(sceneName);
    }
}
