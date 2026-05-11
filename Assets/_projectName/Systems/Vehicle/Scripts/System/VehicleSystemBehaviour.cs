using UnityEngine;
namespace Systems.Vehicle.Core
{
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleSystemBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject _prefabAxleWheelCollider;
        [SerializeField] private GameObject _prefabAxleWheelController;
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private VehicleSystem _vehicleSystem;// serialize for looking in inspector
        //hide system in intarface
        public IVehicleLifecycle VehicleLifecycle => _vehicleSystem;
        public IVehicleSystemPassenger VehicleSystemPassenger => _vehicleSystem;
        private void Awake()
        {
            _vehicleSystem = new VehicleSystem(GetComponent<Rigidbody>(), _prefabAxleWheelCollider, _prefabAxleWheelController);
        }
        private void Start()
        {
            _vehicleSystem.Init(_audioSource);
        }
        private void OnDestroy()
        {
            _vehicleSystem.Destroyed();
        }
        private void FixedUpdate()
        {
            _vehicleSystem.Update();
        }
    }
}