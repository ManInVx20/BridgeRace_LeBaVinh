using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Sprite offSprite;
    [SerializeField]
    private Sprite onSprite;

    private bool toggle;

    private void Start()
    {
        toggle = false;
        image.sprite = offSprite;
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnClickButton);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClickButton);
    }

    private void OnClickButton()
    {
        toggle = !toggle;

        if (toggle)
        {
            image.sprite = onSprite;

            GameManager.Instance.PauseGame();
        }
        else
        {
            image.sprite = offSprite;

            GameManager.Instance.ResumeGame();
        }
    }
}
