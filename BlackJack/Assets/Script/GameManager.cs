using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Player> players = new List<Player>();
    private int currentPlayerIndex = 0;
    public static GameManager Instance;
    public bool isDecisionPhase = false;

    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI player3ScoreText;
    public TextMeshProUGUI player4ScoreText;

    public TMP_Text player1StatusText;
    public TMP_Text player2StatusText;
    public TMP_Text player3StatusText;

    public Transform[] playerCardPositions;
    public Vector3 cardScale = new Vector3(1f, 1f, 1f);
    public float cardSpacing = 0.4f;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        players.RemoveAll(player => player == null);
        InitializeDeck();
        UpdateAllPlayerScoresUI();
    }



    public void UpdateAllPlayerScoresUI()
    {
        for (int i = 0; i < players.Count; i++)
        {
            UpdatePlayerScoreUI(i);
        }
    }

    public void UpdatePlayerActionUI(int playerIndex, string action)
    {
        switch (playerIndex)
        {
            case 1:
                player1StatusText.text = action;
                break;
            case 2:
                player2StatusText.text = action;
                break;
            case 3:
                player3StatusText.text = action;
                break;
            default:
                Debug.LogWarning("Player index non valido per l'aggiornamento dell'azione.");
                break;


        }
    }

    public void UpdatePlayerScoreUI(int playerIndex)
    {
        int score = players[playerIndex].GetScore();

        switch (playerIndex)
        {
            case 0:
                player1ScoreText.text = $"Player 1: {score}";
                break;
            case 1:
                player2ScoreText.text = $"Player 2: {score}";
                break;
            case 2:
                player3ScoreText.text = $"Player 3: {score}";
                break;
            case 3:
                player4ScoreText.text = $"Player 4: {score}";
                break;
            default:
                Debug.LogWarning("Player index non valido per aggiornare il punteggio.");
                break;
        }
    }




    private void InitializeDeck()
    {
        Debug.Log("Mazzo inizializzato.");
    }

    public void StartCardDistribution()
    {
        StartCoroutine(DistributeInitialCards());
    }

    private IEnumerator DistributeInitialCards()
    {
        foreach (var player in players)
        {
            for (int i = 0; i < 2; i++)
            {
                GiveCardToPlayer(player, false);
                yield return new WaitForSeconds(1f);
            }
        }

        if (AllPlayersHaveAtLeastTwoCards())
        {
            Debug.Log("Tutti i giocatori hanno almeno due carte. Inizio la fase decisionale.");

            isDecisionPhase = true;

            StartCoroutine(StartPlayerDecisionPhase());
        }
    }



    public void GiveCardToPlayer(Player player, bool isDuringTurn)
    {
        // Se il punteggio è 21 o superiore, non dare carte
        if (player.GetScore() >= 21)
        {
            Debug.Log($"{player.name} ha raggiunto o superato 21. Non può ricevere altre carte.");
            return;
        }

        // Assegna la carta e aggiorna lo stato
        if (isDecisionPhase || isDuringTurn)
        {
            GameObject card = DeckManager.Instance.DrawCard();
            if (card == null) return; // Verifica che la carta sia valida

            player.ReceiveCard(card); // Assegna la carta al giocatore
            UpdatePlayerScoreUI(players.IndexOf(player)); // Aggiorna il punteggio dopo aver ricevuto una carta

            Debug.Log($"{player.name} ha ricevuto una carta. Punteggio attuale: {player.GetScore()}");
            return;
        }

        // Durante la distribuzione iniziale, limita a 2 carte
        if (player.GetHandSize() >= 2)
        {
            Debug.Log($"{player.name} ha già due carte. Non può riceverne altre.");
            return;
        }

        // Se il giocatore ha meno di 2 carte, dai la carta
        GameObject cardToDistribute = DeckManager.Instance.DrawCard();
        if (cardToDistribute == null) return;

        player.ReceiveCard(cardToDistribute); // Assegna la carta al giocatore
        UpdatePlayerScoreUI(players.IndexOf(player)); // Aggiorna il punteggio dopo la distribuzione iniziale

        // Codice per posizionare la carta nel mondo di gioco se necessario
        int playerIndex = players.IndexOf(player);
        if (playerIndex >= 0 && playerIndex < playerCardPositions.Length)
        {
            Transform cardPosition = playerCardPositions[playerIndex];
            Vector3 newPosition = cardPosition.position + Vector3.right * player.GetHandSize() * cardSpacing;

            cardToDistribute.transform.SetParent(cardPosition, false); // Mantieni la scala e la rotazione locali
            cardToDistribute.transform.localPosition = newPosition;   // Imposta la posizione locale
            cardToDistribute.transform.localScale = cardScale;
        }

        Debug.Log($"{player.name} ha ricevuto una carta durante la distribuzione iniziale. Punteggio attuale: {player.GetScore()}");
    }






    public void RequestCard(Player player)
    {
        if (player.playerFinishedTurn)
        {
            Debug.Log($"{player.name} ha già finito il suo turno e non può chiedere altre carte.");
            return;
        }

        // Aggiorna solo se siamo in fase decisionale (durante i turni, non durante la distribuzione iniziale)
        if (isDecisionPhase)
        {
            Debug.Log($"{player.name} ha richiesto una carta (HIT).");

            UpdatePlayerActionUI(players.IndexOf(player), "HIT");
            StartCoroutine(WaitForCardToBeGiven(player));
        }
        else
        {
            Debug.Log($"{player.name} non può richiedere carte fuori dalla fase decisionale.");
        }
    }






    public IEnumerator StartPlayerTurn()
    {
        while (currentPlayerIndex < players.Count)
        {
            Player currentPlayer = players[currentPlayerIndex];

            // Se il giocatore ha sballato (OUT), salta il turno
            if (currentPlayer.GetScore() > 21)
            {
                Debug.Log($"{currentPlayer.name} ha sballato. Stato: OUT.");
                UpdatePlayerActionUI(currentPlayerIndex, "OUT");
                currentPlayer.playerFinishedTurn = true;
                currentPlayerIndex++;
                continue;
            }


            if (currentPlayerIndex == 0)
            {
                if (currentPlayer.ShouldAskForAnotherCard())
                {
                    Debug.Log($"{currentPlayer.name} (Dealer) ha richiesto una carta.");
                    RequestCard(currentPlayer);
                }
                else
                {
                    Debug.Log($"{currentPlayer.name} (Dealer) non richiede altre carte. Stato: STAND.");
                    UpdatePlayerActionUI(currentPlayerIndex, "STAND");
                    currentPlayer.FinishTurn();
                }
                currentPlayerIndex++;
                continue;
            }


            Debug.Log($"Turno del giocatore {currentPlayerIndex + 1}: {currentPlayer.name}");


            yield return new WaitForSeconds(1f);
            currentPlayer.MakeDecision(OnPlayerDecision);

            // Aspetta che il giocatore termini il turno
            while (!currentPlayer.HasFinishedTurn())
            {
                yield return null;
            }

            if (currentPlayer.playerFinishedTurn)
            {
                currentPlayer.wantAnotherCard = false;
            }

            currentPlayerIndex++;
        }

        Debug.Log("Tutti i giocatori hanno finito i loro turni.");
        if (AllPlayersHaveFinishedTurn())
        {
            StartCoroutine(StartPlayerDecisionPhase());
            CheckWinner();
        }
    }




    private bool AllPlayersHaveFinishedTurn()
    {
        foreach (var player in players)
        {
            if (!player.playerFinishedTurn)
            {
                return false; // C'è almeno un giocatore che non ha finito il turno
            }
        }
        return true; // Tutti i giocatori hanno finito i loro turni
    }


    private IEnumerator WaitForCardToBeGiven(Player player)
    {
        yield return new WaitUntil(() => player.HasFinishedTurn());

        Debug.Log($"Fase decisionale: {GameManager.Instance.isDecisionPhase}");
        GiveCardToPlayer(player, true);
        player.FinishTurn();
    }

    public bool AllPlayersHaveAtLeastTwoCards()
    {
        foreach (var player in players)
        {
            if (player.GetHandSize() < 2) return false;
        }
        return true;
    }

    public IEnumerator StartPlayerDecisionPhase()
    {
        currentPlayerIndex = 0;
        yield return StartCoroutine(StartPlayerTurn());
    }


    private void OnPlayerDecision(Player player, bool wantsAnotherCard)
    {
        if (wantsAnotherCard)
        {
            // Se il giocatore chiede una carta durante il turno, stampiamo HIT
            RequestCard(player); // Aggiorna a HIT
        }
        else
        {
            // Se il giocatore decide di fermarsi, stampiamo STAND
            Debug.Log($"{player.name} ha deciso di fermarsi (STAND).");
            UpdatePlayerActionUI(players.IndexOf(player), "STAND");
            player.FinishTurn();
        }
    }



    public void CheckWinner()
    {
        List<Player> potentialWinners = new List<Player>();
        int bestScore = 0;

        // Trova il punteggio più vicino a 21 che non superi 21
        foreach (var player in players)
        {
            int score = player.GetScore();
            if (score <= 21 && score > bestScore)
            {
                bestScore = score;
                potentialWinners.Clear(); // Svuota la lista, c'è un nuovo miglior punteggio
                potentialWinners.Add(player); // Aggiungi il giocatore corrente
            }
            else if (score == bestScore && score <= 21)
            {
                potentialWinners.Add(player); // Aggiungi alla lista dei potenziali vincitori
            }
        }

        Player winner = null;

        // Controlla se ci sono più giocatori con il punteggio migliore
        if (potentialWinners.Count == 1)
        {
            winner = potentialWinners[0]; // Un solo vincitore
        }
        else if (potentialWinners.Count > 1)
        {
            // Se uno dei giocatori è il dealer (indice 0), vince lui
            Player dealer = players[0];
            if (potentialWinners.Contains(dealer))
            {
                winner = dealer;
            }
            else
            {
                // Se non c'è il dealer, scegli casualmente un vincitore tra i giocatori che hanno pareggiato
                winner = potentialWinners[Random.Range(0, potentialWinners.Count)];
            }
        }

        if (winner != null)
        {
            int winnerIndex = players.IndexOf(winner);
            Debug.Log($"Il giocatore {winnerIndex + 1} ha vinto con un punteggio di {winner.GetScore()}!");
            UpdatePlayerActionUI(winnerIndex, "WINNER"); // Mostra WINNER sull'UI per il vincitore
        }
        else
        {
            Debug.Log("Nessun vincitore, tutti hanno sballato.");
        }
    }

}