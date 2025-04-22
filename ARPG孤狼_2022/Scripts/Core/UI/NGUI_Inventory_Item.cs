using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGUI_Inventory_Item : MonoBehaviour
{
    [HideInInspector]
    public ItemDataConfig itemDataConfig;
    private int amount;
    public int Amount
    {
        get { return amount; }
        set
        {
            if (amount == value)
            {
                return;
            }
            amount = value;
            UpdateText();
        }
    }
    protected UITexture itemSprite;
    public UITexture ItemSprite
    {
        get
        {
            if (itemSprite == null)
                itemSprite = gameObject.GetComponent<UITexture>();
            return itemSprite;
        }
    }
    protected UILabel amountText;
    protected UILabel AmountText
    {
        get
        {
            if (amountText == null)
            {
                amountText = GetComponentInChildren<UILabel>();
            }
            return amountText;
        }
    }

    protected virtual void Start()
    {
        //ItemSprite.raycastTarget = false;
        //AmountText.raycastTarget = false;
    }
    public void SetItem(ItemDataConfig itemDataConfig, int amount = 1)
    {
        this.itemDataConfig = itemDataConfig;

        this.Amount = amount;
        SetSprite(itemDataConfig.GetIcon());

    }
    protected void SetSprite(Texture texture)
    {
        ItemSprite.mainTexture = texture;
    }
    public void AddAmount(int count = 1)
    {
        if (Amount + count > itemDataConfig.capacity) return;
        Amount += count;

    }
    public bool ReduceAmount(int count = 1)
    {
        Amount -= count;
        return Amount <= 0;
    }
    protected void HideText()
    {
        amountText.gameObject.SetActive(false);
    }
    protected void ShowText()
    {
        amountText.gameObject.SetActive(true);
    }
    protected virtual void UpdateText()
    {
        AmountText.text = Amount.ToString();
        if (Amount > 0)
        {
            if (Amount == 1)
            {
                HideText();
            }
            else
            {
                ShowText();
            }
        }
        else
        {
            Dispose();
        }
    }
    protected void Dispose()
    {
        //DestroyImmediate(this.gameObject);
        GameObjectFactory.Instance.PushItem(gameObject);
    }
    public virtual void Exchange(NGUI_Inventory_Item itemTruly)
    {
        ItemDataConfig item = itemTruly.itemDataConfig;
        int amount = itemTruly.amount;
        itemTruly.SetItem(this.itemDataConfig, this.amount);
        this.SetItem(itemDataConfig: item, amount: amount);
    }

    
}
