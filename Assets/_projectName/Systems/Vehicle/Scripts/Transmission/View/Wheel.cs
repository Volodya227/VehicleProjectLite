using UnityEngine;
namespace Systems.Vehicle.Transmission.View
{
    public abstract class Wheel
    {
        protected Rigidbody _body;
        protected float _maxSpeedDifference = 2;
        protected float _traction = 1;
        public float Traction => _traction;
        public virtual bool Grounded => false;
        public virtual float Torque => 1;
        public virtual Transform Transform => null;
        protected GameObject _meshWheel = null;
        public virtual void SetNewWheel(Data.WheelData data, bool right) { }
        protected void SetMesh(GameObject meshWheel)
        {
            if (_meshWheel != null)
            {
                Object.Destroy(_meshWheel);
            }
            _meshWheel = meshWheel;
        }
        public virtual void SteerAngle(float angle) { }
        public virtual float Rpm => 0;
        public virtual void SetTorque(float torque) { }
        public virtual void BrakeTorque(float torque) { }
        public virtual void Update() { }

        public virtual float Travel() { return 0; }
        public virtual void UpdateFriction() { }
        public virtual void ApplyTraction() { }
    }
}