using UnityEngine;
using TMPro;
using System.Collections;

public class FightPhaseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject fightPhaseUI;            // Root panel to enable
    public TMP_Text announcementText;          // "Both robots ready... switching..." text
    public PlayerRobotPanel player1Panel;      // assign Player1Panel (PlayerRobotPanel component)
    public PlayerRobotPanel player2Panel;      // assign Player2Panel

    [Header("Timing")]
    public float announcementDuration = 2.0f;  // seconds to show announcement before enabling fight controls

    void Start()
    {
        // ensure hidden at start (optional)
        if (fightPhaseUI != null) fightPhaseUI.SetActive(false);
    }

    // Call this to enter fight phase (e.g., from ArmorPhaseManager)
    public void EnterFightPhase()
    {
        // Populate panels with robots from GameManager
        var p1 = GameManager.Instance.player1Robot;
        var p2 = GameManager.Instance.player2Robot;

        if (p1 == null || p2 == null)
        {
            Debug.LogWarning("FightPhaseManager: one or both robots are null when entering fight phase.");
        }

        // update UI text immediately (we'll show announcement first)
        player1Panel.Setup(1, p1);
        player2Panel.Setup(2, p2);

        // show announcement and then enable fight UI
        StartCoroutine(ShowAnnouncementAndActivate());
    }

    IEnumerator ShowAnnouncementAndActivate()
    {
        // show panel but maybe hide content until announcement finishes (optional)
        fightPhaseUI.SetActive(true);

        // display announcement
        announcementText.text = "Both robots ready — entering Fight Phase!";
        announcementText.gameObject.SetActive(true);

        // wait a bit
        yield return new WaitForSeconds(announcementDuration);

        // hide announcement after delay (or leave it, choose what UX you want)
        announcementText.gameObject.SetActive(false);

        // Now: you can enable fight controls here (if separated). For now panels are populated and visible.
        // If you have other fight phase initialization, call it here.
        OnFightPhaseReady();
    }

    void OnFightPhaseReady()
    {
        // placeholder for further fight phase setup (turn order, draw first attack, etc.)
        Debug.Log("FightPhaseManager: Fight Phase is ready.");
    }
}
