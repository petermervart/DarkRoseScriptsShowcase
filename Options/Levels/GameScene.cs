using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Scene Config", fileName = "GameSceneConfig")]
public class GameScene : ScriptableObject
{
    [Header("Information")]
    public string sceneName;
}