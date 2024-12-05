using Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopUISub : UI_ScrollView_Sub
{
    [SerializeField] public Image icon;
    [SerializeField] public TMP_Text productName;
    [SerializeField] public TMP_Text description;
    [SerializeField] public TMP_Text price;
    [SerializeField] public Button buy;

    public ProductData productData;

    public override void Init()
    {
        buy.gameObject.BindEvent((evt) => { buyItem(); });
    }


    public void buyItem()
    {
        Managers.ShopManager.BuyItem(productData.productId);
    }

    public override void RefreshUI()
    {
        ItemData itemData = Managers.DataManager.ItemDatas.GetData(productData.itemId);

        icon.sprite = itemData?.itemSprite;
        productName.text = itemData?.itemName;
        description.text = itemData?.description;
        price.text = productData.price.ToString();
    }
}