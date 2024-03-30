using Mx5NcStn1110;

var portName = args[0];

var stn = new Stn1110(portName, 115200);

stn.SetupConnection();

stn.AddFilter(0x430);

stn.StartMonitoring();
var msg = stn.ReadCanMessage();
stn.StopMonitoring();

Console.WriteLine(msg);