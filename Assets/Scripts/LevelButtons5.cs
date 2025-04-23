using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButtons5 : MonoBehaviour
{
    [SerializeField] private Sprite lockedSprite, unlockedSprite;
    private Button[] levelButtons;

    void Start()
    {
        levelButtons = GetComponentsInChildren<Button>(true);
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            Button button = levelButtons[i];
            Image buttonImage = button.GetComponent<Image>();
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>(true);

            if (levelIndex <= unlockedLevel)
            {
                // Level da mo khoa
                buttonText.gameObject.SetActive(true);
                buttonImage.sprite = unlockedSprite;

                button.onClick.AddListener(() => LoadLevel(levelIndex));
            }
            else
            {
                // Level chua mo khoa
                buttonText.gameObject.SetActive(false);
                buttonImage.sprite = lockedSprite;
            }

            if (buttonText) buttonText.text = levelIndex.ToString("00");
        }
    }

    private void LoadLevel(int levelIndex) => GameManager5.Instance.SetCurrentLV(levelIndex);
}
