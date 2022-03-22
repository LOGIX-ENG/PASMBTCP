# PASMBTCP

## Modbus/TCP

**Modbus-TCP** is an Industrial Communications Protocol developed by MODICON in 1979 for their PLC's. 
Since then it has become the most common type of communications that can be found in industrial devices. 

This is a version of the Modbus/TCP protocol written in .NET6.
This version only reads from remote devices and places the data into a local SQLite Database. 
*All methods are Asynchronous.*

### Capabilities:

#### Modbus Specific to the End Device.
These Are The Available Registers In A Typical Modbus Device
Please Refer To Your Devices Manual For Details About Data Structures And Register Addresses

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
/// You Can Choose To Poll All Devices You Created And All The Tags For Each Device
/// Or You Can Poll A Single Device (All Tags)
/// Or You Can Poll A Single Tag In A Device
/// </summary>
await PollingEngine.PollAllDevicesAsync();

/// <summary>
/// Name The Client You Wish To Poll
/// Must Be Same As (String)Client You Created
/// </summary>
await PollingEngine.PollSingleDeviceAsync("Client");

/// <summary>
/// Name The Client
/// Name The Tag
/// You Wish To Poll
/// Must Be Same As (String)Client And (String)Tag You Created
/// </summary>
await PollingEngine.PollSingleTagAsync("Client", "Example");
```
#### TO DO:
    -[] Add Delete Single Client
    -[] Add Delete All Clients
    -[] Add Delete Single Tag
    -[] Add Delete All Tags
    -[] Add Drop Client Table
    -[] Add Drop Tag Table
    -[] Add Get All Tables In Database
    -[] Parallel Polling
