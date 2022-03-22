namespace PASMBTCP.Utility
{
    internal class ModbusUtility
    {
        //Constant Length Field For Modbus MBAP Header
        public const short LengthField = 6;

        //Constant For MBTCP = 0
        public const short ProtocolId = 0;

        //Constant For Reading A Float Value
        //Float = 4 bytes
        public const short RealQuantity = 2;

        //Constant For Reading A Word
        //Word = 2 bytes
        public const short ShortQuantity = 1;

        //Constant For Reading A Long
        //Long = 4 bytes
        public const short LongQuantity = 2;

        //Index
        public const int Index = 0;

        //Modbus Buffer Max Size
        public const int BufferSize = 256;

        // Constants For Writing To A Coil
        public const ushort CoilOn = 0xFF00;
        public const ushort CoilOff = 0x0000;

        // Modbus Exception Code Offset
        public const byte ExceptionCodeOffset = 128;

        /// <summary>
        /// Used To Track Data Type
        /// Is Placed In The Transaction ID Section Of The MBAP
        /// Used To Convert Data Into Specific Type
        /// </summary>
        public const short BoolCode = 1;
        public const short ShortCode = 2;
        public const short FloatCode = 3;
        public const short LongCode = 4;

    }
}
