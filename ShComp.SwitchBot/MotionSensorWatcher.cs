using ShComp.SwitchBot.Models;
using System.Buffers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;

namespace ShComp.SwitchBot;

public sealed class MotionSensorWatcher : IDisposable
{
    private readonly BluetoothLEAdvertisementWatcher _watcher;

    public bool IsDisposed { get; private set; }

    public event Action<MotionSensorData>? DataReceived;

    public MotionSensorWatcher()
    {
        _watcher = new BluetoothLEAdvertisementWatcher();
        _watcher.Received += Watcher_Received;

        using var dataWriter = new DataWriter();
        dataWriter.WriteBytes(new byte[] { 0x3d, 0xfd, (byte)'s' });

        _watcher.ScanningMode = BluetoothLEScanningMode.Active;
        _watcher.AdvertisementFilter.Advertisement.DataSections.Add(new BluetoothLEAdvertisementDataSection(0x16, dataWriter.DetachBuffer()));

        _watcher.Start();
    }

    private void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
    {
        var section = args.Advertisement.GetSectionsByType(0x16)[0];

        var buffer = ArrayPool<byte>.Shared.Rent(8);

        section.Data.CopyTo(buffer);

        var data = new MotionSensorData
        {
            IsScopeTested = IsOn(buffer[3], 0b10000000),
            SomeoneIsMoving = IsOn(buffer[3], 0b01000000),

            BatteryCapacity = buffer[4] & 0b01111111,

            SinceLastTriggered = TimeSpan.FromSeconds(((buffer[7] >> 7) << 16) + (buffer[5] << 8) + buffer[6]),

            LedIsEnabled = IsOn(buffer[7], 0b01000000),
            IoTIsEnabled = IsOn(buffer[7], 0b00100000),
            SensingDistance = (SensingDistance)((buffer[7] & 0b1100) >> 2),
            LightIntensity = (LightIntensity)(buffer[7] & 0b11),
        };

        ArrayPool<byte>.Shared.Return(buffer);

        try { DataReceived?.Invoke(data); }
        catch { }
    }

    private static bool IsOn(byte buffer, byte flag) => (buffer & flag) != 0;

    public void Dispose()
    {
        if (IsDisposed) return;
        IsDisposed = true;

        _watcher.Stop();
    }
}
