using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            

            // Function Code 1 Byte, Starting Address 2 Bytes, Quantity 2 Bytes
            byte[] data = new byte[12];

            ModbusReadCommandParameters ModbusRead = this.CommandParameters as ModbusReadCommandParameters;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModbusRead.TransactionId)), 0, data, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModbusRead.ProtocolId)), 0, data, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModbusRead.Length)), 0, data, 4, 2);
            data[6] = ModbusRead.UnitId;
            data[7] = ModbusRead.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModbusRead.StartAddress)), 0, data, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModbusRead.Quantity)), 0, data, 10, 2);

            
            return data;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            ModbusReadCommandParameters ModbusRead = this.CommandParameters as ModbusReadCommandParameters;

            Dictionary<Tuple<PointType, ushort>, ushort> dic = new Dictionary<Tuple<PointType, ushort>, ushort>();

            int count = 0;
            ushort adresa = ModbusRead.StartAddress;
            ushort value;
            byte maska = 1;
            for (int i = 0; i < response[8]; i++)
            {
                byte tempbyte = response[9 + i];
                for (int j = 0; j < 8; j++)
                {
                    value = (ushort)(tempbyte & maska);
                    tempbyte >>= 1;
                    dic.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, adresa), value);
                    count++;
                    adresa++;
                    if (count == ModbusRead.Quantity)
                    {
                        break;
                    }

                }
            }
            return dic;
        }
    }
}