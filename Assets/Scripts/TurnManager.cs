using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public TMP_Text playerTurnText;
    private int currentPlayer = 1;
    private bool player1Done = false;
    private bool player2Done = false;

    public int CurrentPlayer => currentPlayer;

    public void NextTurn()
    {
        // Switch players, but skip the one who is done
        if (currentPlayer == 1 && !player2Done) currentPlayer = 2;
        else if (currentPlayer == 2 && !player1Done) currentPlayer = 1;

        UpdatePhaseText("draw cards");
    }

    public void UpdatePhaseText(string phase)
    {
        if ((currentPlayer == 1 && player1Done) || (currentPlayer == 2 && player2Done))
        {
            playerTurnText.text = $"Player {currentPlayer} finished building. Waiting...";
        }
        else
        {
            playerTurnText.text = $"Player {currentPlayer}, {phase}";
        }
    }

    public void MarkPlayerDone(int playerId)
    {
        if (playerId == 1) player1Done = true;
        else player2Done = true;
    }

    public bool IsPlayerDone(int playerId)
    {
        return (playerId == 1) ? player1Done : player2Done;
    }

    public bool BothPlayersDone()
    {
        return player1Done && player2Done;
    }
}
