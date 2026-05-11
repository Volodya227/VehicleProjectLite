namespace Systems.Vehicle.Transmission.Model.GearBoxControl
{
    public enum GearBoxState
    {
        R, N, D, M
    }
    public abstract class GearBoxControlBase
    {
        public event System.Action<int> EventTrySetGear;
        protected int _positionX;
        protected int _positionZ;

        protected Input.VehicleInput _input;
        public virtual void SetInput(Input.VehicleInput input)
        {
            DisableEvent();
            _input = input;
            EnableEvent();
        }
        private void EnableEvent()
        {
            if (_input == null) return;
            _input.EventChangeShiftXGearDelta += ChangeXDelta;
            _input.EventChangeShiftZGearDelta += ChangeZDelta;
            _input.EventChangeShiftXZGear += ChangeXZ;
            _input.EventChangeGear += ActivateEventTrySetGear;
        }
        private void DisableEvent()
        {
            if (_input == null) return;
            _input.EventChangeShiftXGearDelta -= ChangeXDelta;
            _input.EventChangeShiftZGearDelta -= ChangeZDelta;
            _input.EventChangeShiftXZGear -= ChangeXZ;
            _input.EventChangeGear -= ActivateEventTrySetGear;
        }
        protected virtual void NewPosition(int x, int z) { }
        private void ChangeXZ(int x, int z)
        {
            NewPosition(x, z);
        }
        private void ChangeXDelta(int value)
        {
            NewPosition(_positionX + value, _positionZ);
        }
        private void ChangeZDelta(int value)
        {
            NewPosition(_positionX, _positionZ + value);
        }
        protected void ActivateEventTrySetGear(int gear)
        {
            EventTrySetGear?.Invoke(gear);
        }
        public virtual void UpdatePosition() { }
    }
}