﻿using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            
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
            ModbusReadCommandParameters ModbusRead = this.CommandParameters as ModbusReadCommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> dic = new Dictionary<Tuple<PointType, ushort>, ushort>();
            ushort byte_count = response[8];
            ushort value;

            int byte02_start = 7;
            int byte01_start = 8;
            for (int i = 0; i < byte_count / 2; i++)
            {
                byte second_byte = response[byte02_start += 2];
                byte first_byte = response[byte01_start += 2];
                //Biti vece vaznosti se stavljaju na pocetak a biti manje vaznost se siftuju da bi s emoglo lepo spojiti i stavljaju na bite manje vaznosti
                value = (ushort)(first_byte + (second_byte << 8));


                dic.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, (ushort)(ModbusRead.StartAddress + i)), value);
            }
            return dic;
        }
    }
}