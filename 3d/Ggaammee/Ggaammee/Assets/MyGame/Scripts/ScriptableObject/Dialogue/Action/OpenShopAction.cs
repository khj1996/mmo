using UnityEngine;

[CreateAssetMenu(fileName = "OpenShopAction", menuName = "ScriptableObjects/Dialogue/DialogueAction/OpenShopAction")]
public class OpenShopAction : DialogueAction
{
    [SerializeField] private string shopId = "shop_";

    public override void Execute()
    {
        PopupUIManager.Instance.OpenPopupById(PopUpName.ShopUI, shopId);
    }
}