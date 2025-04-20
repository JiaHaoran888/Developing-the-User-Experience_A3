using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    public Button StartGame;
    public Button QuitGame;
    public Button showdifficultyPanel;
    public Button showlevelPanel;
    public GameObject StartPanel;

    // levelPanel
    [Header("difficultPanel")]
    public GameObject difficultyPanel;
    public Button diffback;
    public Button difficulty1;
    public Button difficulty2;
    public Button difficulty3;

    [Header("levelPanel")]
    public GameObject levelPanel;
    public Button levelback;
    public Button level1;
    public Button level2;
    public Button level3;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        diffback.onClick.AddListener(() =>
        {
            difficultyPanel.SetActive(false);
            StartPanel.SetActive(true);
        });
        showdifficultyPanel.onClick.AddListener(() =>
        {
            difficultyPanel.SetActive(true);
        });
        difficulty1.onClick.AddListener(() =>
        {
            GameMgr.instance.row = 10;
            GameMgr.instance.col = 10;
            GameMgr.instance.level = 1;
            difficultyPanel.SetActive(false);
            StartPanel.SetActive(true);
        });
        difficulty2.onClick.AddListener(() =>
        {
            GameMgr.instance.row = 15;
            GameMgr.instance.col = 15;
            GameMgr.instance.level = 2;
            difficultyPanel.SetActive(false);
            StartPanel.SetActive(true);
        });
        difficulty3.onClick.AddListener(() =>
        {
            GameMgr.instance.row = 20;
            GameMgr.instance.col = 20;
            GameMgr.instance.level = 3;
            difficultyPanel.SetActive(false);
            StartPanel.SetActive(true);
        });
        ///////levelPanel
        showlevelPanel.onClick.AddListener(() =>
        {
            levelPanel.SetActive(true);
        });
        levelback.onClick.AddListener(() =>
        {
            levelPanel.SetActive(false);
            StartPanel.SetActive(true);
        });
        level1.onClick.AddListener(() =>
        {
            Cursor.lockState = CursorLockMode.Locked;
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            StartPanel.SetActive(true);
        });
        level2.onClick.AddListener(() =>
        {
            Cursor.lockState = CursorLockMode.Locked;
            SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
            StartPanel.SetActive(true);
        });
        level3.onClick.AddListener(() =>
        {
            Cursor.lockState = CursorLockMode.Locked;
            GameMgr.instance.SetGloabMusic(GameMgr.instance.fight);
            SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive);
            StartPanel.SetActive(true);
        });
        //////////////////////////////////////////
        QuitGame.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        StartGame.onClick.AddListener(() =>
        {
            Cursor.lockState = CursorLockMode.Locked;
            Scene currentScene = SceneManager.GetActiveScene();

            SceneManager.LoadSceneAsync(currentScene.buildIndex + 1, LoadSceneMode.Additive);
        });
    }


    private void OnDestroy()
    {
        if (instance != null)
        {
            instance = null;
        }
    }
}