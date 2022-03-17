using PASMBTCP.Events;
using System.Globalization;
using System.Net;

namespace PASMBTCP.Tag
{
    public class Converter<T> where T : DataTag
    {
        /// <summary>
        /// Private Variables
        /// </summary>
        private static readonly int _startIndex = 9;
        private static DataTag dataTag = Activator.CreateInstance<T>();
        private static readonly CultureInfo cultureInfo = new("en-US", false);
        private static GeneralExceptionEventArgs _args = new();
        public static event EventHandler<GeneralExceptionEventArgs>? RaiseGeneralExceptionEvent;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public Converter()
        {
        }

        /// <summary>
        /// Formats Date Time With Culture Info
        /// </summary>
        /// <returns>Date Time Formated String</returns>
        private static string GetDateTime()
        {
            DateTime dateTime = DateTime.Now;
            CultureInfo cultureInfo = new("en-US");
            string formatspecifier = "dd/MMMM/yyyy, hh:mm:ss tt";
            return dateTime.ToString(formatspecifier, cultureInfo);
        }

        /// <summary>
        /// Convert Bytes To Bool
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataTag BoolValue(DataTag data)
        {
            dataTag = data;
            try
            {
                
                bool convertedValue = BitConverter.ToBoolean(dataTag.ModbusResponse, _startIndex);
                dataTag.Value = convertedValue.ToString();
            }
            catch (Exception ex)
            {
                _args = new(GetDateTime(), new Exception(ex.Message, ex.InnerException).ToString());
                RaiseGeneralExceptionEvent?.Invoke(this, _args);
            }

            return dataTag;
        }

        /// <summary>
        /// Convert Bytes To Short
        /// </summary>
        /// <param name="data"></param>
        /// <returns>DataTag</returns>
        public DataTag ShortValue(DataTag data)
        {
            dataTag = data;
            try
            {
                short convertedValue = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(dataTag.ModbusResponse, _startIndex));
                dataTag.Value = convertedValue.ToString("D", cultureInfo);
            }
            catch (Exception ex)
            {
                _args = new(GetDateTime(), new Exception(ex.Message, ex.InnerException).ToString());
                RaiseGeneralExceptionEvent?.Invoke(this, _args);     
            }

            return dataTag;
        }

        /// <summary>
        /// Convert Bytes To Long
        /// </summary>
        /// <param name="data"></param>
        /// <returns>DataTag</returns>
        public DataTag LongValue(DataTag data)
        {
            dataTag = data;
            try
            {
                int convertedValue = BitConverter.ToInt32(dataTag.ModbusResponse, _startIndex);
                dataTag.Value = convertedValue.ToString("D", cultureInfo);
            }
            catch (Exception ex)
            {
                _args = new(GetDateTime(), new Exception(ex.Message, ex.InnerException).ToString());
                RaiseGeneralExceptionEvent?.Invoke(this, _args);
            }

            return dataTag;
        }

        /// <summary>
        /// Convert Bytes To Float
        /// </summary>
        /// <param name="data"></param>
        /// <returns>DataTag</returns>
        public DataTag Realvalue(DataTag data)
        {

            dataTag = data;

            List<short> output = new();
            int startIndex = 9;
            while (startIndex < dataTag.ModbusResponse.Length)
            {
                try
                {
                    short value = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(dataTag.ModbusResponse, startIndex));
                    startIndex += 2;
                    output.Add(value);

                }
                catch (ArgumentException ex)
                {
                    _args = new(GetDateTime(), new ArgumentException(ex.Message, ex.InnerException).ToString());
                    RaiseGeneralExceptionEvent?.Invoke(this, _args);
                }

            }

            short[] shortValues = output.ToArray();
            int len = shortValues.Length;
            int index = 0;

            List<byte> buffer = new();
            while (index < len)
            {
                buffer.AddRange(BitConverter.GetBytes(shortValues[index]));
                index += 1;
            }

            byte[] i = buffer.ToArray();
            index = 0;
            float floatValue;

            try
            {
                floatValue = BitConverter.ToSingle(i, index);
                dataTag.Value = floatValue.ToString();
                return dataTag;
            }
            catch (Exception ex)
            {
                _args = new(GetDateTime(), new Exception(ex.Message, ex.InnerException).ToString());
                RaiseGeneralExceptionEvent?.Invoke(this, _args);
            }

            return dataTag;
        }
    }
}
