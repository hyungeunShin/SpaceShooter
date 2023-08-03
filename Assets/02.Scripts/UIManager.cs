using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Button startButton;
    public Button optionButton;
    public Button shopButton;

    private UnityAction action;

    void Start()
    {
        action = () => OnStartClick();
        startButton.onClick.AddListener(action);

        optionButton.onClick.AddListener(delegate {OnButtonClick();});

        shopButton.onClick.AddListener(() => OnButtonClick());
    }

    public void OnButtonClick()
    {
        Debug.Log("button click");
    }

    public void OnStartClick()
    {
        SceneManager.LoadScene("Level_01");
        SceneManager.LoadScene("Play", LoadSceneMode.Additive);
    }
}
