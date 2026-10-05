using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BeaconAudioRC
{
    public class BluetoothManager
    {
        private BluetoothDevice? _bluetoothDevice;
        private GattDeviceService? _audioControlService;
        private GattCharacteristic? _volumeCharacteristic;
        private GattCharacteristic? _eqCharacteristic;

        public event Action<bool>? ConnectionStatusChanged;
        public event Action<string>? LogMessage;

        // UUIDs personnalisés / standard pour Beacon Audio Control
        private static readonly Guid SERVICE_UUID = new Guid("0000180A-0000-1000-8000-00805F9B34FB");
        private static readonly Guid VOLUME_CHAR_UUID = new Guid("00002A06-0000-1000-8000-00805F9B34FB");
        private static readonly Guid EQ_CHAR_UUID = new Guid("00002A07-0000-1000-8000-00805F9B34FB");

        public async Task<List<DeviceInformation>> DiscoverBeaconDevicesAsync()
        {
            LogMessage?.Invoke("Recherche des appareils Bluetooth Beacon Audio...");
            
            // Filtre A2DP / Bluetooth LE pour appareils audio
            string selector = BluetoothDevice.GetDeviceSelectorFromPairingState(false);
            var devices = await DeviceInformation.FindAllAsync(selector);
            
            var beaconDevices = new List<DeviceInformation>();
            foreach (var dev in devices)
            {
                if (dev.Name.Contains("Beacon", StringComparison.OrdinalIgnoreCase) ||
                    dev.Name.Contains("Speaker", StringComparison.OrdinalIgnoreCase))
                {
                    beaconDevices.Add(dev);
                }
            }
            return beaconDevices;
        }

        public async Task<bool> ConnectDeviceAsync(string deviceId)
        {
            try
            {
                LogMessage?.Invoke($"Connexion à l'appareil ID: {deviceId}...");
                _bluetoothDevice = await BluetoothDevice.FromIdAsync(deviceId);

                if (_bluetoothDevice == null)
                {
                    LogMessage?.Invoke("Impossible d'accéder au périphérique Bluetooth.");
                    ConnectionStatusChanged?.Invoke(false);
                    return false;
                }

                _bluetoothDevice.ConnectionStatusChanged += OnDeviceConnectionStatusChanged;

                var gattServices = await _bluetoothDevice.GetGattServicesAsync();
                if (gattServices.Status == GattCommunicationStatus.Success)
                {
                    LogMessage?.Invoke("Services GATT récupérés avec succès.");
                    ConnectionStatusChanged?.Invoke(true);
                    return true;
                }

                ConnectionStatusChanged?.Invoke(true);
                return true;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Erreur de connexion Bluetooth: {ex.Message}");
                ConnectionStatusChanged?.Invoke(false);
                return false;
            }
        }

        private void OnDeviceConnectionStatusChanged(BluetoothDevice sender, object args)
        {
            bool isConnected = sender.ConnectionStatus == BluetoothConnectionStatus.Connected;
            LogMessage?.Invoke($"Statut Bluetooth modifié: {(isConnected ? "Connecté" : "Déconnecté")}");
            ConnectionStatusChanged?.Invoke(isConnected);
        }

        public async Task SetVolumeAsync(int volumePercent)
        {
            if (_bluetoothDevice == null || _volumeCharacteristic == null) return;

            byte volByte = (byte)Math.Clamp(volumePercent, 0, 100);
            var writer = new DataWriter();
            writer.WriteByte(volByte);

            await _volumeCharacteristic.WriteValueAsync(writer.DetachBuffer());
            LogMessage?.Invoke($"Volume envoyé via Bluetooth: {volumePercent}%");
        }

        public async Task SetPresetEQAsync(int presetIndex)
        {
            if (_bluetoothDevice == null || _eqCharacteristic == null) return;

            var writer = new DataWriter();
            writer.WriteByte((byte)presetIndex);

            await _eqCharacteristic.WriteValueAsync(writer.DetachBuffer());
            LogMessage?.Invoke($"Preset Égaliseur appliqué: Mode #{presetIndex}");
        }

        public void Disconnect()
        {
            if (_bluetoothDevice != null)
            {
                _bluetoothDevice.ConnectionStatusChanged -= OnDeviceConnectionStatusChanged;
                _bluetoothDevice.Dispose();
                _bluetoothDevice = null;
            }
            ConnectionStatusChanged?.Invoke(false);
            LogMessage?.Invoke("Déconnecté de l'appareil Beacon Audio.");
        }
    }
}