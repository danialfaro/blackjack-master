using System;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;

    public int[] values = new int[52];
    int cardIndex = 0;

    CardHand playerCardHand;
    CardHand dealerCardHand;

    BancaManager bancaManager;

    private void Awake()
    {
        InitCardValues();
    }

    /*test*/
    public int playerWins = 0;
    public int dealerWins = 0;

    private void Start()
    {

        playerCardHand = player.GetComponent<CardHand>();
        dealerCardHand = dealer.GetComponent<CardHand>();

        bancaManager = FindObjectOfType<BancaManager>();

        ShuffleCards();
        StartGame();

        /*for (int i = 0; i < 999; i++)
        {
            PlayAgain();
        }*/

        /*do
        {
            PlayAgain();

        } while (!(playerCardHand.points == 20 && (dealerCardHand.cards[1].GetComponent<CardModel>().value == 1 || dealerCardHand.cards[1].GetComponent<CardModel>().value == 11)));*/

        /*bool hasAs = false;
        do
        {
            PlayAgain();


            for (int i = 0; i < 999; i++)
            {
                foreach (GameObject card in playerCardHand.cards)
                {
                    if (card.GetComponent<CardModel>().value == 11)
                    {
                        hasAs = true;
                        goto As;
                    }
                }
                Hit();
            }

            As: { }

        } while (!(playerCardHand.cards.Count > 2 && playerCardHand.points == 19 && hasAs));*/

    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */

        int[] tenValueCards = { 11, 12, 13 };
        for (int i = 0; i < values.Length; i++)
        {
            int value = i % (values.Length / 4) + 1;
            if (value == 1) value = 11;
            else if (Array.IndexOf(tenValueCards, value) != -1)
            {
                value = 10;
            }
            values[i] = value;
        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */
        for (int i = 0; i < values.Length; i++)
        {
            int firstIndex = UnityEngine.Random.Range(0, values.Length);
            int secondIndex = UnityEngine.Random.Range(0, values.Length);

            int firstValue = values[firstIndex];
            values[firstIndex] = values[secondIndex];
            values[secondIndex] = firstValue;

            Sprite firstFace = faces[firstIndex];
            faces[firstIndex] = faces[secondIndex];
            faces[secondIndex] = firstFace;
        }
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
            if (playerCardHand.points == 21)
            {
                EndGame("Player wins!");
            }
            else if (dealerCardHand.points == 21)
            {
                dealerCardHand.InitialToggle();
                EndGame("Dealer wins!");
            }

            CalculateProbabilities();


        }
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador*/
        float probOne = ProbabilityOne();

        /* - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta*/
        float probTwo = ProbabilityTwo();

        /* - Probabilidad de que el jugador obtenga más de 21 si pide una carta*/
        float probThree = ProbabilityThree();

        probMessage.text = "El dealer tenga más puntuación que el jugador: \n" + probOne.ToString() + "\n"
                           + "El jugador obtenga entre un 17 y un 21 si pide una carta: \n" + probTwo.ToString() + "\n"
                           + "El jugador obtenga más de 21 si pide una carta: \n" + probThree.ToString() + "\n";
    }

    float ProbabilityOne()
    {
        float probability = 1;

        if (dealerCardHand.cards.Count < 2)
        {
            probability = 0; //0
            return probability;
        }

        int d1 = dealerCardHand.cards[1].GetComponent<CardModel>().value;
        int pointsDifference = playerCardHand.points - d1;

        if (pointsDifference > 0)
        {
            int y = 4;
            if (pointsDifference != 10) y = (13 - pointsDifference + 1) * 4;

            foreach (GameObject card in playerCardHand.cards)
            {
                if (card.GetComponent<CardModel>().value > pointsDifference) y--;
            }

            foreach (GameObject card in dealerCardHand.cards)
            {
                if (card.GetComponent<CardModel>().value > pointsDifference) y--;
            }

            int d = 52 - (playerCardHand.cards.Count + (dealerCardHand.cards.Count - 1));
            probability = (float)y / d;

            if (probability < 0) probability = 0;

        }

        return probability;

    }

    float ProbabilityTwo()
    {
        float probability = 0;

        int min = 17;
        int max = 21;

        int pointsDifferenceMin = min - playerCardHand.points;
        int pointsDifferenceMax = max - playerCardHand.points;

        Debug.Log("min: " + pointsDifferenceMin + " max: " + pointsDifferenceMax);

        int y = (max - min) * 4;
        if (pointsDifferenceMax == 0) y = 0;
        else if (pointsDifferenceMin < 0) y = 4;

        foreach (GameObject card in playerCardHand.cards)
        {
            int c = card.GetComponent<CardModel>().value;
            if (c == 11) c = 1; // no sabemos diferenciar aqui cuando el as vale 1 o 11
            if (pointsDifferenceMin <= c && c <= pointsDifferenceMax) y--;
        }

        foreach (GameObject card in dealerCardHand.cards)
        {
            int c = card.GetComponent<CardModel>().value;
            if (c == 11) c = 1; // no sabemos diferenciar aqui cuando el as vale 1 o 11
            if (pointsDifferenceMin <= c && c <= pointsDifferenceMax) y--;
        }

        int d = 52 - (playerCardHand.cards.Count + (dealerCardHand.cards.Count - 1));
        probability = (float)y / d;

        return probability;
    }

    float ProbabilityThree()
    {
        float probability = 0;

        int pointsDifferenceMin = 22 - playerCardHand.points;

        int y = 52 - pointsDifferenceMin * 4;

        foreach (GameObject card in playerCardHand.cards)
        {
            int c = card.GetComponent<CardModel>().value;
            if (c == 11) c = 1; // no sabemos diferenciar aqui cuando el as vale 1 o 11
            if (c >= pointsDifferenceMin) y--;
        }

        foreach (GameObject card in dealerCardHand.cards)
        {
            int c = card.GetComponent<CardModel>().value;
            if (c == 11) c = 1; // no sabemos diferenciar aqui cuando el as vale 1 o 11
            if (c >= pointsDifferenceMin) y--;
        }

        int d = 52 - (playerCardHand.cards.Count + (dealerCardHand.cards.Count - 1));
        probability = (float)y / d;

        return probability;
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealerCardHand.Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        playerCardHand.Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        //if (dealerCardHand.cards.Count == 2) dealerCardHand.InitialToggle();


        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (playerCardHand.points > 21) EndGame("Dealer wins!");

        if (playerCardHand.cards.Count > 2) bancaManager.EnableBet(false);
    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        if (dealerCardHand.cards.Count == 2) dealerCardHand.InitialToggle();

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
        while (dealerCardHand.points <= 16)
        {
            PushDealer();
        }

        if (dealerCardHand.points > 21)
        {
            EndGame("Player wins!");
            return;
        }        

        if (dealerCardHand.points > playerCardHand.points) EndGame("Dealer wins!");
        else if (dealerCardHand.points < playerCardHand.points) EndGame("Player wins!");
        else EndGame("Draw!");


    }

    public void EndGame(string message)
    {
        if (message.Equals("Player wins!"))
        {
            bancaManager.CurrentMoney += bancaManager.CurrentStacked * 2;
            finalMessage.color = Color.green;
            playerWins++;
        }
        else if (message.Equals("Dealer wins!"))
        {
            bancaManager.CurrentStacked = 0;
            finalMessage.color = Color.red;
            dealerWins++;
        }

        finalMessage.text = message;
        hitButton.interactable = false;
        stickButton.interactable = false;
        bancaManager.EnableBet(false);
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        bancaManager.EnableBet(true);
        bancaManager.ClearStacked();
        finalMessage.text = "";
        playerCardHand.Clear();
        dealerCardHand.Clear();
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }

}
