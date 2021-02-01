using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MenuBehaviour : MonoBehaviour
{
    public InputField leftInput, rightInput, botNick, goals;

    public Slider difficult; //must be 0 - hard, 1 - normal, 2 - easy and etc
    public Text sliderValueText;

    public GameObject LocalMenu, PvEMenu;

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void PlayTypingSound()
    {
        if (!Input.GetKey(KeyCode.Backspace))
        {
            AudioManager.instance.PlaySound(AudioManager.Sounds.Typing, 0, UnityEngine.Random.Range(0.7f, 0.9f));
        }
        else
        {
            AudioManager.instance.PlaySound(AudioManager.Sounds.TypingDelete, 0, UnityEngine.Random.Range(0.9f, 1.05f));
        }
    }

    public void Play()
    {
        GameData.instance.leftPlayerName = "LEFT PLAYER";
        GameData.instance.rightPlayerName = "RIGHT PLAYER";
        GameData.instance.difficulty = -1;

        if (leftInput.text != "")
        {
            GameData.instance.leftPlayerName = leftInput.text.ToUpper();
        }

        if (rightInput.text != "")
        {
            GameData.instance.rightPlayerName = rightInput.text.ToUpper();
        }

        if (goals.text != "")
        {
            GameData.instance.maxGoals = Convert.ToInt32(goals.text);
        }

        if (PvEMenu.activeSelf)
        {
            GameData.instance.rightPlayerName = "BOT";
            GameData.instance.difficulty = (int)difficult.value;

            if (botNick.text != "")
            {
                GameData.instance.rightPlayerName = botNick.text.ToUpper();
            }
        }

        SceneManager.LoadScene("Main");
    }

    public void CheckDigitInput()
    {
        if (goals.text.Length > 0 && goals.text[0] == '-')
        {
            goals.text = goals.text.Trim('-');
        }

        if (goals.text.Length == 1 && goals.text[0] == '0')
        {
            goals.text = "1";
        }
    }

    public void OnDifficultChanged()
    {
        sliderValueText.text = difficult.value.ToString();
    }

    public void ChangeGameMode()
    {
        if (LocalMenu.activeSelf)
        {
            LocalMenu.SetActive(false);
            PvEMenu.SetActive(true);
        }
        else
        {
            LocalMenu.SetActive(true);
            PvEMenu.SetActive(false);
        }
    }
}
