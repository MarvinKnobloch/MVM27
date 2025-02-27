using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Dialog", menuName = "ScriptableObjects/DialogMessage")]
public class DialogObj : ScriptableObject
{
    public bool cantSkipDialog;
    public bool disableInputs;
    public bool pauseGame;
    public float autoPlayInterval;
    public Dialog[] dialogs;
}
[Serializable]
public struct Dialog
{
    public Sprite characterSprite;
    public string characterName;
    [TextArea]
    public string dialogText;
    public VoidEventChannel dialogEvent;
}
