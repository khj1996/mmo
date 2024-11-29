using UnityEngine;

public class SoundManager
{
    private string poolName = "AudioObject"; // AudioObject 풀 이름
    private AudioSource bgmSource;

    public SoundManager()
    {
        Managers.PoolManager.PrewarmPools<AudioObject>(poolName);

        // 배경음 전용 AudioSource 생성
        GameObject bgmObject = new GameObject("BGM Source");
        bgmSource = bgmObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        PlayBGM(Managers.Instance.bgmClip, 0.08f);
    }

    /// <summary>
    /// 배경음악 재생
    /// </summary>
    public void PlayBGM(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            return;
        }

        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.Play();
    }

    /// <summary>
    /// 배경음악 정지
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    /// <summary>
    /// 효과음 재생 (풀링 사용)
    /// </summary>
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
        {
            return;
        }

        var audioObject = Managers.PoolManager.GetFromPool<AudioObject>(poolName);

        if (audioObject != null)
        {
            audioObject.PlaySound(clip, position, volume);
        }
    }
}