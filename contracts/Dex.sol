// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;
pragma experimental ABIEncoderV2;

import "./Wallet.sol";

contract Dex is Wallet {

	using SafeMath for uint256;

	enum Side {
		BUY,   // 0
		SELL   // 1
	}

	struct Order {
		uint id;
		address trader;
		Side side;
		bytes32 ticker;
		uint amount;
		uint price;
		uint filled;
	}

	uint public nextOrderId = 0;
	mapping(bytes32 => mapping(uint => Order[])) public orderBook;

	function getOrderBook(bytes32 _ticker, Side _side) view public returns(Order[] memory) {
		return orderBook[_ticker][uint(_side)];
	}

	function createLimitOrder(Side _side, bytes32 _ticker, uint _amount, uint _price) public {
		if (_side == Side.BUY) {
			require(balances[msg.sender]["ETH"] >= _amount.mul(_price), "Don't have enough ether to buy tokens");
		} else if (_side == Side.SELL) {
			require(balances[msg.sender][_ticker] >= _amount, "Don't have enough tokens amount to sell");
		}

		Order[] storage orders = orderBook[_ticker][uint(_side)]; // Not a copy, but a referece
		orders.push(
			Order(nextOrderId, msg.sender, _side, _ticker, _amount, _price, 0)
		);

		
		// Bubble sorting
		uint i = orders.length > 0 ? orders.length - 1 : 0;

		if (_side == Side.BUY) {
			for (i; i > 0; i--) {
				if (orders[i].price > orders[i-1].price) {
					Order memory temp = orders[i];
					orders[i] = orders[i-1];
					orders[i-1] = temp;
				}
			}
		} else if (_side == Side.SELL) {
			for (i; i > 0; i--) {
				if (orders[i].price < orders[i-1].price) {
					Order memory temp = orders[i];
					orders[i] = orders[i-1];
					orders[i-1] = temp;
				}
			}
		}

		nextOrderId.add(1);
	}


	function createMarketOrder(Side _side, bytes32 _ticker, uint _amount) public {
		uint orderbookSide;
		if (_side == Side.BUY) { // Wants to buy, get sell orderbook
			orderbookSide = uint(Side.SELL);
		} else { // Wants to sell, get buy orderbook
			require(balances[msg.sender][_ticker] >= _amount, "Insuffient balance");
			orderbookSide = uint(Side.BUY);
		}

		Order[] storage orders = orderBook[_ticker][orderbookSide];

      
		uint totalFilled = 0;
		for (uint256 i = 0; i < orders.length && totalFilled < _amount; i++) {
			uint limitOrderLeftAmount = orders[i].amount.sub(orders[i].filled);
			uint filled = 0;
			if (totalFilled.add(limitOrderLeftAmount) > _amount) {
				filled = _amount.sub(totalFilled);
			} else {
				filled = limitOrderLeftAmount;		
			}
			orders[i].filled = orders[i].filled.add(filled);
			totalFilled = totalFilled.add(filled);

			uint etherAmount = filled.mul(orders[i].price);
			if (orderbookSide == 1) { // SELL orderbook
				require(balances[msg.sender]["ETH"] >= etherAmount);

				// Change ticker balances
				balances[orders[i].trader][_ticker] = balances[orders[i].trader][_ticker].sub(filled);
				balances[msg.sender][_ticker] = balances[msg.sender][_ticker].add(filled);

				// Change ether balances
				balances[msg.sender]["ETH"] = balances[msg.sender]["ETH"].sub(etherAmount);
				balances[orders[i].trader]["ETH"] = balances[orders[i].trader]["ETH"].add(etherAmount);
			} else { // BUY orderbook
				balances[msg.sender][_ticker] = balances[msg.sender][_ticker].sub(filled);
				balances[orders[i].trader][_ticker] = balances[orders[i].trader][_ticker].add(filled);

				balances[orders[i].trader]["ETH"] = balances[orders[i].trader]["ETH"].sub(etherAmount);
				balances[msg.sender]["ETH"] = balances[msg.sender]["ETH"].add(etherAmount);
			}
		}

		//Remove 100% filled orders from the orderbook
		while(orders.length > 0 && orders[0].filled == orders[0].amount){
			//Remove the top element in the orders array by overwriting every element
			// with the next element in the order list
			for (uint256 i = 0; i < orders.length - 1; i++) {
				orders[i] = orders[i + 1];
			}
			orders.pop();
		}


		/*
		bool foundFilledOrders = false;
		uint lastFilledOrder = 0;
		bool noFilledOrders = false;
		if (orders.length > 0) {
			for (uint256 i = 0; i < orders.length && foundFilledOrders == false; i++) {
				if (orders[i].filled == orders[i].amount) {
					lastFilledOrder = i;
				} else if (i == 0) {
					noFilledOrders = true;
					foundFilledOrders = true;
				} else {
					foundFilledOrders = true;
				}
			}

			if (noFilledOrders == false) {
				if (lastFilledOrder + 1 == orders.length) {
					for (uint256 i = 0; i < orders.length; i++) {
						orders.pop();
					}
				} else {
					uint placeToCopy = lastFilledOrder.add(1);
					for (uint256 i = 0; i < orders.length - lastFilledOrder - 1; i++) {
						orders[i] = orders[placeToCopy];
						placeToCopy++;
					}

					for (uint256 i = 0; i < lastFilledOrder+1; i++) {
						orders.pop();
					}
				}
			}
		}
		*/		

	}
}