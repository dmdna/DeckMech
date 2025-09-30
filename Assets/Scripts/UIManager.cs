using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject splashScreenUI;
    public GameObject armorPhaseUI;
    public GameObject fightPhaseUI;

    void Start()
    {
        ShowSplashScreen();
    }

    public void ShowSplashScreen()
    {
        splashScreenUI.SetActive(true);
        armorPhaseUI.SetActive(false);
        fightPhaseUI.SetActive(false);
    }

    public void ShowArmorPhase()
    {
        splashScreenUI.SetActive(false);
        armorPhaseUI.SetActive(true);
        fightPhaseUI.SetActive(false);
    }

    public void ShowFightPhase()
    {
        splashScreenUI.SetActive(false);
        armorPhaseUI.SetActive(false);
        fightPhaseUI.SetActive(true);
    }
}
