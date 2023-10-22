using Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoneyPlace : MonoBehaviour
{
    private readonly List<Money> money = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Money money))
        {
            this.money.Add(money);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Money money))
        {
            this.money.Remove(money);
        }
    }

    public int GetSum() => money.Sum(x => x.Value);
    public void RemoveSum(int value)
    {
        for (int i = 0; i < this.money.Count; i++)
        {
            Money money = this.money[i];
            int toRemove = Mathf.Max(money.Value, value);
            if (toRemove == money.Value)
            {
                this.money.RemoveAt(i);
                i--;
            }
            money.SetValue(toRemove);

            value -= toRemove;
            if (value == 0)
                break;
        }
    }
}
