using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSaver : EditorWindow
{
    // Auto Saver :
    //
    // 

    #region Fields / Properties
    // Is the auto save enabled or not
    [SerializeField] private bool isAutoSave = false;

    // The time at which the next auto save will run
    private double nextSaveTime = 0;

    // The interval between auto saves (in seconds)
    [SerializeField] private int saveInterval = 300;

    // The current open scene to save
    [SerializeField] private Scene currentScene;
    #endregion

    #region Methods
    #region Original Methods
    /// <summary>
    /// Initializes the auto saver window instance if none and shows it
    /// </summary>
    [MenuItem("Tools/Auto Save/ Auto Saver")]
    public static void Init()
    {
        // Gets the existing version of this window or creates a new one, then show it
        AutoSaver _instance = (AutoSaver)GetWindow(typeof(AutoSaver));
        _instance.Show();
    }

    /// <summary>
    /// Save the current open edited scene
    /// </summary>
    private void SaveScene()
    {
        // If the application is playing, do not save
        if (EditorApplication.isPlaying) return;

        // Get the active scene, set it as dirty, then save it
        currentScene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(currentScene);
        if (EditorSceneManager.SaveScene(currentScene))
        {
            Debug.Log("Scene successfully saved !");

            nextSaveTime = EditorApplication.timeSinceStartup + saveInterval;
        }
        else
        {
            Debug.LogWarning("Scene not saved correctly ! Another try in 30 seconds");

            nextSaveTime = EditorApplication.timeSinceStartup + 30;
        }
    }
    #endregion

    #region Unity Methods
    // Implement your own editor GUI here
    private void OnGUI()
    {
        // (Des)Activate auto save
        EditorGUI.BeginChangeCheck();

        isAutoSave = EditorGUILayout.Toggle("Auto-Save :", isAutoSave);

        // If auto save was enabled, save the scene
        if (EditorGUI.EndChangeCheck() && isAutoSave == true)
        {
            SaveScene();
        }

        // ---------- Show Time Values ---------- //

        EditorGUILayout.Space();

        // Setting the auto save interval
        EditorGUI.BeginChangeCheck();

        saveInterval = EditorGUILayout.IntField("Save Interval (Secs) :", saveInterval);

        if (EditorGUI.EndChangeCheck() && isAutoSave == true)
        {
            nextSaveTime = EditorApplication.timeSinceStartup + saveInterval;
        }

        // Show when the next save will happen
        if (isAutoSave)
        {
            EditorGUILayout.LabelField("Next save in :", $"{(int)(nextSaveTime - EditorApplication.timeSinceStartup)} secs");
        }

        Repaint();
    }

    // Update is called once per frame
    void Update()
    {
        // Returns if auto save is not enabled or if playing
        if (!isAutoSave || EditorApplication.isPlaying) return;

        // Triggers auto save if the timer is superior to the save interval
        if (EditorApplication.timeSinceStartup >= nextSaveTime)
        {
            SaveScene();
        }
	}
    #endregion
    #endregion
}
