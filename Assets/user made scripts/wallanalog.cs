// WallGlitchController.cs (Modified)
using UnityEngine;
using System.Collections;

namespace Kino
{
    [AddComponentMenu("Custom/Wall Glitch Controller")]
    public class WallGlitchController : MonoBehaviour
    {
        #region Public Properties

        // Reference to the script on the player's camera
        [Header("Target Effect")]
        [Tooltip("Drag the Player Camera with the PlayerGlitchEffect script here.")]
        [SerializeField] private PlayerGlitchEffect _playerGlitchEffect;
        
        [Tooltip("The player's camera transform. Used for distance checks.")]
        [SerializeField] private Transform _playerTransform;

        [Header("Glitch Intensity")]
        [SerializeField, Range(0, 1)]
        float _baseIntensity = 0.3f;
        
        [SerializeField, Range(0, 2)]
        float _maxIntensityMultiplier = 2f;

        [Header("Base Glitch Parameters")]
        [Tooltip("These values define the 'look' of the glitch at full intensity.")]
        [SerializeField, Range(0, 1)]
        float _scanLineJitterAmount = 0.75f;

        [SerializeField, Range(0, 1)]
        float _verticalJumpAmount = 0.2f;

        [SerializeField, Range(0, 1)]
        float _horizontalShakeAmount = 0.2f;

        [SerializeField, Range(0, 1)]
        float _colorDriftAmount = 0.5f;

        [Header("Animation Settings")]
        [SerializeField]
        bool _enableRandomGlitches = true;
        
        [SerializeField, Range(0.5f, 10f)]
        float _glitchFrequency = 3f;
        
        [SerializeField, Range(0.1f, 2f)]
        float _glitchDuration = 0.5f;

        [Header("Interaction")]
        [SerializeField]
        bool _enableProximityResponse = true;
        
        [SerializeField]
        float _detectionDistance = 5f;

        [Header("Time-based Variation")]
        [SerializeField]
        bool _enableTimeVariation = true;
        
        [SerializeField, Range(0.1f, 5f)]
        float _timeVariationSpeed = 1f;

        #endregion

        #region Private Properties
        
        // Animation control
        bool _isGlitching = false;
        float _currentIntensity = 0f;
        float _targetIntensity = 0f;
        Coroutine _glitchCoroutine;
        
        // Proximity detection
        float _lastProximityCheck = 0f;

        #endregion

        #region MonoBehaviour Functions

        void Start()
        {
            // Auto-find the player camera and effect if not assigned
            if (_playerGlitchEffect == null)
            {
                if (Camera.main != null)
                {
                    _playerGlitchEffect = Camera.main.GetComponent<PlayerGlitchEffect>();
                    if (_playerGlitchEffect == null)
                    {
                        Debug.LogError("WallGlitchController: Could not find PlayerGlitchEffect script on the main camera.", this);
                        this.enabled = false;
                        return;
                    }
                }
            }
            if (_playerTransform == null && Camera.main != null)
            {
                 _playerTransform = Camera.main.transform;
            }

            if (_playerTransform == null || _playerGlitchEffect == null)
            {
                Debug.LogError("PlayerGlitchEffect or PlayerTransform is not assigned, and Camera.main could not be found. Disabling script.", this);
                this.enabled = false;
                return;
            }
            
            // Start random glitch routine
            if (Application.isPlaying && _enableRandomGlitches)
            {
                StartCoroutine(RandomGlitchRoutine());
            }
        }

        void Update()
        {
            if (!Application.isPlaying || _playerGlitchEffect == null) return;
            
            // Proximity detection
            if (_enableProximityResponse && Time.time - _lastProximityCheck > 0.1f)
            {
                CheckProximity();
                _lastProximityCheck = Time.time;
            }
            
            // Smooth intensity transitions
            _currentIntensity = Mathf.Lerp(_currentIntensity, _targetIntensity, Time.deltaTime * 5f);
            
            // Apply time-based variation
            if (_enableTimeVariation)
            {
                ApplyTimeVariation();
            }
            
            // Update glitch parameters and send them to the renderer script
            UpdateGlitchParameters();
        }

        #endregion

        #region Wall-Specific Features

        void UpdateGlitchParameters()
        {
            float intensity = _baseIntensity + (_currentIntensity * _maxIntensityMultiplier);
            
            // We now set the public properties on the renderer script
            _playerGlitchEffect.scanLineJitter = _scanLineJitterAmount * intensity;
            _playerGlitchEffect.verticalJump = _verticalJumpAmount * intensity;
            _playerGlitchEffect.horizontalShake = _horizontalShakeAmount * intensity;
            _playerGlitchEffect.colorDrift = _colorDriftAmount * intensity;
        }

        void ApplyTimeVariation()
        {
            float timeVariation = (Mathf.PerlinNoise(Time.time * _timeVariationSpeed, 0f) - 0.5f) * 0.5f; // Centered noise
            _targetIntensity = Mathf.Clamp01(_targetIntensity + timeVariation * Time.deltaTime);
        }

        void CheckProximity()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            if (distanceToPlayer <= _detectionDistance)
            {
                // Player is close. We can either trigger a single glitch or ramp up intensity.
                // For this example, we'll trigger a glitch if one isn't already running.
                if (!_isGlitching)
                {
                    float intensity = 1.0f - (distanceToPlayer / _detectionDistance); // Closer = more intense
                    TriggerGlitch(intensity, 1.5f);
                }
            }
        }

        IEnumerator RandomGlitchRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(1f, 5f) / _glitchFrequency);
                
                if (!_isGlitching)
                {
                    float intensity = Random.Range(0.3f, 1f);
                    float duration = Random.Range(_glitchDuration * 0.5f, _glitchDuration * 1.5f);
                    TriggerGlitch(intensity, duration);
                }
            }
        }

        public void TriggerGlitch(float intensity, float duration)
        {
            if (_glitchCoroutine != null)
            {
                StopCoroutine(_glitchCoroutine);
            }
            
            _glitchCoroutine = StartCoroutine(GlitchSequence(intensity, duration));
        }

        IEnumerator GlitchSequence(float intensity, float duration)
        {
            _isGlitching = true;
            
            _targetIntensity = intensity;
            yield return new WaitForSeconds(duration);
            _targetIntensity = 0f;
            
            _isGlitching = false;
        }

        #endregion

        #region Public Methods

        public void SetGlitchIntensity(float intensity)
        {
            _targetIntensity = Mathf.Clamp01(intensity);
        }

        public void TriggerManualGlitch()
        {
            TriggerGlitch(Random.Range(0.5f, 1f), Random.Range(0.5f, 1.5f));
        }

        #endregion
    }
}