using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shuffle : MonoBehaviour
{
    private DeckManager deckManager;

    private void Start()
    {
        deckManager = FindObjectOfType<DeckManager>();

        if (deckManager == null)
        {
            Debug.LogError("DeckManager non trovato in scena!");
        }
    }

    private void OnMouseDown()
    {
        if (deckManager != null)
        {
            deckManager.ShuffleDeck();  
            Debug.Log("Mazzo mescolato dal tasto!");
        }
        else
        {
            Debug.LogError("DeckManager non disponibile!");
        }
    }
}