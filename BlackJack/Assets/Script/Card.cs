using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Card : MonoBehaviour
{
    [SerializeField] private SuitEnum suit;  
    [SerializeField] private int value;  

    public SuitEnum Suit => suit; 
    public int Value => value; 

    public void SetCard(SuitEnum newSuit, int newValue)
    {
        suit = newSuit;
        value = newValue;
    }

    public string GetCardName()
    {
        return $"{suit} {value}";
    }

    public override string ToString()
    {
        return $"{value} di {suit}";
    }
}