using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<GameObject> hand = new List<GameObject>();
    public int score = 0;
    private bool finishedTurn = false;
    public float riskFactor = 0.7f;
    public bool wantAnotherCard = false;
    public bool playerFinishedTurn = false;

    public void ReceiveCard(GameObject card)
    {
        if (score > 21)
        {
            return;
        }

        hand.Add(card);
        UpdateScore(card);
        Debug.Log($"{name} ha ricevuto una carta. Punteggio attuale: {score}.");

        int playerIndex = GameManager.Instance.players.IndexOf(this);
        GameManager.Instance.UpdatePlayerScoreUI(playerIndex);

        if (score > 21)
        {
            Debug.Log($"{name} ha sballato!");
            GameManager.Instance.UpdatePlayerActionUI(playerIndex, "OUT");
            FinishTurn();
        }
        else
        {
            GameManager.Instance.UpdatePlayerActionUI(playerIndex, "HIT");
        }
    }



    private void UpdateScore(GameObject card)
    {
        Card cardScript = card.GetComponent<Card>();
        int cardValue = cardScript.Value;

        // Gestione dell'asso
        if (cardValue == 1)
        {
            cardValue = (score + 11 <= 21) ? 11 : 1;
        }

        score += cardValue;
    }

    public int GetHandSize() { return hand.Count; }
    public int GetScore() { return score; }
    public bool HasFinishedTurn() { return finishedTurn; }

    public void MakeDecision(System.Action<Player, bool> callback)
    {
        callback(this, ShouldAskForAnotherCard());
    }


    public bool ShouldAskForAnotherCard()
    {
        if (score > 21) return false;
        if (GetHandSize() < 2) return false;

        if (score >= 21) return false;


        if (score < 17)
        {
            wantAnotherCard = true;
            return true;
        }
        return false;
    }


    public void FinishTurn()
    {
        finishedTurn = true;
        playerFinishedTurn = true;
        Debug.Log($"{name} ha deciso di fermarsi con un punteggio di {score}.");
    }
}