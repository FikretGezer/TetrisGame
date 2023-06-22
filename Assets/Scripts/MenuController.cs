using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    public ScoreListScriptable _scoreListScriptable;
    public GameObject inGameMenu, mainMenu, MovementController, EndingMenu;
    public GameObject inGameButtons, pausedPanel;
    public List<GameObject> menuListObjects;
    public TMP_Text _yourScoreText;

    public static Action<int> OnGameEnded = delegate{};
    private void Awake()
    {
        List<int> menuScoreList = _scoreListScriptable.scoreList;
        if(menuScoreList[0] < menuScoreList[menuScoreList.Count-1])
        {
            menuScoreList.Reverse();            
        }
        for (int i = 0; i < menuListObjects.Count; i++)
        {
            if(menuScoreList[i] > 0)
                menuListObjects[i].transform.GetChild(0).GetComponent<TMP_Text>().text = menuScoreList[i].ToString();
            else
                break;
        }
    }
    private void OnEnable()
    {
        OnGameEnded += OnEnding;
    }
    private void OnDisable()
    {
        OnGameEnded -= OnEnding;
    }
    public void Play()
    {
        mainMenu.SetActive(false);
        inGameMenu.SetActive(true);
    }
    public void Quit()
    {        
        Application.Quit();
    }
    public void Pause()
    {
        MovementController.SetActive(false);
        inGameButtons.SetActive(false);
        pausedPanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void Resume()
    {
        pausedPanel.SetActive(false);
        inGameButtons.SetActive(true);
        Time.timeScale = 1;
        MovementController.SetActive(true);
    }
    public void Return2Menu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        mainMenu.SetActive(false);
        inGameMenu.SetActive(true);
    }
    private void OnEnding(int _finalScore)
    {
        MovementController.SetActive(false);
        inGameButtons.SetActive(false);
        EndingMenu.SetActive(true);
        _yourScoreText.text = _finalScore.ToString();
    }
}
