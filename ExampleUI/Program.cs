// See https://aka.ms/new-console-template for more information
using PASMBTCP.Device;
using PASMBTCP.Polling;

//coil numbers span from 000001 to 065536,
//discrete input numbers span from 100001 to 165536,
//input register numbers span from 300001 to 365536,
//holding register numbers span from 400001 to 465536.

//await Device.CreateClient("Brillhart", "107.84.202.5", 2502, 60000, 60000);
//ClientDevice.IpAddress = "107.84.202.5";
//ClientDevice.Port = 2502;
//ClientDevice.SocketTimeout = 60000;
//ClientDevice.ReadWriteTimeout = 60000;
//await DataTagCreator.CreateTag("Example2", "400003", "Float", "Brillhart");
await PollingEngine.PollAllAsync();