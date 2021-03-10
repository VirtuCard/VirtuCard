using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    public abstract void Print();
    public abstract bool Compare(Card card);
    public override abstract string ToString();
}
