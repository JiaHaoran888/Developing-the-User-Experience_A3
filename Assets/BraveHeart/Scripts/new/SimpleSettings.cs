using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class SimpleSettings : MonoBehaviour
{
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Button mainMenu;
    public Button back;
    public CanvasGroup canvasGroup;
    private void Start()
    {
        musicVolumeSlider.value = GameMgr.instance.musicSource.volume;
        sfxVolumeSlider.value = GameMgr.instance.sfxSource.volume;

        mainMenu.onClick.AddListener(() =>
        {
           
            canvasGroup.DOFade(0, 1);

           
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene currentScene = SceneManager.GetSceneAt(i);
               
                if (currentScene.buildIndex == 1)
                {
                    GameMgr.instance.SetGloabMusic(GameMgr.instance.normal);
                    SceneManager.UnloadSceneAsync(currentScene);
                }
                else if (currentScene.buildIndex == 2)
                {
                    GameMgr.instance.SetGloabMusic(GameMgr.instance.normal);
                    SceneManager.UnloadSceneAsync(currentScene);
                }
                else if (currentScene.buildIndex == 3)
                {
                    GameMgr.instance.SetGloabMusic(GameMgr.instance.normal);
                    SceneManager.UnloadSceneAsync(currentScene);
                }
            }
        });

        back.onClick.AddListener(() =>
        {
            Cursor.lockState = CursorLockMode.Locked;
            this.canvasGroup.DOFade(0,1);
        });
    }

    public void AdjustMusicVolume()
    {
        GameMgr.instance.musicSource.volume = musicVolumeSlider.value;
    }

    public void AdjustSFXVolume()
    {
        GameMgr.instance.sfxSource.volume = sfxVolumeSlider.value;
    }
}