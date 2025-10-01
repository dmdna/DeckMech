using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ArmorPhaseManager : MonoBehaviour
{
    [Header("Managers")]
    public DeckManager deckManager;
    public TurnManager turnManager;
    public UIManager uiManager;
    public FightPhaseManager fightManager;

    [Header("UI References")]
    public Transform drawPanel;
    public Transform[] hangarColumns;     // 0=Helmet, 1=Chest, 2=Gauntlets, 3=Legs
    public GameObject cardUIPrefab;
    public Button drawButton;

    private List<GameObject> currentDraw = new List<GameObject>();
    private bool selectingArmorSet = false;

    // --- DRAW ---
    public void DrawThreeArmor()
    {
        if (selectingArmorSet) return;
        if (turnManager.IsPlayerDone(turnManager.CurrentPlayer)) return;

        ClearDrawPanel();
        turnManager.UpdatePhaseText("draw cards");

        for (int i = 0; i < 3; i++)
        {
            CardData card = deckManager.DrawArmorCard();
            if (card == null) continue;

            GameObject cardUI = Instantiate(cardUIPrefab, drawPanel);
            cardUI.GetComponent<CardUI>().Setup(card);

            CardHolder holder = cardUI.GetComponent<CardHolder>();
            holder.cardData = card;

            Button btn = cardUI.GetComponent<Button>();
            btn.onClick.AddListener(() => PickCard(holder));

            currentDraw.Add(cardUI);
        }

        drawButton.interactable = false;
    }

    // --- PICK ---
    void PickCard(CardHolder chosenHolder)
    {
        CardData chosen = chosenHolder.cardData;

        // Add to hangar
        Transform column = hangarColumns[(int)chosen.armorSlot];
        GameObject cardUI = Instantiate(cardUIPrefab, column);
        cardUI.GetComponent<CardUI>().Setup(chosen);
        cardUI.GetComponent<CardHolder>().cardData = chosen;

        // Discard other two
        foreach (GameObject ui in currentDraw)
        {
            CardHolder holder = ui.GetComponent<CardHolder>();
            if (holder.cardData != chosen)
            {
                deckManager.DiscardArmor(holder.cardData);
            }
            Destroy(ui);
        }
        currentDraw.Clear();

        // Check for full set
        if (HangarComplete())
        {
            selectingArmorSet = true;
            turnManager.UpdatePhaseText("choose your armor set");
            PromptArmorSelection();
        }
        else
        {
            // Next player's turn
            turnManager.NextTurn();
            drawButton.interactable = true;
        }
    }

    // --- ARMOR SELECTION ---
    void PromptArmorSelection()
    {
        selectingArmorSet = true;

        // Go through each column
        foreach (Transform column in hangarColumns)
        {
            if (column.childCount == 1)
            {
                // Auto-select if only one option
                AutoSelect(column.GetChild(0).GetComponent<CardHolder>());
            }
            else
            {
                // Multiple options → wait for clicks
                foreach (Transform card in column)
                {
                    CardHolder holder = card.GetComponent<CardHolder>();
                    Button btn = card.GetComponent<Button>();
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => {
                        // ✅ Only current player can pick
                        if (turnManager.IsPlayerDone(turnManager.CurrentPlayer)) return;
                        MarkSelected(holder);
                    });
                }
            }
        }
    }


    void AutoSelect(CardHolder holder)
    {
        holder.selected = true;
        holder.GetComponent<Image>().color = Color.green;
    }

    void MarkSelected(CardHolder holder)
    {
        foreach (Transform sibling in holder.transform.parent)
        {
            sibling.GetComponent<CardHolder>().selected = false;
            sibling.GetComponent<Image>().color = Color.white;
        }
        holder.selected = true;
        holder.GetComponent<Image>().color = Color.green;
        TryFinalizeRobot();
    }

    void TryFinalizeRobot()
    {
        foreach (Transform column in hangarColumns)
        {
            bool hasSelection = false;
            foreach (Transform card in column)
            {
                CardHolder h = card.GetComponent<CardHolder>();
                if (h.selected) { hasSelection = true; break; }
            }
            if (!hasSelection) return; // wait
        }
        AssembleRobot();
    }

    void AssembleRobot()
    {
        int playerId = turnManager.CurrentPlayer;
        if (turnManager.IsPlayerDone(playerId)) return; // prevent double build

        PlayerRobot robot = new PlayerRobot(playerId);

        // Collect stats from selected pieces
        foreach (Transform column in hangarColumns)
        {
            foreach (Transform card in column)
            {
                CardHolder holder = card.GetComponent<CardHolder>();
                if (holder.selected)
                {
                    CardData piece = holder.cardData;
                    robot.hp += piece.hp;
                    robot.thermal += piece.thermal;
                    robot.freeze += piece.freeze;
                    robot.electric += piece.electric;
                    robot.voidRes += piece.voidRes;
                    robot.impact += piece.impact;

                    // Remove chosen armor from Hangar
                    Destroy(card.gameObject);
                    break;
                }
            }
        }

        Debug.Log($"Player {playerId} built Robot! HP:{robot.hp} | T:{robot.thermal} | F:{robot.freeze} | E:{robot.electric} | V:{robot.voidRes} | I:{robot.impact}");

        GameManager.Instance.SaveRobot(robot);
        turnManager.MarkPlayerDone(playerId);

        selectingArmorSet = false;

        if (turnManager.BothPlayersDone())
        {
            Debug.Log("Both robots ready, switch to Fight Phase!");
            fightManager.EnterFightPhase();
            uiManager.ShowFightPhase();
        }
        else
        {
            turnManager.NextTurn();
            drawButton.interactable = true;
        }
    }

    bool HangarComplete()
    {
        foreach (Transform column in hangarColumns)
        {
            if (column.childCount == 0) return false;
        }
        return true;
    }

    void ClearDrawPanel()
    {
        foreach (GameObject ui in currentDraw) Destroy(ui);
        currentDraw.Clear();
    }
}
