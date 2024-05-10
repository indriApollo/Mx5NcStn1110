using Mx5NcStn1110.DotnetSerialPort;
using Mx5NcStn1110.Uds;

namespace Mx5NcStn1110;

public static class MetricsFactory
{
    public static IMetrics GetMetrics(string unixDomainSocketName, string serialPortName, int baudRate)
    {
        if (!string.IsNullOrWhiteSpace(unixDomainSocketName))
            return new Mx5NcUdsMetrics(unixDomainSocketName);

        if (!string.IsNullOrWhiteSpace(serialPortName) && baudRate > 0)
            return new Mx5NcDotnetSerialPortMetrics(serialPortName, baudRate);

        return new FakeMetrics();
    }
}