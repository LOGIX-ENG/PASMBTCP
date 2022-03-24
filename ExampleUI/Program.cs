// See https://aka.ms/new-console-template for more information
using PASMBTCP.Device;
using PASMBTCP.Polling;
using PASMBTCP.SQLite;
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
/// This Client Is Inserted To The Client Table In The App Database
/// Demonstrated Here Is Insert New Client, Update Client, Delete Client and Delete All Clients
/// Input A (String)Name, (String)IP Address, (INT)Port #, (INT)Connection (INT)Timeout and (INT)Read/Write Timeout
/// </summary>
await ClientController.CreateClient("Device", "192.168.1.100", 502, 60000, 60000);
await ClientController.DeleteSingleClientAsync("Device");
await ClientController.DeleteAllClientsAsync();

/// <summary>
/// Create A Tag
/// This Tag Will Be Inserted Into A Table Called *(Client)_Tag* In The App Database.
/// If This Table Does Not Exist It Will Be Created.
/// Demonstrated Here Is Insert New Tag, Update Tag, Delete Tag and Delete All Tags Associated With A Specific Client
/// Input A (String)Name, (String)Register Address, (String)Data Type and What (String)Client It Belongs To
/// </summary>
await DataTagController.CreateTag("Example", "400001", "Float", "Device");
await DataTagController.DeleteSingleTagAsync("Device", "Example");
await DataTagController.DeleteAllTagsAsync("Device");

/// <summary>
/// This Option Polls Every Client And Every Tag
/// </summary>
await PollingEngine.PollAllDevicesAsync();

/// <summary>
/// Name The Client You Wish To Poll
/// Must Be Same As (String)Client You Created
/// This Option Polls All Tags Associated With The Client
/// </summary>
await PollingEngine.PollSingleDeviceAsync("Device");

/// <summary>
/// Name The Client You Wish To Poll
/// Name The Tag You Wish To Poll
/// Must Be Same As (String)Client And (String)Tag You Created
/// </summary>
await PollingEngine.PollSingleTagAsync("Device", "Example");

/// <summary>
/// The Database Controller Handles Generic Funcitons
/// Returns All Table Names In The Database
/// </summary>
IEnumerable<string> tables = await DatabaseController.GetAllTables();

foreach (string tableName in tables)
{
    Console.WriteLine(tableName);
}

/// <summary>
/// Drops A Table
/// </ summary >
await DatabaseController.DropTable("TableName");
