// PlayerGlitchEffect.cs
using UnityEngine;

namespace Kino
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Kino Image Effects/Player Glitch Effect (Renderer)")]
    public class PlayerGlitchEffect : MonoBehaviour
    {
        // This shader should be the same one used by the original script.
        // You'll likely need to find and assign the "AnalogGlitch" shader here in the Inspector.
        [SerializeField] Shader _shader;

        // These public properties will be controlled by our WallGlitchController.
        [Range(0, 1)] public float scanLineJitter = 0;
        [Range(0, 1)] public float verticalJump = 0;
        [Range(0, 1)] public float horizontalShake = 0;
        [Range(0, 1)] public float colorDrift = 0;

        Material _material;
        float _verticalJumpTime;

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            // Calculate shader properties based on the public values
            _verticalJumpTime += Time.deltaTime * verticalJump * 11.3f;

            var sl_thresh = Mathf.Clamp01(1.0f - scanLineJitter * 1.2f);
            var sl_disp = 0.002f + Mathf.Pow(scanLineJitter, 3) * 0.05f;
            _material.SetVector("_ScanLineJitter", new Vector2(sl_disp, sl_thresh));

            var vj = new Vector2(verticalJump, _verticalJumpTime);
            _material.SetVector("_VerticalJump", vj);

            _material.SetFloat("_HorizontalShake", horizontalShake * 0.2f);

            var cd = new Vector2(colorDrift * 0.04f, Time.time * 606.11f);
            _material.SetVector("_ColorDrift", cd);

            Graphics.Blit(source, destination, _material);
        }

        // Optional: Ensure the effect is off if the component is disabled.
        void OnDisable()
        {
            if (_material != null)
            {
                DestroyImmediate(_material);
            }
        }
    }
}