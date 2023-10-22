using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyingStation : MonoBehaviour
{
    [SerializeField] private List<Buyable> buyables;
    [SerializeField] private MoneyPlace moneyPlace;
     public Collider marketArea;

    private Dictionary<Buyable, Buyable> buyablePrefabs;

    protected void Awake()
    {
        buyablePrefabs = new();
        foreach(var buyable in buyables)
        {
            buyable.gameObject.SetActive(true);
            buyable.Init(this);

            var newPrefab = Instantiate(buyable);
            newPrefab.transform.position = buyable.transform.position;
            newPrefab.transform.rotation = buyable.transform.rotation;
            newPrefab.gameObject.SetActive(false);
            buyablePrefabs.Add(buyable, newPrefab);
        }
    }
    private void OnEnable()
    {
        moneyPlace.OnMoneyChange += UpdateBuyables;
        UpdateBuyables();
    }
    private void OnDisable()
    {
        moneyPlace.OnMoneyChange -= UpdateBuyables;
    }

    public void UpdateBuyables()
    {
        int sum = moneyPlace.GetSum();

        foreach (var buyable in buyables)
        {
            buyable.InteractableSetActive(buyable.Price <= sum);
        }
    }

    public void Buy(Buyable buyable)
    {
        buyables.Remove(buyable);
        moneyPlace.RemoveSum(buyable.Price);
        StartCoroutine(SpawnItem(buyable));
    }

    private IEnumerator SpawnItem(Buyable buyable)
    {
        yield return new WaitForSeconds(3);

        if (buyablePrefabs.TryGetValue(buyable, out Buyable buyablePrefab))
        {
            var newBuyable = Instantiate(buyablePrefab);
            newBuyable.gameObject.SetActive(true);
            newBuyable.transform.position = buyablePrefab.transform.position;
            newBuyable.transform.rotation = buyablePrefab.transform.rotation;
            newBuyable.Init(this);
            buyables.Add(newBuyable);
            buyablePrefabs.Remove(buyable);
            buyablePrefabs.Add(newBuyable, buyablePrefab);
            UpdateBuyables();
        }
        else
            Debug.LogError("Dupa");
    }
}
