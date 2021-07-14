using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace DecentralizedExchange.Contracts.Dex.ContractDefinition
{
    public partial class Order : OrderBase { }

    public class OrderBase 
    {
        [Parameter("uint256", "id", 1)]
        public virtual BigInteger Id { get; set; }
        [Parameter("address", "trader", 2)]
        public virtual string Trader { get; set; }
        [Parameter("uint8", "side", 3)]
        public virtual byte Side { get; set; }
        [Parameter("bytes32", "ticker", 4)]
        public virtual byte[] Ticker { get; set; }
        [Parameter("uint256", "amount", 5)]
        public virtual BigInteger Amount { get; set; }
        [Parameter("uint256", "price", 6)]
        public virtual BigInteger Price { get; set; }
        [Parameter("uint256", "filled", 7)]
        public virtual BigInteger Filled { get; set; }
    }
}
