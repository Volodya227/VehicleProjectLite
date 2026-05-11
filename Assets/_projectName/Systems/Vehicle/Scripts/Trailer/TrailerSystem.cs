using UnityEngine;
namespace Systems.Vehicle.Trailer.Core
{
    [RequireComponent(typeof(Rigidbody))]
    public class TrailerSystem : MonoBehaviour
    {
        [SerializeField] private GameObject PrefabAxleCollider;
        [SerializeField] private GameObject PrefabAxleController;
        private Rigidbody _body;
        private Attach.Core.AttachController _controller;
        [SerializeField] private Transform _transformStay;
        [SerializeField] private Transmission.TrailerTransmission _transmission;
        [SerializeField] private Data.TrailerData _data = new();
        [SerializeField] private LayerMask _attachConnectMask = new();
        [SerializeField] private Transform _detectionAttachment;
        [SerializeField] private Collider _attachInput;
        private Input.TrailerInput _input;
        private Input.TrailerInputNullRef _inputNullRef;
        public void SetInput(Input.TrailerInput input = null)
        {
            _input = input ?? _inputNullRef;
            _transmission.SetInput(_input);
        }
        private void Awake()
        {
            _inputNullRef = new Input.TrailerInputNullRef();
            _body = GetComponent<Rigidbody>();
        }
        private void Start()
        {
            InitTransmission();
            InitAttachController();
            SetInput();
        }
        private void OnDestroy()
        {
            _controller.EventChangeInput -= SetInput;
        }
        private void InitTransmission()
        {
            _transmission = new Transmission.TrailerTransmission(_body, _data, PrefabAxleCollider, PrefabAxleController);
        }
        private void InitAttachController()
        {
            _controller = new Attach.Core.AttachController(_body,_attachConnectMask, _detectionAttachment, _transformStay);
            _controller.EventChangeInput += SetInput;
        }
        public void InteractedBy(Collider triggerItem)
        {
            if(_attachInput == triggerItem)
                _controller.InteractAttach();
        }
        private void FixedUpdate()
        {
            _transmission.Update();
        }
    }
}