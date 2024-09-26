//using System.Collections.Generic;
//using UnityEngine;

//public class Dealer : MonoBehaviour
//{
//    private List<Card> hand = new List<Card>();
//    private int score = 0;

//    public void ReceiveCard(GameObject cardPrefab)
//    {
//        Card card = cardPrefab.GetComponent<Card>();
//        if (card != null)
//        {
//            hand.Add(card);
//            UpdateScore(cardPrefab); // Aggiorna il punteggio qui
//            Debug.Log($"Carta assegnata al dealer: {card.GetCardName()}, punteggio attuale: {GetScore()}");
//        }
//    }


//    private void UpdateScore(GameObject card)
//    {
//        Card cardScript = card.GetComponent<Card>();
//        int cardValue = cardScript.Value;

//        // Gestione dell'asso
//        if (cardValue == 1)
//        {
//            cardValue = (score + 11 <= 21) ? 11 : 1;
//        }

//        score += cardValue;
//        Debug.Log($"Punteggio aggiornato del dealer: {score}"); // Verifica se il punteggio viene aggiornato
//    }



//    public int GetScore()
//    {
//        return score; // Non serve sommare di nuovo, il punteggio è già aggiornato
//    }

//    public int GetHandSize()
//    {
//        return hand.Count;
//    }
//}
