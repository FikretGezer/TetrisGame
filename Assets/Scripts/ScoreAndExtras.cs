using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreAndExtras : MonoBehaviour
{
    public static Action OnScoreChange = delegate{};
    public static Action OnLineDestroyed = delegate{};
    public static Action OnEnding = delegate{};
    public static Action OnScoreListYazdir = delegate{};
    public static Action<int, Sprite> OnObjectSpawn = delegate{};


    public List<GameObject> _objeImages = new List<GameObject>();
    public List<GameObject> _textObjeleri = new List<GameObject>();
    public ScoreScriptable _currentScore;
    public ScoreListScriptable _scoresList;
    public TMP_Text _scoreTMP;
    public TMP_Text _destroyedLineTMP;
    
    private int _destroyedLineCount;
    private int _score;

    private static ScoreScriptable _endScore;

    private void Awake()
    {
        _endScore = _currentScore;
        _score = 0;
        _destroyedLineCount = 0;
        ScoreYazdir();
    }

    private void OnEnable()
    {        
        OnScoreChange += ChangeScore;
        OnLineDestroyed += DestroyLine;
        OnEnding += EndScoreChanges;
        OnObjectSpawn += NextShow;
        OnScoreListYazdir += ScoreYazdir;
    }
    private void OnDisable()
    {
        OnScoreChange -= ChangeScore;
        OnLineDestroyed -= DestroyLine;
        OnEnding -= EndScoreChanges;
        OnObjectSpawn -= NextShow;
        OnScoreListYazdir -= ScoreYazdir;
    }
    private void ChangeScore()
    {
        int _randomScore = UnityEngine.Random.Range(30, 41);
        _score += _randomScore;
        _scoreTMP.text = _score.ToString();
    }
    private void DestroyLine()
    {
        int _randomScore = UnityEngine.Random.Range(900, 1201);
        _score += _randomScore;
        _scoreTMP.text = _score.ToString();

        _destroyedLineCount++;
        _destroyedLineTMP.text = _destroyedLineCount.ToString();
    }
    private void EndScoreChanges()
    {
        int _finalScore = 0;
        if(_destroyedLineCount > 0)
            _finalScore = _score * _destroyedLineCount;
        else
            _finalScore = _score;
        
       if(_finalScore > _endScore.score)
            _endScore.score = _score;

        ListTheScores(_finalScore);    
        ScoreYazdir();
        MenuController.OnGameEnded(_finalScore);    
    }
    private void ScoreYazdir()
    {
        var list = _scoresList.scoreList;
        if(list[0] < list[list.Count-1])
        {
            list.Reverse();
        }

        for (int i = 0; i < _textObjeleri.Count; i++)
        {
            if(list[i] > 0)
            {
                TMP_Text _t = _textObjeleri[i].transform.GetChild(0).GetComponent<TMP_Text>();
                _t.text = list[i].ToString();
            }
            else
                break;
        }
    }
    private void ListTheScores(int _finalScore)
    {
        List<int> listOfScores = _scoresList.scoreList;
        if(listOfScores[0] > listOfScores[listOfScores.Count-1])
            listOfScores.Sort();

        bool isChanged = false;

        for (int i = 0; i < listOfScores.Count; i++)
        {
            if(_finalScore > listOfScores[i])
            {
                isChanged = true;
                listOfScores[i] = _finalScore;
                break;
            }
        }
         if(isChanged)
        {
            listOfScores.Sort();   
            isChanged = false;
        }
    }
    private void NextShow(int index, Sprite _sprite)
    {
        foreach (var item in _objeImages)
        {
            item.SetActive(false);
        }

        GameObject obj = _objeImages[index];

        foreach (Transform item in obj.transform)
        {
            item.GetComponent<Image>().sprite = _sprite;
        }

        obj.SetActive(true);
    }
}
