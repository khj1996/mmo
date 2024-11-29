using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioObject : Poolable
{
    [SerializeField] private AudioSource audioSource;

    public void PlaySound(AudioClip clip, Vector3 position, float volume)
    {
        transform.position = position;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        StartCoroutine(ReturnToPoolAfterPlayback(clip.length));
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    private IEnumerator ReturnToPoolAfterPlayback(float delay)
    {
        yield return new WaitForSeconds(delay);
        Managers.PoolManager.ReturnToPool(poolName, this);
    }

    public override void OnReturnToPool()
    {
        gameObject.SetActive(false);
    }

    public override void OnGetFromPool()
    {
        gameObject.SetActive(true);
    }
}