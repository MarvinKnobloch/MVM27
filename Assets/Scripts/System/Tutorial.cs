using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.U2D.IK;

public class Tutorial : MonoBehaviour
{
    private Controls controls;

    [SerializeField] private VoidEventChannel enableBlackScreen;
    [SerializeField] private VoidEventChannel playerSleep;
    [SerializeField] private VoidEventChannel disableBlackScreen;
    [SerializeField] private VoidEventChannel standUp;
    [SerializeField] private VoidEventChannel endTutorial;
    [SerializeField] private VoidEventChannel bossCameraAndGates;
    [SerializeField] private VoidEventChannel factoryMusicFadeIn;

    [Header("HotkeyTutorial")]
    [SerializeField] private VoidEventChannel moveTutorial;
    [SerializeField] private VoidEventChannel attackHealTutoroal;

    [Space]
    [SerializeField] private Transform bossCameraPosition;
    [SerializeField] private MoveOnInteraction[] bossGates;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void OnEnable()
    {
        enableBlackScreen.OnEventRaised += ActivateBlackScreen;
        playerSleep.OnEventRaised += PlayerSleep;
        disableBlackScreen.OnEventRaised += DeactivateBlackScreen;
        standUp.OnEventRaised += IntroStandUp;
        endTutorial.OnEventRaised += TutorialDone;
        bossCameraAndGates.OnEventRaised += BossCameraAndGatesAndMusic;
        moveTutorial.OnEventRaised += MoveTutorial;
        attackHealTutoroal.OnEventRaised += AttackHealTutorial;
        factoryMusicFadeIn.OnEventRaised += FactoryMusicFadeIn;
    }
    private void OnDisable()
    {
        enableBlackScreen.OnEventRaised -= ActivateBlackScreen;
        playerSleep.OnEventRaised -= PlayerSleep;
        disableBlackScreen.OnEventRaised -= DeactivateBlackScreen;
        standUp.OnEventRaised -= IntroStandUp;
        endTutorial.OnEventRaised -= TutorialDone;
        bossCameraAndGates.OnEventRaised -= BossCameraAndGatesAndMusic;
        moveTutorial.OnEventRaised -= MoveTutorial;
        attackHealTutoroal.OnEventRaised -= AttackHealTutorial;
        factoryMusicFadeIn.OnEventRaised -= FactoryMusicFadeIn;
    }

    private void ActivateBlackScreen()
    {
        GameManager.Instance.playerUI.ActivateBlackscreen();
        AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.Empty, true, 0.1f, 4);
    }
    private void PlayerSleep()
    {
        Player.Instance.ChangeAnimationState("Sleep");
    }
    private void DeactivateBlackScreen()
    {
        GameManager.Instance.playerUI.DeactivateBlackScreen();
    }
    public void IntroStandUp()
    {
        Player.Instance.ChangeAnimationState("StandUp");
    }
    public void TutorialDone()
    {

        PlayerPrefs.SetInt(GameManager.OverworldSaveNames.TutorialProgress.ToString(), PlayerPrefs.GetInt(GameManager.OverworldSaveNames.TutorialProgress.ToString()) + 1);
        PlayerPrefs.SetInt("NewGame", 1);
    }
    public void FactoryMusicFadeIn()
    {
        AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.Tutorial, true, 0.1f, 1);
    }

    public void BossCameraAndGatesAndMusic()
    {
        GameManager.Instance.ChangeCamera(bossCameraPosition);
        AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.Empty, true, 3, 3);
        foreach (var obj in bossGates)
        {
            obj.Activate();
        }
    }
    public void MoveTutorial()
    {
        TextMeshProUGUI dialogText = GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().boxText;
        dialogText.text = "Well, that doesn’t matter!Get yourself ready and follow me!\n(Press <color=green>" +
            controls.Player.Move.GetBindingDisplayString(3) + "</color>/<color=green>" + controls.Player.Move.GetBindingDisplayString(4) + "</color> to move)\n(Press <color=green>" +
            controls.Player.Jump.GetBindingDisplayString() + "</color> to jump)";
    }
    public void AttackHealTutorial()
    {
        TextMeshProUGUI dialogText = GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().boxText;
        dialogText.text = "And just a warning, we’re in a Factory... even though it’s old and overgrown, make sure to protect yourself if you must\n(Press <color=green>" +
            controls.Player.Attack.GetBindingDisplayString() + "</color> multiple times to perform a 3-hit combo attack)\n(Hold <color=green>" +
            controls.Player.ElementAbility1.GetBindingDisplayString() + "</color> to cast a ability which recovers health)";
    }
}
