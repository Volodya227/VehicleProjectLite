using UnityEngine;
namespace Systems.Vehicle.AudioSystem
{
    [System.Serializable]
    public class AudioSystem
    {
        [SerializeField] private EngineSystem _engineSound;
        private readonly AudioSource _audioSource;
        public AudioSystem(AudioSource audioSource, ContainerData.VehicleEngineState vehicleEngineState, Data.VehicleData vehicleData)
        {
            _audioSource = audioSource;
            _engineSound = new EngineSystem(_audioSource, vehicleEngineState, vehicleData.EngineData.GetSound());
        }
        public void Update()
        {
            _engineSound.Update();
        }
        public void Disable()
        {
            _engineSound.Disable();
        }
    }
    [System.Serializable]
    public class EngineSystem
    {
        private readonly AudioSource _audioSource;
        private readonly Data.EngineDataSound _engineDataSound;
        private readonly ContainerData.VehicleEngineState _vehicleEngineState;
        private bool _active;
        private bool _starter;
        public EngineSystem(AudioSource audioSource, ContainerData.VehicleEngineState vehicleEngineState, Data.EngineDataSound engineDataSound)
        {
            _audioSource = audioSource;
            _vehicleEngineState = vehicleEngineState;
            _engineDataSound = engineDataSound;
            _vehicleEngineState.EventStarterActive += SetStarterActive;
            _vehicleEngineState.EventActive += SetActive;
        }
        public void Disable()
        {
            _vehicleEngineState.EventStarterActive -= SetStarterActive;
            _vehicleEngineState.EventActive -= SetActive;
        }
        public void Update()
        {
            if (_active)
            {
                _audioSource.volume = .25f + .5f * Mathf.Clamp01(((float)_vehicleEngineState.rpm - _engineDataSound.IdleRPM) / (_engineDataSound.LimitRPM - _engineDataSound.IdleRPM)) + _vehicleEngineState.load + _vehicleEngineState.load * .25f;
                _audioSource.pitch = .5f + .5f * Mathf.Clamp01(((float)_vehicleEngineState.rpm - _engineDataSound.IdleRPM) / (_engineDataSound.LimitRPM - _engineDataSound.IdleRPM)) + _vehicleEngineState.load * .1f;
            }
        }
        private void SetStarterActive(bool value)
        {
            if (_active)
                return;
            if (value)
                PlayStart();
            else
                StopStart();
        }
        private void PlayStart()
        {
            _audioSource.Stop();
            _starter = true;
            _audioSource.clip = _engineDataSound.AudioClipStart;
            _audioSource.loop = false;
            _audioSource.Play();
        }
        private void StopStart()
        {
            if (_starter)
                _audioSource.Stop();
            _starter = false;
        }
        private void SetActive(bool value)
        {
            if (value)
            {
                _starter = false;
                _active = true;
                PlayLoop();
            }
            else
            {
                _active = false;
                PlayStop();
            }
        }
        private void PlayStop()
        {
            _audioSource.Stop();
            _audioSource.clip = _engineDataSound.AudioClipStop;
            _audioSource.loop = false;
            _audioSource.Play();
            _audioSource.pitch = 1;
            _audioSource.volume = 1;
        }
        private void PlayLoop()
        {
            _audioSource.Stop();
            _audioSource.clip = _engineDataSound.AudioClipLoop;
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }
}