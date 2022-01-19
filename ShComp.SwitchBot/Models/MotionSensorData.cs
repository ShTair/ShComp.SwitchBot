namespace ShComp.SwitchBot.Models;

public class MotionSensorData
{
    public ulong Address { get; set; }

    // Has it been scope tested 1: Tested 0: Not tested
    public bool IsScopeTested { get; set; }

    // PIR State 0: No one moves 1: Someone is moving
    public bool SomeoneIsMoving { get; set; }

    public int BatteryCapacity { get; set; }

    // Since the last trigger PIR time
    public TimeSpan SinceLastTriggered { get; set; }

    // LED state 0:disable 1:enalbe
    public bool LedIsEnabled { get; set; }

    // IOT state 0:disable 1:enable

    public bool IoTIsEnabled { get; set; }

    // Sensing distance 00:Long 01:Middle 10:Short 11:Reserve
    public SensingDistance SensingDistance { get; set; }

    // Light intensity 00:Rserve 01:dark 10:bright 11:Reserve
    public LightIntensity LightIntensity { get; set; }
}

public enum SensingDistance
{
    Long = 0b00,
    Middle = 0b01,
    Short = 0b10,
}

public enum LightIntensity
{
    Dark = 0b01,
    Bright = 0b10,
}
