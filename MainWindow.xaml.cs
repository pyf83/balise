using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BeaconAudioRC
{
    public partial class MainWindow : Window
    {
        private readonly BluetoothManager _btManager = new BluetoothManager();
        private bool isPlaying = false;
        private bool isConnected = false;

        public MainWindow()
        {
            InitializeComponent();
            _btManager.ConnectionStatusChanged += OnBluetoothStatusChanged;
        }

        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                BtnConnect.Content = "Recherche en cours...";
                var devices = await _btManager.DiscoverBeaconDevicesAsync();

                if (devices.Count > 0)
                {
                    bool success = await _btManager.ConnectDeviceAsync(devices[0].Id);
                    if (!success)
                    {
                        MessageBox.Show("Impossible de se connecter à l'enceinte Beacon sélectionnée.", "Erreur Bluetooth", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Aucune enceinte Beacon Bluetooth trouvée à proximité.", "Recherche Bluetooth", MessageBoxButton.OK, MessageBoxImage.Information);
                    BtnConnect.Content = "Connecter via Bluetooth";
                }
            }
            else
            {
                _btManager.Disconnect();
            }
        }

        private void OnBluetoothStatusChanged(bool connected)
        {
            Dispatcher.Invoke(() =>
            {
                isConnected = connected;
                if (isConnected)
                {
                    TxtStatus.Text = "Connecté";
                    TxtStatus.Foreground = (SolidColorBrush)FindResource("TextPrimary");
                    StatusDot.Fill = new SolidColorBrush(Color.FromRgb(48, 209, 88));
                    BtnConnect.Content = "Déconnecter l'enceinte";
                    BtnConnect.Background = new SolidColorBrush(Color.FromRgb(255, 69, 58));
                }
                else
                {
                    TxtStatus.Text = "Déconnecté";
                    TxtStatus.Foreground = (SolidColorBrush)FindResource("TextSecondary");
                    StatusDot.Fill = new SolidColorBrush(Color.FromRgb(255, 59, 48));
                    BtnConnect.Content = "Connecter via Bluetooth";
                    BtnConnect.Background = new SolidColorBrush(Color.FromRgb(0, 120, 212));
                    
                    if (isPlaying)
                    {
                        isPlaying = false;
                        BtnPlayPause.Content = "▶";
                    }
                }
            });
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;
            isPlaying = !isPlaying;
            BtnPlayPause.Content = isPlaying ? "⏸" : "▶";
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void SldVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int vol = (int)Math.Round(e.NewValue);
            if (TxtVolume != null)
            {
                TxtVolume.Text = $"{vol}%";
            }
            if (isConnected)
            {
                await _btManager.SetVolumeAsync(vol);
            }
        }

        private async void EqPreset_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag != null && isConnected)
            {
                int presetIndex = int.Parse(rb.Tag.ToString() ?? "0");
                await _btManager.SetPresetEQAsync(presetIndex);
            }
        }
    }
}