using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card
{
    public abstract void Print();
    public abstract bool Compare(Card card);
    public abstract void CopyCard(Card toCopy);
    public override abstract string ToString();
}
