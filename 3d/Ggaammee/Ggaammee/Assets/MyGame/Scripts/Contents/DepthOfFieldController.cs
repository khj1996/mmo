using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthOfFieldController : MonoBehaviour
{
    public Volume volume;
    [SerializeField]private DepthOfField depthOfField;

    public float focusDistance = 5.0f; // 애니메이션 가능한 값

    private void Awake()
    {
        if (volume.profile.TryGet(out depthOfField))
        {
            depthOfField.focusDistance.overrideState = true;
        }
    }

    private void Update()
    {
        if (depthOfField != null)
        {
            depthOfField.focusDistance.value = focusDistance;
        }
    }
}