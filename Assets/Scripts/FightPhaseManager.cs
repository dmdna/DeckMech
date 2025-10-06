using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class FightPhaseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject fightPhaseUI;            
    public Transform announcementTextContainer;          
    public GameObject announcementTextPrefab;     
    public PlayerRobotPanel player1Panel;      
    public PlayerRobotPanel player2Panel;      
    public GameObject player1TurnText;      
    public GameObject player2TurnText;      

    [Header("Fight UI")]
    public Button drawAttacksButton;           
    public Transform drawnCardsPanel;         
    public GameObject attackCardUIPrefab;
    
    [Header("Robot Visuals")]
    public RobotBuilder player1RobotBuilder;
    public Animator player1Anim;
    public RobotBuilder player2RobotBuilder;
    public Animator player2Anim;



    // runtime
    private DeckManager deckManager;
    private PlayerRobot attacker;
    private PlayerRobot defender;
    private int attackerId;                    // 1 or 2
    private List<GameObject> currentAttackUIs = new List<GameObject>();

    void Start()
    {
        deckManager = FindObjectOfType<DeckManager>();
        if (fightPhaseUI != null) fightPhaseUI.SetActive(false);

        // ensure draw button hooked
        if (drawAttacksButton != null)
        {
            drawAttacksButton.onClick.RemoveAllListeners();
            drawAttacksButton.onClick.AddListener(DrawTwoAttacks);
        }
    }

    // Called by ArmorPhaseManager when both robots ready; shows announcement then populates panels
    public void EnterFightPhase()
    {
        var p1 = GameManager.Instance.player1Robot;
        var p2 = GameManager.Instance.player2Robot;

        if (p1 == null || p2 == null)
        {
            Debug.LogWarning("FightPhaseManager: one or both robots are null when entering fight phase.");
        }

        // populate panels
        player1Panel.Setup(1, p1);
        player2Panel.Setup(2, p2);

        // show announcement then init the fight turn order
        ShowAnnouncementAndStartFight();

        player1RobotBuilder.BuildRobotFromData(p1, true);
        player2RobotBuilder.BuildRobotFromData(p2, false);
    }

    void ShowAnnouncementAndStartFight()
    {
        fightPhaseUI.SetActive(true);
        announcementTextContainer.gameObject.SetActive(true);

        // decide who starts (lowest HP starts)
        var p1 = GameManager.Instance.player1Robot;
        var p2 = GameManager.Instance.player2Robot;

        if (p1.hp <= p2.hp)
        {
            attacker = p1; defender = p2; attackerId = 1;
            player1TurnText.SetActive(true);
            player2TurnText.SetActive(false);
        }
        else
        {
            attacker = p2; defender = p1; attackerId = 2;
            player1TurnText.SetActive(false);
            player2TurnText.SetActive(true);
        }
        StartCoroutine(AnnounceText($"Player {attackerId} (lowest HP) starts as attacker. Please draw your attack card."));

        drawAttacksButton.interactable = true;

        // enable draw button for the attacker
        UpdateUIForTurn();
    }

    IEnumerator AnnounceText(string txt)
    {
        TMP_Text announcement =  Instantiate(announcementTextPrefab, announcementTextContainer).GetComponent<TMP_Text>();
        announcement.text = txt;
        Debug.Log(txt);
        yield return new WaitForSeconds(10);
        Destroy(announcement.gameObject);
        yield return null;
    }

    void UpdateUIForTurn()
    {
        // refresh panels (hp may have changed)
        player1Panel.Setup(1, GameManager.Instance.player1Robot);
        player2Panel.Setup(2, GameManager.Instance.player2Robot);

        // enable draw only for attacker
        drawAttacksButton.interactable = true;
        ClearCurrentAttackUIs();
    }

    // Player action: draw 2 attack card options
    public void DrawTwoAttacks()
    {
        drawAttacksButton.interactable = false; // lock until player chooses
        ClearCurrentAttackUIs();

        for (int i = 0; i < 2; i++)
        {
            CardData attack = deckManager.DrawAttackCard();
            if (attack == null)
            {
                StartCoroutine(AnnounceText($"Attack deck empty when drawing."));
                continue;
            }

            GameObject cardUI = Instantiate(attackCardUIPrefab, drawnCardsPanel);
            cardUI.GetComponent<CardUI>().Setup(attack);

            CardHolder holder = cardUI.GetComponent<CardHolder>();
            holder.cardData = attack;

            // Make button choose this attack
            Button btn = cardUI.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnChooseAttack(holder));

            currentAttackUIs.Add(cardUI);
        }
    }

    // Attacker chose an attack option
    void OnChooseAttack(CardHolder chosenHolder)
    {
        CardData attackCard = chosenHolder.cardData;
        // Discard the *unused* option(s)
        foreach (var ui in currentAttackUIs)
        {
            CardHolder h = ui.GetComponent<CardHolder>();
            if (h != chosenHolder)
            {
                deckManager.DiscardAttack(h.cardData);
                Destroy(ui);
            }
        }
        currentAttackUIs.Clear();


        SFXManager.Instance.PlaySFX(3);

        // Apply attack to defender (with last-stand check)
        ApplyAttackWithLastStand(attackCard);
        // After resolution, discard the used attack card as well (if not used in last-stand)
        deckManager.DiscardAttack(attackCard);
    }

    void ApplyAttackWithLastStand(CardData attackCard)
    {
        // compute defender's resistance for this attack type
        int defenderRes = GetResistanceFor(defender, attackCard.damageType);
        int incomingDamage = Mathf.Max(0, attackCard.damage - defenderRes);

        StartCoroutine(AnnounceText($"Player {attackerId} attacks Player {(attackerId == 1 ? 2 : 1)} with {attackCard.cardName} ({attackCard.damageType} {attackCard.damage}). Defender res {defenderRes} → net {incomingDamage} damage."));

        // If this attack would kill defender, trigger last-stand
        if (incomingDamage >= defender.hp && defender.hp > 0)
        {
            StartCoroutine(AnnounceText("Last Stand triggered!"));
            HandleLastStand(attackCard, incomingDamage);
            return;
        }

        // Normal apply
        defender.hp = Mathf.Max(0, defender.hp - incomingDamage);
        UpdatePanels();


        if (attackerId == 1) { player1Anim.SetTrigger("Attack"); } else { player2Anim.SetTrigger("Attack"); }

        // check for win
        if (defender.hp <= 0)
        {
            // ensure defender hp is exactly 0 in the model and UI before defeat handling
            defender.hp = 0;
            UpdatePanels();
            OnPlayerDefeated((attackerId == 1) ? 2 : 1);
            return;
        }
        else
        {
            if (attackerId == 1) {
                player2Anim.SetTrigger("Hit"); 
            }
            else 
            { 
                player1Anim.SetTrigger("Hit");
            }
        }


        // continue to next turn (swap attacker/defender)
        SwapTurns();
    }

    void HandleLastStand(CardData incomingAttack, int incomingDamage)
    {
        // Defender draws 1 attack card for last stand
        CardData lastCard = deckManager.DrawAttackCard();
        if (lastCard == null)
        {
            StartCoroutine(AnnounceText("Attack deck empty during last stand."));
            // force defender to 0 HP, update UI, and trigger defeat
            defender.hp = 0;
            UpdatePanels();
            OnPlayerDefeated((attackerId == 1) ? 2 : 1);
            return;
        }


        StartCoroutine(AnnounceText($"Last Stand draw: {lastCard.cardName} ({lastCard.damageType} {lastCard.damage})"));

        // compute if parry (same type and enough damage to cancel)
        bool parry = (lastCard.damageType == incomingAttack.damageType) && (lastCard.damage >= incomingDamage);

        // compute if abort (this lastCard, applied to attacker, would be finishing blow)
        int attackerRes = GetResistanceFor(attacker, lastCard.damageType);
        int damageToAttacker = Mathf.Max(0, lastCard.damage - attackerRes);
        bool abort = damageToAttacker >= attacker.hp;

        if (parry)
        {
            StartCoroutine(AnnounceText("Last Stand result: PARRY! Incoming attack nullified."));
            deckManager.DiscardAttack(lastCard);
            ClearCurrentAttackUIs();
            UpdatePanels();

            // attacker retains turn — make it explicit and re-enable draw
            StartCoroutine(AnnounceText($"Player {attackerId} retains the turn (draw again)."));
            drawAttacksButton.interactable = true;
            return;
        }

        if (abort)
        {
            StartCoroutine(AnnounceText("Last Stand result: ABORT! Both attacks canceled."));
            deckManager.DiscardAttack(lastCard);
            ClearCurrentAttackUIs();
            UpdatePanels();

            // attacker retains turn — explicit
            StartCoroutine(AnnounceText($"Player {attackerId} retains the turn (draw again)."));
            drawAttacksButton.interactable = true;
            return;
        }

        // last stand failed -> defender loses
        StartCoroutine(AnnounceText("Last Stand failed; defender dies."));
        deckManager.DiscardAttack(lastCard);
        ClearCurrentAttackUIs();

        // set defender hp to 0 explicitly and show it before handling defeat
        defender.hp = 0;
        UpdatePanels();

        OnPlayerDefeated((attackerId == 1) ? 2 : 1);

    }

    int GetResistanceFor(PlayerRobot robot, DamageType type)
    {
        switch (type)
        {
            case DamageType.Thermal: return robot.thermal;
            case DamageType.Freeze: return robot.freeze;
            case DamageType.Electric: return robot.electric;
            case DamageType.Void: return robot.voidRes;
            case DamageType.Impact: return robot.impact;
            default: return 0;
        }
    }

    void SwapTurns()
    {
        // swap attacker/defender references and IDs
        if (attackerId == 1)
        {
            attackerId = 2;
            attacker = GameManager.Instance.player2Robot;
            defender = GameManager.Instance.player1Robot;
            player1TurnText.SetActive(false);
            player2TurnText.SetActive(true);
        }
        else
        {
            attackerId = 1;
            attacker = GameManager.Instance.player1Robot;
            defender = GameManager.Instance.player2Robot;
            player1TurnText.SetActive(true);
            player2TurnText.SetActive(false);
        }
        StartCoroutine(AnnounceText($"Player {attackerId}, it's your turn — draw 2 attack cards."));

        UpdateUIForTurn();
    }

    void UpdatePanels()
    {
        player1Panel.Setup(1, GameManager.Instance.player1Robot);
        player2Panel.Setup(2, GameManager.Instance.player2Robot);
    }

    void OnPlayerDefeated(int defeatedPlayerId)
    {
        StartCoroutine(AnnounceText($"Player {defeatedPlayerId} defeated. Player {(defeatedPlayerId == 1 ? 2 : 1)} wins!"));

        // disable draw
        drawAttacksButton.interactable = false;
        ClearCurrentAttackUIs();

        UpdatePanels();

        player1TurnText.GetComponent<TMP_Text>().text = "You Win!";
        player2TurnText.GetComponent<TMP_Text>().text = "You Win!";

        if (defeatedPlayerId == 1)
        {
            player1TurnText.SetActive(false);
            player2TurnText.SetActive(true);
        } 
        else
        {
            player1TurnText.SetActive(true);
            player2TurnText.SetActive(false);
        }

        if (defeatedPlayerId == 1) 
        {
            player1Anim.SetBool("Dead", true); 
        }
        else 
        { 
            player2Anim.SetBool("Dead", true);
        }

        SFXManager.Instance.PlaySFX(4);
    }

    void ClearCurrentAttackUIs()
    {
        foreach (var o in currentAttackUIs) Destroy(o);
        currentAttackUIs.Clear();

        // also clear drawnCardsPanel children (safety)
        for (int i = drawnCardsPanel.childCount - 1; i >= 0; i--) Destroy(drawnCardsPanel.GetChild(i).gameObject);
    }
}
