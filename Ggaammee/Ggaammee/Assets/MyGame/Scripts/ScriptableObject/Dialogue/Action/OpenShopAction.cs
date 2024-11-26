using UnityEngine;

[CreateAssetMenu(fileName = "OpenShopAction", menuName = "Dialogue/DialogueAction/OpenShopAction")]
public class OpenShopAction : DialogueAction
{
    [SerializeField] private string shopId = "shop_";

    public override void Execute()
    {
        // 팝업 매니저에 상점 열기 요청
        PopupUIManager.Instance.OpenPopupById(PopUpName.ShopUI, shopId);
    }
}