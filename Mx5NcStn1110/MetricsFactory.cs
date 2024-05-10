using Mx5NcStn1110.DotnetSerialPort;
using Mx5NcStn1110.Shm;
using Mx5NcStn1110.Uds;

namespace Mx5NcStn1110;

public static class MetricsFactory
{
    public static IMetrics GetMetrics(bool sharedMemory, string unixDomainSocketName, string serialPortName, int baudRate)
    {
        if (sharedMemory)
        {
            Console.WriteLine("using shm");
            return new Mx5NcShmMetrics();
        }
        
        if (!string.IsNullOrWhiteSpace(unixDomainSocketName))
        {
            Console.WriteLine("using uds");
            return new Mx5NcUdsMetrics(unixDomainSocketName);
        }

        if (!string.IsNullOrWhiteSpace(serialPortName) && baudRate > 0)
        {
            Console.WriteLine("using serial");
            return new Mx5NcDotnetSerialPortMetrics(serialPortName, baudRate);
        }

        Console.WriteLine("using fake");
        return new FakeMetrics();
    }
}