using UnityEngine;
using UnityEngine.Rendering;

public class CharacterRenderer : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer[] _renderers;

    private ShadowCastingMode _currentCastingMode;

    public void SetCastingModeToDefault() =>
        SetCastingMode(ShadowCastingMode.On);

    public void SetCastingModeToShadowsOnly() =>
        SetCastingMode(ShadowCastingMode.ShadowsOnly);

    private void SetCastingMode(ShadowCastingMode castingMode)
    {
        if (_currentCastingMode == castingMode)
            return;

        foreach (MeshRenderer meshRenderer in _renderers)
        {
            meshRenderer.shadowCastingMode = castingMode;
        }

        _currentCastingMode = castingMode;
    }
}