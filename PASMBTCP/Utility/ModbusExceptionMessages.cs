namespace PASMBTCP.Utility
{
    internal class ModbusExceptionMessages
    {
        /// <summary>
        /// Modbus Exception Messages
        /// </summary>
        public const string IllegalFunction = "Function code received in the query is not recognized or allowed by slave.";
        public const string IllegalDataAddress = "Data address of some or all the required entities are not allowed or do not exist in slave.";
        public const string IllegalDataValue = "Value is not accepted by slave.";
        public const string SlaveDeviceFailure = "Unrecoverable error occurred while slave was attempting to perform requested action.";
        public const string Acknowledge = "Slave has accepted request and is processing it, but a long duration of time is required. This response is returned to prevent a timeout error from occurring in the master. Master can next issue a Poll Program Complete message to determine whether processing is completed.";
        public const string SlaveDeviceBusy = "Slave is engaged in processing a long-duration command. Master should retry later.";
        public const string NegativeAcknowledge = "Slave cannot perform the programming functions. Master should request diagnostic or error information from slave.";
        public const string MemoryParityError = " 	Slave detected a parity error in memory. Master can retry the request, but service may be required on the slave device.";
        public const string GatewayPathUnavailable = "Specialized for Modbus gateways. Indicates a misconfigured gateway.";
        public const string GatewayTargetDeviceFailedToRespond = "Specialized for Modbus gateways. Sent when slave fails to respond.";
        public const string Unknown = "Unknown Error Code.";
    }
}
