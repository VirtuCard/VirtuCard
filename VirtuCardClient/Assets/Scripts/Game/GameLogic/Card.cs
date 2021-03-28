using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    public abstract void Print();
    public abstract bool Compare(Card card);
    public abstract void CopyCard(Card toCopy);
    public abstract string ToNiceString();
    public override abstract string ToString();
}
