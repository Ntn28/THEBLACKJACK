using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    [SerializeField] private List<GameObject> cardPrefabs;
    public Stack<GameObject> deck;

    private readonly SuitEnum[] suits = (SuitEnum[])System.Enum.GetValues(typeof(SuitEnum));
    private readonly int[] values = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

    public int CardsRemaining => deck.Count;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //AssignIDsToCards();
        deck = new Stack<GameObject>(cardPrefabs);
    }

    //private void AssignIDsToCards()
    //{
    //    int index = 0;
    //    foreach (SuitEnum suit in suits)
    //    {
    //        foreach (int value in values)
    //        {
    //            if (index < cardPrefabs.Count)
    //            {
    //                var card = cardPrefabs[index].GetComponent<Card>();
    //                if (card != null)
    //                {
    //                    // Assegna il seme e il valore corretti alla carta
    //                    card.SetCard(suit, value);

    //                    // Log per confermare che le carte sono state settate correttamente
    //                    Debug.Log($"Carta assegnata: {card.GetCardName()}, Valore: {card.Value}");
    //                }
    //                index++;
    //            }
    //        }
    //    }
    //}

    public void ReturnCardToDeck(GameObject card)
    {
        card.transform.SetParent(null);
        card.SetActive(false);
        deck.Push(card); 
        Debug.Log($"Carta {card.GetComponent<Card>().GetCardName()} rimessa nel mazzo.");
    }



    public void OnShuffleButtonPressed()
    {
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        List<GameObject> tempDeck = new List<GameObject>(deck);

        for (int i = 0; i < tempDeck.Count; i++)
        {
            int rnd = Random.Range(0, tempDeck.Count);
            GameObject temp = tempDeck[rnd];
            tempDeck[rnd] = tempDeck[i];
            tempDeck[i] = temp;
        }

        deck = new Stack<GameObject>(tempDeck);

        foreach (var cardPrefab in tempDeck)
        {
            var card = cardPrefab.GetComponent<Card>();
            Debug.Log($"Carta dopo lo shuffle: {card.GetCardName()}, Valore: {card.Value}");
        }

        Debug.Log("Mazzo mescolato correttamente.");
    }

    public GameObject DrawCard()
    {
        if (deck == null || deck.Count == 0)
        {
            Debug.LogError("Il mazzo è vuoto o non inizializzato.");
            return null; 
        }

        GameObject drawnCardPrefab = deck.Pop();
        GameObject cardInstance = Instantiate(drawnCardPrefab);
      
        Card cardComponent = cardInstance.GetComponent<Card>();
        if (cardComponent != null)
        {
            Debug.Log($"Carta estratta: {cardComponent.GetCardName()}, Valore: {cardComponent.Value}");
        }
        else
        {
            Debug.LogError("La carta estratta non ha un componente 'Card'!");
        }

        return cardInstance;
    }

}