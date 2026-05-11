using UnityEngine;
namespace Systems.Vehicle.Trailer.Attach.Core
{
    public class AttachController
    {
        public event System.Action<Input.TrailerInput> EventChangeInput;
        private readonly LayerMask _attachConnectMask;
        private readonly Transform _transformStay;
        private CharacterJoint _joint;
        private Attachment _connectedPoint;
        private readonly Transform _detectionAttachment;
        private readonly Rigidbody _body;
        public AttachController(Rigidbody body, LayerMask attachConnectMask, Transform detectionAttachment, Transform transformStay)
        {
            _attachConnectMask = attachConnectMask;
            _detectionAttachment = detectionAttachment;
            _body = body;
            _transformStay = transformStay;
            _joint = null;
        }
        private Attachment GetConnectPoint()
        {
            Ray ray = new(_detectionAttachment.position, _detectionAttachment.forward);
            //Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red, 1f);
            if (Physics.Raycast(ray, out RaycastHit hit, 2, _attachConnectMask))
            {
                //Debug.Log(hit.collider.name);
                return hit.collider.GetComponent<Attachment>();
            }
            return null;
        }
        public void InteractAttach()
        {
            if (_joint == null)
            {
                _connectedPoint = GetConnectPoint();
                if (_connectedPoint == null) return;
                _joint = _body.gameObject.AddComponent<CharacterJoint>();

                _joint.anchor = _detectionAttachment.localPosition + _detectionAttachment.forward * .4f;
                _joint.connectedBody = _connectedPoint.Data.Body;
                _joint.autoConfigureConnectedAnchor = false;
                _joint.connectedAnchor = _connectedPoint.Data.PositionConnectedBody;
                _joint.swing1Limit = new SoftJointLimit { limit = 100f };
                _joint.swing2Limit = new SoftJointLimit { limit = 30f };
                _joint.lowTwistLimit = new SoftJointLimit { limit = -20f };
                _joint.highTwistLimit = new SoftJointLimit { limit = 20f };
                _joint.enableCollision = true;
                _joint.connectedMassScale = 1;
                _transformStay.gameObject.SetActive(false);
                SetInput();
                _connectedPoint.EventInputChange += SetInput;
            }
            else
            {
                Object.Destroy(_joint);
                _joint = null;
                _connectedPoint.EventInputChange -= SetInput;
                _connectedPoint = null;
                _transformStay.gameObject.SetActive(true);
                EventChangeInputActivate();
            }
        }
        private void SetInput()
        {
            EventChangeInputActivate(_connectedPoint.GetInput);
        }
        private void EventChangeInputActivate(Input.TrailerInput input = null) {
            EventChangeInput?.Invoke(input);
        }
    }
}