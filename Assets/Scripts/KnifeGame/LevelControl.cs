using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KnifeGame
{
    public class LevelControl : MonoBehaviourHelper
    {
        [HideInInspector] public Image Main;
        [HideInInspector] public TextMeshProUGUI Label;
        [HideInInspector] public Image Star;
        [HideInInspector] public Button LButton;
        [HideInInspector] public int LevelByUser;

        private void Awake()
        {
            Main = gameObject.GetComponent<Image>();
            Label = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Star = gameObject.transform.GetChild(1).GetComponent<Image>();
            LButton = gameObject.GetComponent<Button>();

            Lock();
        }

        private void Start()
        {
            LButton.onClick.AddListener(GoToLevel);
        }

        private void Lock()
        {
            Main.sprite = chooseLevelController.Locked;
            Label.gameObject.SetActive(false);
            Star.gameObject.SetActive(false);
            LButton.interactable = false;
        }

        private void GoToLevel()
        {
            audioManager.PlayButtonClick();
            audioManager.BackgroundGamePlay();
            Util.ChooseLevelBool = true;
            Util.SetLevelByChoosing(LevelByUser);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}