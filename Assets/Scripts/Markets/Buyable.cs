using Interactables;
using UnityEngine;

public class Buyable : MonoBehaviour
{
    [SerializeField] private int price;
    private BuyingStation buyingStation;
    private Liftable liftable;
    private Interactable interactable;

    public int Price => price;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        liftable = GetComponent<Liftable>();
        liftable.OnLiftStateChanged += TryBuy;
    }

    public void TryBuy(bool islifted)
    {
        if (islifted)
        {
            Buy();
        }
    }

    public void Buy()
    {
        liftable.OnLiftStateChanged -= TryBuy;
        buyingStation.Buy(this);
        this.enabled = false;
    }

    public void Init(BuyingStation buyingStation)
    {
        this.buyingStation = buyingStation;
    }
    public void InteractableSetActive(bool active)
    {
        interactable ??= GetComponent<Interactable>();
        interactable.enabled = active;
    }
}
