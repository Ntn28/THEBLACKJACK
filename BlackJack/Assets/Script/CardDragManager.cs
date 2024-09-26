using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class CardDragManager : MonoBehaviour
{
    public static CardDragManager Instance;
    private GameObject cardToDrag;
    private bool isDragging = false;
    private Vector3 originalPosition;
    private Transform originalParent;

    public Vector3 cardScale = new Vector3(1f, 1f, 1f);
    public float cardSpacing;

    private void Awake()
    {
        Instance = this;
    }

    public void StartDragging(GameObject card)
    {
        cardToDrag = card;
        isDragging = true;
        originalPosition = card.transform.position;
        originalParent = card.transform.parent;

        Card cardComponent = card.GetComponent<Card>();
        Debug.Log($"Inizio trascinamento: {cardComponent.GetCardName()}, Valore: {cardComponent.Value}");
    }

    public void StopDragging()
    {
        if (isDragging && cardToDrag != null)
        {
            isDragging = false;

            Collider[] colliders = Physics.OverlapSphere(cardToDrag.transform.position, 0.5f);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    Player player = collider.GetComponent<Player>();
                    // Controlla se il giocatore ha già due carte
                    if (player.GetHandSize() < 2)
                    {
                        Debug.Log($"{player.name} richiede la carta.");
                        SnapCardToPlayer(collider.gameObject, collider.transform.position);
                    }
                    else if (player.wantAnotherCard)
                    {
                        player.ReceiveCard(cardToDrag);
                        SnapCardToPlayer(collider.gameObject, collider.transform.position);
                        Debug.Log($"{name} ha ricevuto la terza carte. Punteggio attuale: {player.score}.");

                    }
                    else
                    {
                        Debug.Log($"{player.name} ha già due carte e non può prenderne altre.");
                        ReturnCardToDeck();
                    }
                }
            }

            Debug.Log("Carta tornata al mazzo.");
            ReturnCardToDeck();
        }
    }

    private void Update()
    {
        if (isDragging && cardToDrag != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                cardToDrag.transform.position = hit.point;
            }

            if (Input.GetMouseButtonUp(0))
            {
                StopDragging();
            }
        }
    }

    private void SnapCardToPlayer(GameObject playerObject, Vector3 position)
    {
        if (cardToDrag != null)
        {
            Card cardComponent = cardToDrag.GetComponent<Card>();
            Debug.Log($"Carta assegnata al player: {playerObject.name}, Nome: {cardComponent.GetCardName()}, Valore: {cardComponent.Value}");

            Transform cardAnchor = playerObject.transform.Find("CardAnchor");
            if (cardAnchor != null)
            {
                cardToDrag.transform.SetParent(cardAnchor);

                cardToDrag.transform.rotation = cardAnchor.rotation;

                Player player = playerObject.GetComponent<Player>();
                int handSize = player.GetHandSize();

                float cardSpacing = 0.2f;
                Vector3 newPosition = new Vector3(handSize * cardSpacing, 0, 0);

                cardToDrag.transform.localPosition = newPosition;
                cardToDrag.transform.localScale = Vector3.one;

                Rigidbody cardRigidbody = cardToDrag.GetComponent<Rigidbody>();
                if (cardRigidbody != null)
                {
                    cardRigidbody.isKinematic = true;
                }

                player.ReceiveCard(cardToDrag);

                if (CheckAllPlayersHaveTwoCards())
                {
                    Debug.Log("Tutti i giocatori hanno due carte. Inizio della fase decisionale.");
                    StartCoroutine(GameManager.Instance.StartPlayerDecisionPhase());
                }

                cardToDrag = null;
            }
            else
            {
                Debug.LogError($"Il player {playerObject.name} non ha un CardAnchor. Assicurati che esista come figlio.");
            }
        }
    }





    private bool CheckAllPlayersHaveTwoCards()
    {
        foreach (var player in GameManager.Instance.players)
        {
            if (player.GetHandSize() < 2)
            {
                return false;
            }
        }
        return true;
    }

    private void ReturnCardToDeck()
    {
        if (cardToDrag != null)
        {
            cardToDrag.transform.position = originalPosition;
            cardToDrag.transform.SetParent(originalParent);

            Rigidbody cardRigidbody = cardToDrag.GetComponent<Rigidbody>();
            if (cardRigidbody != null)
            {
                cardRigidbody.isKinematic = false;
            }

            Debug.Log("Carta tornata al mazzo.");
        }
    }
}