using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BancaManager : MonoBehaviour
{
    public Button[] betButtons;

    [SerializeField] Text moneyUI;
    [SerializeField] Text stackedUI;

    [SerializeField] int initialMoney;
        
    int currentMoney;
    int currentStacked;

    public int CurrentMoney {
        get {
            return currentMoney;
        }
        set {
            currentMoney = value;
            moneyUI.text = value.ToString();
        }
    }

    public int CurrentStacked
    {
        get
        {
            return currentStacked;
        }
        set
        {
            currentStacked = value;
            stackedUI.text = value.ToString();
        }
    }

    private void Awake()
    {
        CurrentMoney = initialMoney;
        CurrentStacked = 0;
    }

    public void Bet(int amount)
    {
        if (CurrentMoney - amount < 0) return;
        
        CurrentMoney -= amount;
        CurrentStacked += amount;
    }

    public void ClearStacked()
    {
        CurrentMoney += CurrentStacked;
        CurrentStacked = 0;
    }

    public void EnableBet(bool enable)
    {
        foreach (Button button in betButtons)
        {
            button.interactable = enable;
        }
    }
}
