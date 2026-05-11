using UnityEngine;
namespace Systems.Vehicle.Transmission.Model.GearBoxControl
{
    public class GearBoxAutomaticControl : GearBoxControlBase
    {
        public event System.Action<GearBoxState> EventChangeState;
        public event System.Action<int> EventChangeGearDelta;
        private readonly ContainerData.VehicleContainerData _vehicleContainerData;
        private readonly Vector2Int _positionXLimit;
        private readonly Vector2Int _positionZLimit;
        private readonly Vector2Int _modeR;
        private readonly Vector2Int _modeN;
        private readonly Vector2Int _modeD;
        private readonly Vector2Int _modeM;
        private int _newX;
        private int _newZ;
        public GearBoxState CurrentMode { get; private set; }
        public GearBoxAutomaticControl(Data.GearBoxAutomaticControlData config, ContainerData.VehicleContainerData vehicleContainerData) {
            _modeR = config.ModeR;
            _modeN = config.ModeN;
            _modeD = config.ModeD;
            _modeM = config.ModeM;
            _positionXLimit = config.XLimit;
            _positionZLimit = config.ZLimit;
            _vehicleContainerData = vehicleContainerData;
            NewPosition(0, 0);
        }
        protected override void NewPosition(int x, int z)
        {
            _newX = Mathf.Clamp(x, _positionXLimit[0], _positionXLimit[1]);
            _newZ = Mathf.Clamp(z, _positionZLimit[0], _positionZLimit[1]);
            GearBoxState newChoice = GetState();
            if (newChoice == CurrentMode) return;
            _positionX = _newX;
            _positionZ = _newZ;
            CurrentMode = newChoice;
            EventChangeState?.Invoke(CurrentMode);
            _vehicleContainerData.SetPositionGear(_positionX, _positionZ);
        }
        public override void SetInput(Input.VehicleInput input)
        {
            base.SetInput(input);
            DisableEvent();
            _input = input;
            EnableEvent();
        }
        private void EnableEvent()
        {
            if (_input == null) return;
            _input.EventChangeSequentialGear += ChangeGear;
        }
        private void DisableEvent()
        {
            if (_input == null) return;
            _input.EventChangeSequentialGear -= ChangeGear;
        }
        private GearBoxState GetState()
        {
            Vector2Int pos = new(_newX, _newZ);
            if (pos == _modeR) return GearBoxState.R;
            if (pos == _modeN) return GearBoxState.N;
            if (pos == _modeD) return GearBoxState.D;
            if (pos == _modeM) return GearBoxState.M;
            return CurrentMode;
        }
        private void ChangeGear(int delta = 0)
        {
            if (CurrentMode == GearBoxState.M)
            {
                EventChangeGearDelta?.Invoke(delta);
            }
        }
    }
}