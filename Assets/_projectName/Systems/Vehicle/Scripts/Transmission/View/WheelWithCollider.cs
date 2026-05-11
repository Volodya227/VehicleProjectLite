using UnityEngine;
namespace Systems.Vehicle.Transmission.View
{
    public class WheelWithCollider : Wheel
    {
        private WheelHit _hit;
        private readonly WheelCollider _collider;
        private bool _grounded = false;
        private float _torque;
        public override float Torque => _torque;
        public override bool Grounded => _collider.isGrounded;
        public override Transform Transform => _collider.transform;
        private float _minSidewaysFriction;
        private float _loseSidewaysFriction;

        private float _sidewaysExtremumValue;
        private float _sidewaysAsymptoteValue;
        public WheelWithCollider(WheelCollider collider, Data.WheelData data, bool right)
        {
            _collider = collider;
            _body = _collider.attachedRigidbody;
            SetNewWheel(data, right);
        }
        public override void SetNewWheel(Data.WheelData data, bool right)
        {
            SetData(data);
            GameObject mesh = Object.Instantiate(data.PrefabMesh);
            mesh.transform.SetParent(_collider.transform);
            mesh.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            mesh.transform.localScale = new Vector3((right ? 1 : -1) * data.Width, data.Radius * 2, data.Radius * 2);
            SetMesh(mesh);
        }
        private void SetData(Data.WheelData data)
        {
            _collider.radius = data.Radius;
            _collider.forwardFriction = new()
            {
                stiffness = data.ForwardStiffness
            };
            _collider.sidewaysFriction = new()
            {
                stiffness = data.SidewaysStiffness
            };
            _minSidewaysFriction = data.MinSidewaysFriction;
            _loseSidewaysFriction = 1 - _minSidewaysFriction;
            SetFriction(data.GetFrictionForSurface(Data.SurfaceType.Asphalt));
        }
        private void SetFriction(Data.WheelFriction data)
        {
            _collider.forwardFriction = new()
            {
                extremumValue = data.forwardExtremumValue,
                extremumSlip = data.forwardExtremumSlip,
                asymptoteValue = data.forwardAsymptoteValue,
                asymptoteSlip = data.forwardAsymptoteSlip,
                stiffness = _collider.forwardFriction.stiffness
            };
            _collider.sidewaysFriction = new()
            {
                extremumValue = data.sidewaysExtremumValue,
                extremumSlip = data.sidewaysExtremumSlip,
                asymptoteValue = data.sidewaysAsymptoteValue,
                asymptoteSlip = data.sidewaysAsymptoteSlip,
                stiffness = _collider.sidewaysFriction.stiffness
            };
            _sidewaysExtremumValue = data.sidewaysExtremumValue;
            _sidewaysAsymptoteValue = data.sidewaysAsymptoteValue;
        }
        private void SetGround()
        {
            _grounded = _collider.GetGroundHit(out _hit);
        }
        public override void SteerAngle(float angle) {
            if (_collider.steerAngle != angle) { _collider.steerAngle = angle; }
        }
        public override float Rpm => _collider.rpm;
        public override void SetTorque(float torque) {
            if (torque != 0) _torque = torque;
            _collider.motorTorque = _torque;
        }
        private bool CheckNormalGround()
        {
            if (_grounded) return _hit.normal.y < .9998f;
            return false;
        }
        public override void BrakeTorque(float torque) {
            _collider.brakeTorque = torque;
            _torque = 0;
            if (_collider.brakeTorque == 0)
            {
                if (Mathf.Abs(_collider.rpm) < 5) {
                    if (CheckNormalGround())
                        _torque = .01f;
                }
            }
        }
        public override void Update() {
            SetGround();
            ApplyLocalPositionToVisuals();
        }
        public override float Travel()
        {
            if (_grounded)
            {
                if (_collider.suspensionDistance == 0) return 1;
                return (-_collider.transform.InverseTransformPoint(_hit.point).y - _collider.radius) / _collider.suspensionDistance;
            }
            return 1;
        }
        private void ApplyLocalPositionToVisuals()
        {
            if (_meshWheel == null) return;
            _collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
            _meshWheel.transform.SetPositionAndRotation(position, rotation);
        }
        public override void ApplyTraction()
        {
            if (_grounded)
            {
                float targetTraction = Mathf.Clamp01(1 - Mathf.Abs(_hit.forwardSlip));
                _traction = Mathf.Lerp(_traction, targetTraction, Time.fixedDeltaTime * 5f);
            }
            else _traction = 0;
        }
        public override void UpdateFriction()
        {
            //UpdateLocalDrag();
            //_minSidewaysFriction + _loseSidewaysFriction = 1
            WheelFrictionCurve value = _collider.sidewaysFriction;
            value.extremumValue = (_minSidewaysFriction + _loseSidewaysFriction * Mathf.Clamp01(Traction)) * _sidewaysExtremumValue;
            value.asymptoteValue = (_minSidewaysFriction + _loseSidewaysFriction * Mathf.Clamp01(Traction)) * _sidewaysAsymptoteValue;
            _collider.sidewaysFriction = value;
        }
        private void UpdateLocalDrag()
        {
            if (_grounded)
            {
                Vector3 v = _body.GetPointVelocity(_collider.transform.position);
                Vector3 drag = 140 * v.magnitude * -v;
                _body.AddForceAtPosition(drag, _collider.transform.position);
            }
        }
    }
}