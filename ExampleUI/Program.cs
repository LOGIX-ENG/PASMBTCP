// See https://aka.ms/new-console-template for more information
using PASMBTCP.Device;
using PASMBTCP.Polling;
using PASMBTCP.Tag;

/* 
    ************************************************************************************************************
    * These Are The Available Registers In A Typical Modbus Device
    * Please Refer To Your Devices Manual For Details About Data Structures And Register Addresses
    * 
    * coil numbers span from 000001 to 065536,
    * discrete input numbers span from 100001 to 165536,
    * input register numbers span from 300001 to 365536,
    * holding register numbers span from 400001 to 465536.
    ************************************************************************************************************
*/
/* 
    ************************************************************************************************************
    * These Are The Most Used Data Types
    * Please Refer To Your Devices Manual For Details About Data Structures You Have Available
    * Available Data Types: 
    * Float - Also Known As A Real
    * Long - This Is A 32bit Word
    * Short - This Is A 16bit Word
    * Bool = This Is A Boolean
    ************************************************************************************************************
*/
/* 
    ************************************************************************************************************
    * Create A Device You Wish To Poll
    * Create A Tag You Wish To Poll In That Device
    * Choose How You Wish To Poll That Device
    ************************************************************************************************************
*/

/// <summary>
/// Create Device You Wish To Poll
/// This Device Is Inserted To The *Client* Table In The App Database
/// Input A (String)Name, (String)IP Address, (INT)Port #, (INT)Connection (INT)Timeout and (INT)Read/Write Timeout
/// </summary>
await Device.CreateClient("Client", "192.168.1.1", 2502, 60000, 60000);

/// <summary>
/// Create A Tag
/// This Tag Will Be Inserted Into A Table Called *(Client)_Tag* In The App Database.
/// If This Table Does Not Exist It Will Be Created.
/// Input A (String)Name, (String)Register Address, (String)Data Type and What (String)Client It Belongs To
/// </summary>
await DataTagCreator.CreateTag("Example", "400001", "Float", "Client");

/// <summary>
/// This Option Polls Every Client And Every Tag
/// </summary>
await PollingEngine.PollAllDevicesAsync();

/// <summary>
/// Name The Client You Wish To Poll
/// Must Be Same As (String)Client You Created
/// This Option Polls All Tags Associated With The Client
/// </summary>
await PollingEngine.PollSingleDeviceAsync("Client");

/// <summary>
/// Name The Client You Wish To Poll
/// Name The Tag You Wish To Poll
/// Must Be Same As (String)Client And (String)Tag You Created
/// </summary>
await PollingEngine.PollSingleTagAsync("Client", "Example");