using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Score List", menuName = "ScoreScriptables/Score List", order = 1)]
public class ScoreListScriptable : ScriptableObject
{
    public List<int> scoreList;
}
