using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sellable : MonoBehaviour
{
    [SerializeField] private int price = 1;

    public int Price => price;
}
