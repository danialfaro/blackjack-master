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

    private void Awake()
    {
        InitCardValues();

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();
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
            if (Array.IndexOf(tenValueCards, value) != -1)
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
            if (player.GetComponent<CardHand>().points == 21) finalMessage.text = "Player wins!";
            else if (dealer.GetComponent<CardHand>().points == 21) finalMessage.text = "Dealer wins!";

        }
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        CardHand dealerCardHand = dealer.GetComponent<CardHand>();
        if (dealerCardHand.cards.Count == 2) dealerCardHand.InitialToggle();

        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (player.GetComponent<CardHand>().points > 21) finalMessage.text = "Dealer wins!";
    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        if (dealer.GetComponent<CardHand>().cards.Count == 2) dealer.GetComponent<CardHand>().InitialToggle();

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
        while (dealer.GetComponent<CardHand>().points <= 16)
        {
            PushDealer();
        }

        if (dealer.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "Player wins!";
            return;
        }

        if(dealer.GetComponent<CardHand>().points > player.GetComponent<CardHand>().points) finalMessage.text = "Dealer wins!";
        else if (dealer.GetComponent<CardHand>().points < player.GetComponent<CardHand>().points) finalMessage.text = "Player wins!";
        else finalMessage.text = "Draw!";


    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }    

}
