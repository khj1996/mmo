using UnityEngine;

public class DropItem : Poolable
{
    private Rigidbody _rigidbody;
    [SerializeField] private ItemData _itemData;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnGetFromPool()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    public override void OnReturnToPool()
    {
        _rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void Initialize(ItemData itemData, Vector3 position, Vector3 force)
    {
        transform.position = position;
        _itemData = itemData;
        gameObject.SetActive(true);
        _rigidbody.AddForce(force, ForceMode.Impulse);
    }

    public void Interact(PlayerController player)
    {
        if (player.AddItemToInventory(_itemData))
        {
            Managers.PoolManager.ReturnToPool(this); 
        }
    }
}