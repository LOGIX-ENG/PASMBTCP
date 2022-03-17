# PASMBTCP

## Modbus/TCP

Modbus-TCP is an Industrial Communications Protocol developed by MODICON in 1979 for their PLC's. 
Since then it has become the most common type of communications that can be found in industrial devices. 

This is a version of the Modbus/TCP protocol written in .NET6.
This version only reads from remote devices and place the data into a local SQLite Database. 
All methods are Asynchronous.

### Capabilities:

#### Modbus Specific to the End Device.

| Function Code | Function Name | Address Range |
|----------|----------|----------|
| 1 | Read Coil | 00001 - 09999 |
| 2 | Read Discrete Inputs | 10001 - 19999 |
| 3 | Read Holding Registers | 40001 - 49999 |
| 4 | Read Input Registers | 30001 - 39999 |


#### Typical Addressing by the User.
This is the Addressing you will use when creating a Tag.

| Function Code | Function Name | Address Range | Data Types |
|----------|----------|----------|----------|
| 1 | Read Coil | 000001 - 065536 | Boolean |
| 2 | Read Discrete Inputs | 10001 - 165536 | Boolean |
| 3 | Read Holding Registers | 40001 - 465536 | Short - Long - Float |
| 4 | Read Input Registers | 30001 - 365536 | Short - Long - Float |

### Example

``` C#
ClientDevice.IpAddress = "127.0.0.1";
ClientDevice.Port = 502;
ClientDevice.SocketTimeout = 60000;
ClientDevice.ReadWriteTimeout = 60000;
// Inserting a Tag into the Database with a Name, Address, Data Type.
// All inputs are Strings.
await ClientDevice.InsertTagIntoDB("Example", "400001", "Float");
await ClientDevice.PollAsync();
```

