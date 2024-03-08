using UnityEngine;

namespace Player
{
	using Tools;

	/// <summary>
	/// Controls the particle emissions for the bike tire based on its movement and interaction with the environment.
	/// </summary>
	[RequireComponent(typeof(Rigidbody2D)), DefaultExecutionOrder(10)]
	public class BikeTireParticles : MonoBehaviour
	{
		private enum Side
		{
			Left,
			Right
		}
		
		[SerializeField] private float maxVelocity = 40f;
		[SerializeField] private float minAngularVelocity = 500f;
		[SerializeField] private float dirtFallOffRate = 0.1f;
		[SerializeField] private Side tireSide;
		[SerializeField] private ParticleSystem tireParticleSystem;
		[SerializeField] private PlayerRefs playerRefs;

		private Rigidbody2D _rigidbody;
		private bool _isRearTire;
		private bool _isThrottling;
		private int _levelColliderTouchCounter;
		private float _dirtAccumulation = 1f;
		private float _defaultEmissionRate = 1f;
		private float _defaultEmissionSpeed = 1f;
		private const string ColliderName = "LevelCollider";

		private void Awake()
		{
			this.IsReferenceNull(tireParticleSystem);
			_defaultEmissionRate = tireParticleSystem.emission.rateOverTimeMultiplier;
			_defaultEmissionSpeed = tireParticleSystem.main.startSpeedMultiplier;
			
			_rigidbody = GetComponent<Rigidbody2D>();
		}

		private void Start()
		{
			OnFlipStateChange();
			OnDrivingStateChange();
		}

		private void OnEnable()
		{
			playerRefs.States.OnFlipStateChange += OnFlipStateChange;
			playerRefs.States.OnDrivingStateChange += OnDrivingStateChange;
		}

		private void OnDisable()
		{
			playerRefs.States.OnFlipStateChange -= OnFlipStateChange;
			playerRefs.States.OnDrivingStateChange -= OnDrivingStateChange;
		}

		private void OnFlipStateChange()
		{
			_isRearTire =
				(tireSide == Side.Left && !playerRefs.States.IsFlipped) ||
				(tireSide == Side.Right && playerRefs.States.IsFlipped);
			
			if (!_isRearTire) SetEmissionRate(0);
		}

		private void OnDrivingStateChange()
		{
			_isThrottling = playerRefs.States.DrivingState == BikeStates.DrivingStates.Throttle;
			
			if (!_isThrottling) SetEmissionRate(0);
		}
		
		private void OnCollisionEnter2D(Collision2D other)
		{
			if (!other.gameObject.CompareTag(ColliderName)) return;
			
			_levelColliderTouchCounter++;
			_dirtAccumulation = 1f;
		}
		
		private void OnCollisionExit2D(Collision2D other)
		{
			if (other.gameObject.CompareTag(ColliderName)) 
				_levelColliderTouchCounter--;

			if (_levelColliderTouchCounter > 0) return;
			_levelColliderTouchCounter = 0;
			_dirtAccumulation = 0.5f;
		}

		private void LateUpdate()
		{
			tireParticleSystem.transform.position = transform.position;
		}

		private void FixedUpdate()
		{
			if (!_isRearTire || !_isThrottling) return;
			
			// If tire is not touching ground, start reducing its dirt accumulation.
			if (_levelColliderTouchCounter <= 0) _dirtAccumulation -= dirtFallOffRate;
			_dirtAccumulation = Mathf.Clamp01(_dirtAccumulation);

			var normalizedAngularVelocity = Mathf.InverseLerp(0f, minAngularVelocity, _rigidbody.angularVelocity.Abs());
			var normalizedVelocity = Mathf.InverseLerp(0f, maxVelocity, _rigidbody.velocity.sqrMagnitude);

			var emissionMultiplier = 0.25f + (Mathf.Max(normalizedAngularVelocity - normalizedVelocity, 0f) * 0.75f);
			SetEmissionRate(_defaultEmissionRate * emissionMultiplier * _dirtAccumulation);

			var emissionSpeed = _defaultEmissionSpeed * emissionMultiplier;
			SetEmissionSpeed(emissionSpeed);
		}

		private void SetEmissionRate(float rate)
		{
			var emission = tireParticleSystem.emission;

			emission.rateOverTimeMultiplier = rate;
			emission.enabled = rate > 0.0001f;
		}

		private void SetEmissionSpeed(float speed)
		{
			var main = tireParticleSystem.main;
			
			main.startSpeedMultiplier = speed;
		}
	}
}
