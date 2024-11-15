using UnityEngine;

public class DropItem : Poolable
{
    private Rigidbody _rigidbody;

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
    }

    public void Initialize(Vector3 position, Vector3 force)
    {
        transform.position = position;
        gameObject.SetActive(true);
        _rigidbody.AddForce(force, ForceMode.Impulse);
    }
}