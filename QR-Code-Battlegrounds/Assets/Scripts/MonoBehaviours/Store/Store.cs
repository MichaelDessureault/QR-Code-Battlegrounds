using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Price
{
    int  cointAmount;
    float priceAmount;

    public int   CoinAmount { get { return cointAmount; } }
    public float PriceAmount  { get { return priceAmount;  } }

    public Price (int _potionAmount, float _price)
    {
        cointAmount = _potionAmount;
        priceAmount  = _price;
    }
}
 
public class Store : MonoBehaviour {

    private List<Price> prices = new List<Price>();
    public Text potionAmountOwnedText;
    public Text etherAmountOwnedText;
    public Text coinAmountText;

    private StoreData potionInstance;
    
    private void Start()
    {
        prices.Add(new Price(120, 1.99f));
        prices.Add(new Price(375, 4.99f));
        prices.Add(new Price(825, 9.99f));
        prices.Add(new Price(1350, 14.99f));

        potionInstance = StoreData.Instance;
        potionAmountOwnedText.text = "Potions owned: " + potionInstance.PotionAmount;
        etherAmountOwnedText.text = "Ethers owned: " + potionInstance.EtherAmount;
        coinAmountText.text = potionInstance.CoinAmount.ToString();
    } 
    
    public void MakeMoneyPurchase (IndexData indexSelected)
    {
        int index = indexSelected.IndexValue;
        if (index < 0 || index >= prices.Count)
            return;

        var amountBought = prices[index].CoinAmount;

        // update the amount of potions the player has
        potionInstance.CoinAmount += amountBought;

        // setup an alert and say purchase has been made
        var message = "Thank you for your purchase you now have " + potionInstance.CoinAmount + " coins.";
        Debug.Log(message);

        // This is not my code, warpped in a try catch incase something fails
        try {
            AndroidNativeFunctions.ShowAlert(message: message, title: "Purchase Completed", positiveButton: "Ok", negativeButton: "", neutralButton: "", action: ShowAlertAction);
        } catch (System.Exception) {}

        coinAmountText.text = potionInstance.CoinAmount.ToString();
    }

    // This is not my code, warpped in a try catch incase something fails
    void ShowAlertAction(DialogInterface w) {
        try {
            AndroidNativeFunctions.ShowToast(w.ToString());
        } catch (System.Exception) { }
    }

    public void MakeCoinPurchase (IndexData indexSelected)
    {
        int index = indexSelected.IndexValue;
        if (index < 0 || index > 1)
            return;

        switch (index)
        {
            case 0:
                if (potionInstance.CoinAmount >= 150)
                {
                    potionInstance.CoinAmount -= 150;
                    potionInstance.PotionAmount += 10;
                    potionAmountOwnedText.text = "Potions owned: " + potionInstance.PotionAmount;
                    coinAmountText.text = potionInstance.CoinAmount.ToString();
                }
                break;
            case 1:
                if (potionInstance.CoinAmount >= 50)
                {
                    potionInstance.CoinAmount -= 50;
                    potionInstance.EtherAmount += 10;
                    etherAmountOwnedText.text = "Ethers owned: " + potionInstance.EtherAmount;
                    coinAmountText.text = potionInstance.CoinAmount.ToString();
                }
                break;
            default: break;
        }

    }
}