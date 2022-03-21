// See https://aka.ms/new-console-template for more information
using PASMBTCP.Device;
using PASMBTCP.Tag;

//coil numbers span from 000001 to 065536,
//discrete input numbers span from 100001 to 165536,
//input register numbers span from 300001 to 365536,
//holding register numbers span from 400001 to 465536.

ClientDevice.IpAddress = "127.0.0.1";
ClientDevice.Port = 502;
ClientDevice.SocketTimeout = 60000;
ClientDevice.ReadWriteTimeout = 60000;
await ClientDevice.InsertTagIntoDB("Example", "400001", "Float");
await ClientDevice.PollAsync();