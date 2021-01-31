using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject settingsGO;
    [SerializeField]
    private Toggle muteToggle;
    [SerializeField]
    private Slider volumeSlider;
    [SerializeField]
    private TextMeshProUGUI volumeText;
    public static GameManager instance;
    private int CurrentSceneIndex = 1;
    private void Awake()
    {
        instance = this;
#if !UNITY_EDITOR
        LoadSceneSync(1);
#else
        SetPlayButton();
#endif
    }
    private void SetPlayButton()
    {
        var buttonList = GetAllObjectsOnlyInSceneWithTag<Button>("LoadGameButton");
        if (buttonList.Count > 0)
        {
            Button loadGameButton = buttonList[0];
            loadGameButton.onClick.AddListener(() => LoadScene(2));
        }
        else
        {
            Debug.LogWarning("Could not finds settings button in loaded scenes");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        FindAndUpdateSettingsButton();
#else
        updateSettings();
#endif
    }
    public void updateSettings()
    {
        AudioListener.volume = muteToggle.isOn ? 0 : volumeSlider.value;
        volumeText.text = $"Volume: {Mathf.RoundToInt(volumeSlider.value * 100)}/100";
    }
    public void LoadScene(int index)
    {
        SceneManager.UnloadSceneAsync(CurrentSceneIndex);
        var loadSceneOperation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        loadSceneOperation.completed += (AsyncOperation obj) => GameManager_completed(obj);
        CurrentSceneIndex = index;
    }
    public void LoadSceneSync(int index)
    {
        SceneManager.LoadScene(index, LoadSceneMode.Additive);
        SetPlayButton();
    }

    private void GameManager_completed(AsyncOperation obj)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(CurrentSceneIndex));
        ComicBookManager.instance.PlayComic(ComicType.Scene, CurrentSceneIndex);
        FindAndUpdateSettingsButton();
    }

    private void FindAndUpdateSettingsButton()
    {
        var buttonList = GetAllObjectsOnlyInSceneWithTag<Button>("SettingsButton");
        if (buttonList.Count > 0)
        {
            Button settingsButton = buttonList[0];
            settingsButton.onClick.AddListener(() => settingsGO.SetActive(!settingsGO.activeInHierarchy));
        }
        else
        {
            Debug.LogWarning("Could not finds settings button in loaded scenes");
        }
        updateSettings();
    }

    public List<T> GetAllObjectsOnlyInScene<T>() where T : Object
    {
        List<T> objectsInScene = new List<T>();

        foreach (T obj in Resources.FindObjectsOfTypeAll(typeof(T)) as T[])
        {
            if (
#if !UNITY_WEBGL
                !EditorUtility.IsPersistent(obj) && //If platform is not webgl check if it is not an editor object
#endif
                !(obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave))
            {
                objectsInScene.Add(obj);
            }
        }

        return objectsInScene;
    }
    public List<T> GetAllObjectsOnlyInSceneWithTag<T>(string tag) where T : Component
    {
        List<T> objectsInScene = new List<T>();

        foreach (T obj in Resources.FindObjectsOfTypeAll(typeof(T)) as T[])
        {
            if (
#if !UNITY_WEBGL
                !EditorUtility.IsPersistent(obj) && //If platform is webgl check if it is not an editor object
#endif
                !(obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave) && obj.CompareTag(tag))
            {
                objectsInScene.Add(obj);
            }
        }

        return objectsInScene;
    }
}
