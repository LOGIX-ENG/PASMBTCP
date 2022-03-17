# PASMBTCP

## Modbus/TCP

Modbus-TCP is an Industrial Communications Protocol developed by MODICON in 1979 for their PLC's. 
Since then it has become the most common type of communications that can be found in industrial devices. 

### Capabilities:

#### Modbus Specific to the End Device.

| Function Code | Function Name | Address Range |
|----------|----------|----------|
| 1 | Read Coil | 00001 - 09999 |
| 2 | Read Discrete Inputs | 10001 - 19999 |
| 3 | Read Holding Registers | 40001 - 49999 |
| 4 | Read Input Registers | 30001 - 39999 |


#### Typical Addressing by the User.

| Function Code | Function Name | Address Range |
|----------|----------|----------| 
| 1 | Read Coil | 000001 - 065536 |
| 2 | Read Discrete Inputs | 10001 - 165536 |
| 3 | Read Holding Registers | 40001 - 465536 |
| 4 | Read Input Registers | 30001 - 365536 |

