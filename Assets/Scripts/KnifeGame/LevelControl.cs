using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KnifeGame
{
    public class LevelControl : MonoBehaviour
    {
        [HideInInspector] public Image Main;
        [HideInInspector] public TextMeshProUGUI Label;
        [HideInInspector] public Image Star;

        private void Awake()
        {
            Main = gameObject.GetComponent<Image>();
            Label = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Star = gameObject.transform.GetChild(1).GetComponent<Image>();
        }
    }
}