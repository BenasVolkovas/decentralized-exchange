using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using DecentralizedExchange.Contracts.Dex.ContractDefinition;

namespace DecentralizedExchange.Contracts.Dex
{
    public partial class DexService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, DexDeployment dexDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<DexDeployment>().SendRequestAndWaitForReceiptAsync(dexDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, DexDeployment dexDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<DexDeployment>().SendRequestAsync(dexDeployment);
        }

        public static async Task<DexService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, DexDeployment dexDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, dexDeployment, cancellationTokenSource);
            return new DexService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public DexService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> AddTokenRequestAsync(AddTokenFunction addTokenFunction)
        {
             return ContractHandler.SendRequestAsync(addTokenFunction);
        }

        public Task<TransactionReceipt> AddTokenRequestAndWaitForReceiptAsync(AddTokenFunction addTokenFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addTokenFunction, cancellationToken);
        }

        public Task<string> AddTokenRequestAsync(byte[] ticker, string tokenAddress)
        {
            var addTokenFunction = new AddTokenFunction();
                addTokenFunction.Ticker = ticker;
                addTokenFunction.TokenAddress = tokenAddress;
            
             return ContractHandler.SendRequestAsync(addTokenFunction);
        }

        public Task<TransactionReceipt> AddTokenRequestAndWaitForReceiptAsync(byte[] ticker, string tokenAddress, CancellationTokenSource cancellationToken = null)
        {
            var addTokenFunction = new AddTokenFunction();
                addTokenFunction.Ticker = ticker;
                addTokenFunction.TokenAddress = tokenAddress;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addTokenFunction, cancellationToken);
        }

        public Task<BigInteger> BalancesQueryAsync(BalancesFunction balancesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BalancesFunction, BigInteger>(balancesFunction, blockParameter);
        }

        
        public Task<BigInteger> BalancesQueryAsync(string returnValue1, byte[] returnValue2, BlockParameter blockParameter = null)
        {
            var balancesFunction = new BalancesFunction();
                balancesFunction.ReturnValue1 = returnValue1;
                balancesFunction.ReturnValue2 = returnValue2;
            
            return ContractHandler.QueryAsync<BalancesFunction, BigInteger>(balancesFunction, blockParameter);
        }

        public Task<string> CreateLimitOrderRequestAsync(CreateLimitOrderFunction createLimitOrderFunction)
        {
             return ContractHandler.SendRequestAsync(createLimitOrderFunction);
        }

        public Task<TransactionReceipt> CreateLimitOrderRequestAndWaitForReceiptAsync(CreateLimitOrderFunction createLimitOrderFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createLimitOrderFunction, cancellationToken);
        }

        public Task<string> CreateLimitOrderRequestAsync(byte side, byte[] ticker, BigInteger amount, BigInteger price)
        {
            var createLimitOrderFunction = new CreateLimitOrderFunction();
                createLimitOrderFunction.Side = side;
                createLimitOrderFunction.Ticker = ticker;
                createLimitOrderFunction.Amount = amount;
                createLimitOrderFunction.Price = price;
            
             return ContractHandler.SendRequestAsync(createLimitOrderFunction);
        }

        public Task<TransactionReceipt> CreateLimitOrderRequestAndWaitForReceiptAsync(byte side, byte[] ticker, BigInteger amount, BigInteger price, CancellationTokenSource cancellationToken = null)
        {
            var createLimitOrderFunction = new CreateLimitOrderFunction();
                createLimitOrderFunction.Side = side;
                createLimitOrderFunction.Ticker = ticker;
                createLimitOrderFunction.Amount = amount;
                createLimitOrderFunction.Price = price;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createLimitOrderFunction, cancellationToken);
        }

        public Task<string> CreateMarketOrderRequestAsync(CreateMarketOrderFunction createMarketOrderFunction)
        {
             return ContractHandler.SendRequestAsync(createMarketOrderFunction);
        }

        public Task<TransactionReceipt> CreateMarketOrderRequestAndWaitForReceiptAsync(CreateMarketOrderFunction createMarketOrderFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createMarketOrderFunction, cancellationToken);
        }

        public Task<string> CreateMarketOrderRequestAsync(byte side, byte[] ticker, BigInteger amount)
        {
            var createMarketOrderFunction = new CreateMarketOrderFunction();
                createMarketOrderFunction.Side = side;
                createMarketOrderFunction.Ticker = ticker;
                createMarketOrderFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(createMarketOrderFunction);
        }

        public Task<TransactionReceipt> CreateMarketOrderRequestAndWaitForReceiptAsync(byte side, byte[] ticker, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var createMarketOrderFunction = new CreateMarketOrderFunction();
                createMarketOrderFunction.Side = side;
                createMarketOrderFunction.Ticker = ticker;
                createMarketOrderFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createMarketOrderFunction, cancellationToken);
        }

        public Task<string> DepositRequestAsync(DepositFunction depositFunction)
        {
             return ContractHandler.SendRequestAsync(depositFunction);
        }

        public Task<TransactionReceipt> DepositRequestAndWaitForReceiptAsync(DepositFunction depositFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(depositFunction, cancellationToken);
        }

        public Task<string> DepositRequestAsync(BigInteger amount, byte[] ticker)
        {
            var depositFunction = new DepositFunction();
                depositFunction.Amount = amount;
                depositFunction.Ticker = ticker;
            
             return ContractHandler.SendRequestAsync(depositFunction);
        }

        public Task<TransactionReceipt> DepositRequestAndWaitForReceiptAsync(BigInteger amount, byte[] ticker, CancellationTokenSource cancellationToken = null)
        {
            var depositFunction = new DepositFunction();
                depositFunction.Amount = amount;
                depositFunction.Ticker = ticker;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(depositFunction, cancellationToken);
        }

        public Task<string> DepositEthRequestAsync(DepositEthFunction depositEthFunction)
        {
             return ContractHandler.SendRequestAsync(depositEthFunction);
        }

        public Task<string> DepositEthRequestAsync()
        {
             return ContractHandler.SendRequestAsync<DepositEthFunction>();
        }

        public Task<TransactionReceipt> DepositEthRequestAndWaitForReceiptAsync(DepositEthFunction depositEthFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(depositEthFunction, cancellationToken);
        }

        public Task<TransactionReceipt> DepositEthRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<DepositEthFunction>(null, cancellationToken);
        }

        public Task<GetOrderBookOutputDTO> GetOrderBookQueryAsync(GetOrderBookFunction getOrderBookFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetOrderBookFunction, GetOrderBookOutputDTO>(getOrderBookFunction, blockParameter);
        }

        public Task<GetOrderBookOutputDTO> GetOrderBookQueryAsync(byte[] ticker, byte side, BlockParameter blockParameter = null)
        {
            var getOrderBookFunction = new GetOrderBookFunction();
                getOrderBookFunction.Ticker = ticker;
                getOrderBookFunction.Side = side;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetOrderBookFunction, GetOrderBookOutputDTO>(getOrderBookFunction, blockParameter);
        }

        public Task<BigInteger> NextOrderIdQueryAsync(NextOrderIdFunction nextOrderIdFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NextOrderIdFunction, BigInteger>(nextOrderIdFunction, blockParameter);
        }

        
        public Task<BigInteger> NextOrderIdQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NextOrderIdFunction, BigInteger>(null, blockParameter);
        }

        public Task<OrderBookOutputDTO> OrderBookQueryAsync(OrderBookFunction orderBookFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<OrderBookFunction, OrderBookOutputDTO>(orderBookFunction, blockParameter);
        }

        public Task<OrderBookOutputDTO> OrderBookQueryAsync(byte[] returnValue1, BigInteger returnValue2, BigInteger returnValue3, BlockParameter blockParameter = null)
        {
            var orderBookFunction = new OrderBookFunction();
                orderBookFunction.ReturnValue1 = returnValue1;
                orderBookFunction.ReturnValue2 = returnValue2;
                orderBookFunction.ReturnValue3 = returnValue3;
            
            return ContractHandler.QueryDeserializingToObjectAsync<OrderBookFunction, OrderBookOutputDTO>(orderBookFunction, blockParameter);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public Task<string> RenounceOwnershipRequestAsync(RenounceOwnershipFunction renounceOwnershipFunction)
        {
             return ContractHandler.SendRequestAsync(renounceOwnershipFunction);
        }

        public Task<string> RenounceOwnershipRequestAsync()
        {
             return ContractHandler.SendRequestAsync<RenounceOwnershipFunction>();
        }

        public Task<TransactionReceipt> RenounceOwnershipRequestAndWaitForReceiptAsync(RenounceOwnershipFunction renounceOwnershipFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(renounceOwnershipFunction, cancellationToken);
        }

        public Task<TransactionReceipt> RenounceOwnershipRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<RenounceOwnershipFunction>(null, cancellationToken);
        }

        public Task<byte[]> TokenListQueryAsync(TokenListFunction tokenListFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TokenListFunction, byte[]>(tokenListFunction, blockParameter);
        }

        
        public Task<byte[]> TokenListQueryAsync(BigInteger returnValue1, BlockParameter blockParameter = null)
        {
            var tokenListFunction = new TokenListFunction();
                tokenListFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<TokenListFunction, byte[]>(tokenListFunction, blockParameter);
        }

        public Task<TokenMappingOutputDTO> TokenMappingQueryAsync(TokenMappingFunction tokenMappingFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<TokenMappingFunction, TokenMappingOutputDTO>(tokenMappingFunction, blockParameter);
        }

        public Task<TokenMappingOutputDTO> TokenMappingQueryAsync(byte[] returnValue1, BlockParameter blockParameter = null)
        {
            var tokenMappingFunction = new TokenMappingFunction();
                tokenMappingFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryDeserializingToObjectAsync<TokenMappingFunction, TokenMappingOutputDTO>(tokenMappingFunction, blockParameter);
        }

        public Task<string> TransferOwnershipRequestAsync(TransferOwnershipFunction transferOwnershipFunction)
        {
             return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(TransferOwnershipFunction transferOwnershipFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public Task<string> TransferOwnershipRequestAsync(string newOwner)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
                transferOwnershipFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(string newOwner, CancellationTokenSource cancellationToken = null)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
                transferOwnershipFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public Task<string> WithdrawRequestAsync(WithdrawFunction withdrawFunction)
        {
             return ContractHandler.SendRequestAsync(withdrawFunction);
        }

        public Task<TransactionReceipt> WithdrawRequestAndWaitForReceiptAsync(WithdrawFunction withdrawFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawFunction, cancellationToken);
        }

        public Task<string> WithdrawRequestAsync(BigInteger amount, byte[] ticker)
        {
            var withdrawFunction = new WithdrawFunction();
                withdrawFunction.Amount = amount;
                withdrawFunction.Ticker = ticker;
            
             return ContractHandler.SendRequestAsync(withdrawFunction);
        }

        public Task<TransactionReceipt> WithdrawRequestAndWaitForReceiptAsync(BigInteger amount, byte[] ticker, CancellationTokenSource cancellationToken = null)
        {
            var withdrawFunction = new WithdrawFunction();
                withdrawFunction.Amount = amount;
                withdrawFunction.Ticker = ticker;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawFunction, cancellationToken);
        }

        public Task<string> WithdrawEthRequestAsync(WithdrawEthFunction withdrawEthFunction)
        {
             return ContractHandler.SendRequestAsync(withdrawEthFunction);
        }

        public Task<TransactionReceipt> WithdrawEthRequestAndWaitForReceiptAsync(WithdrawEthFunction withdrawEthFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawEthFunction, cancellationToken);
        }

        public Task<string> WithdrawEthRequestAsync(BigInteger amount)
        {
            var withdrawEthFunction = new WithdrawEthFunction();
                withdrawEthFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(withdrawEthFunction);
        }

        public Task<TransactionReceipt> WithdrawEthRequestAndWaitForReceiptAsync(BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var withdrawEthFunction = new WithdrawEthFunction();
                withdrawEthFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawEthFunction, cancellationToken);
        }
    }
}
