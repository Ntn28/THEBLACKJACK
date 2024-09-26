using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckClick : MonoBehaviour
{
    private DeckManager deckManager;

    private void Start()
    {
        deckManager = FindObjectOfType<DeckManager>();
        if (deckManager == null)
        {
            Debug.LogError("DeckManager non trovato!");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == this.gameObject)  
                {
                    GameObject card = deckManager.DrawCard(); 
                    if (card != null)
                    {
                        CardDragManager.Instance.StartDragging(card);  
                    }
                    else
                    {
                        Debug.LogWarning("Nessuna carta disponibile!");
                    }
                }
            }
        }
    }
}