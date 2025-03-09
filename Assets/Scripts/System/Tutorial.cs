using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private VoidEventChannel enableBlackScreen;
    [SerializeField] private VoidEventChannel playerSleep;
    [SerializeField] private VoidEventChannel disableBlackScreen;
    [SerializeField] private VoidEventChannel standUp;
    [SerializeField] private VoidEventChannel endTutorial;
    [SerializeField] private VoidEventChannel bossCamera;

    [Space]
    [SerializeField] private Transform bossCameraPosition;

    private void OnEnable()
    {
        enableBlackScreen.OnEventRaised += ActivateBlackScreen;
        playerSleep.OnEventRaised += PlayerSleep;
        disableBlackScreen.OnEventRaised += DeactivateBlackScreen;
        standUp.OnEventRaised += IntroStandUp;
        endTutorial.OnEventRaised += TutorialDone;
        bossCamera.OnEventRaised += BossCamera;
    }
    private void OnDisable()
    {
        enableBlackScreen.OnEventRaised -= ActivateBlackScreen;
        playerSleep.OnEventRaised -= PlayerSleep;
        disableBlackScreen.OnEventRaised -= DeactivateBlackScreen;
        standUp.OnEventRaised -= IntroStandUp;
        endTutorial.OnEventRaised -= TutorialDone;
        bossCamera.OnEventRaised -= BossCamera;
    }

    private void ActivateBlackScreen()
    {
        GameManager.Instance.playerUI.ActivateBlackscreen();
    }
    private void PlayerSleep()
    {
        Player.Instance.state = Player.States.Emtpy;
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
    public void BossCamera()
    {
        GameManager.Instance.cinemachineCamera.Target.TrackingTarget = bossCameraPosition;
    }
}
