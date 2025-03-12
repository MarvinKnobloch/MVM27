using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private VoidEventChannel enableBlackScreen;
    [SerializeField] private VoidEventChannel playerSleep;
    [SerializeField] private VoidEventChannel disableBlackScreen;
    [SerializeField] private VoidEventChannel standUp;
    [SerializeField] private VoidEventChannel endTutorial;
    [SerializeField] private VoidEventChannel bossCameraAndGates;

    [Space]
    [SerializeField] private Transform bossCameraPosition;
    [SerializeField] private MoveOnInteraction[] bossGates;

    private void OnEnable()
    {
        enableBlackScreen.OnEventRaised += ActivateBlackScreen;
        playerSleep.OnEventRaised += PlayerSleep;
        disableBlackScreen.OnEventRaised += DeactivateBlackScreen;
        standUp.OnEventRaised += IntroStandUp;
        endTutorial.OnEventRaised += TutorialDone;
        bossCameraAndGates.OnEventRaised += BossCameraAndGatesAndMusic;
    }
    private void OnDisable()
    {
        enableBlackScreen.OnEventRaised -= ActivateBlackScreen;
        playerSleep.OnEventRaised -= PlayerSleep;
        disableBlackScreen.OnEventRaised -= DeactivateBlackScreen;
        standUp.OnEventRaised -= IntroStandUp;
        endTutorial.OnEventRaised -= TutorialDone;
        bossCameraAndGates.OnEventRaised -= BossCameraAndGatesAndMusic;
    }

    private void ActivateBlackScreen()
    {
        GameManager.Instance.playerUI.ActivateBlackscreen();
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
    public void BossCameraAndGatesAndMusic()
    {
        GameManager.Instance.ChangeCamera(bossCameraPosition);
        AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.Boss);
        foreach (var obj in bossGates)
        {
            obj.Activate();
        }
    }
}
