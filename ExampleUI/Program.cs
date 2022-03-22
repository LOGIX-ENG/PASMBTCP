// See https://aka.ms/new-console-template for more information
using PASMBTCP.Device;
using PASMBTCP.Polling;
using PASMBTCP.Tag;

//coil numbers span from 000001 to 065536,
//discrete input numbers span from 100001 to 165536,
//input register numbers span from 300001 to 365536,
//holding register numbers span from 400001 to 465536.

await Device.CreateClient("Client", "192.168.1.1", 2502, 60000, 60000);
await DataTagCreator.CreateTag("Example", "400001", "Float", "Client");
await PollingEngine.PollAllAsync();
