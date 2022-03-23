# PASMBTCP

## Modbus/TCP

**Modbus-TCP** is an Industrial Communications Protocol developed by MODICON in 1979 for their PLC's. 
Since then it has become the most common type of communications that can be found in industrial devices. 

This is a version of the Modbus/TCP protocol written in .NET6.
This version only reads from remote devices and places the data into a local SQLite Database. 
*All methods are Asynchronous.*

### Capabilities:

#### Modbus Specific to the End Device.
These Are The Available Registers In A Typical Modbus Device.
Please Refer To Your Devices Manual For Details About Data Structures And Register Addresses.

| Function Code | Function Name | Address Range |
|----------|----------|----------|
| 1 | Read Coil | 00001 - 09999 |
| 2 | Read Discrete Inputs | 10001 - 19999 |
| 3 | Read Holding Registers | 40001 - 49999 |
| 4 | Read Input Registers | 30001 - 39999 |


#### Typical Addressing by the User.
This is the Addressing you will use when creating a Tag.
These Are The Most Used Data Types
Please Refer To Your Devices Manual For Details About Data Structures You Have Available
**Info** Enter Your Register Address As Show In The Table Below 

| Function Code | Function Name | Address Range | Data Types |
|----------|----------|----------|----------|
| 1 | Read Coil | 000001 - 065536 | Bool |
| 2 | Read Discrete Inputs | 10001 - 165536 | Bool |
| 3 | Read Holding Registers | 40001 - 465536 | Short - Long - Float |
| 4 | Read Input Registers | 30001 - 365536 | Short - Long - Float |

### Example

``` C#
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
await ClientController.CreateClient("Client", "192.168.1.1", 502, 60000, 60000);
await ClientController.UpdateClientAsync("Client", "192.168.1.100", 502, 10000, 20000);
await ClientController.DeleteSingleClientAsync("Client");
await ClientController.DeleteAllClientsAsync();

/// <summary>
/// Create A Tag
/// This Tag Will Be Inserted Into A Table Called *(Client)_Tag* In The App Database.
/// If This Table Does Not Exist It Will Be Created.
/// Demonstrated Here Is Insert New Tag, Update Tag, Delete Tag and Delete All Tags Associated With A Specific Client
/// Input A (String)Name, (String)Register Address, (String)Data Type and What (String)Client It Belongs To
/// </summary>
await DataTagController.CreateTag("Example", "400001", "Float", "Client");
await DataTagController.UpdateTagAsync("Example", "400001", "Float", "Client");
await DataTagController.DeleteSingleTagAsync("Client", "Example");
await DataTagController.DeleteAllTagsAsync("Client");

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

/// <summary>
/// The Database Controller Handles Generic Funcitons
/// Returns All Table Names In The Database
/// </summary>
IEnumerable<string> tables = await DatabaseController.GetAllTables();

/// <summary>
/// Drops A Table
/// </summary>
await DatabaseController.DropTable("Table Name");
```
#### TO DO:
    -[X] Add Delete Single Client
    -[X] Add Delete All Clients
    -[X] Add Delete Single Tag
    -[X] Add Delete All Tags
    -[] Add Drop Client Table
    -[] Add Drop Tag Table
    -[] Add Get All Tables In Database
    -[] Parallel Polling
