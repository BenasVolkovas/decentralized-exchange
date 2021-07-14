const Dex = artifacts.require("Dex");
const Link = artifacts.require("Link");
const truffleAssert = require("truffle-assertions");

// Limit orders
contract.skip("Dex", (accounts) => {
    // #01
    // The user must have ETH deposited such that deposited eth >= buy order value
    it("should throw an error if ETH balance is too low when creating BUY limit order", async () => {
        let dex = await Dex.deployed();

        await truffleAssert.reverts(
            dex.createLimitOrder(0, web3.utils.fromUtf8("LINK"), 10, 1)
        );
        await dex.depositEth({ value: 10 });
        await truffleAssert.passes(
            dex.createLimitOrder(0, web3.utils.fromUtf8("LINK"), 10, 1)
        );
    });

    // #02
    // The user must have enough tokens deposited such that token balance >= sell order amount
    it("should throw an error if token balance is too low when creating SELL limit order", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();
        await truffleAssert.reverts(
            dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 10, 1)
        );
        await dex.addToken(web3.utils.fromUtf8("LINK"), link.address);
        await link.approve(dex.address, 500);
        await dex.deposit(10, web3.utils.fromUtf8("LINK"));
        await truffleAssert.passes(
            dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 10, 1)
        );
    });

    // #03
    // The BUY order book should be ordered on price from highest to lowest starting at index 0
    it("The BUY order book should be ordered on price from highest to lowest starting at index 0", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();
        await dex.depositEth({ value: 1000 });

        await dex.createLimitOrder(0, web3.utils.fromUtf8("LINK"), 1, 300);
        await dex.createLimitOrder(0, web3.utils.fromUtf8("LINK"), 1, 100);
        await dex.createLimitOrder(0, web3.utils.fromUtf8("LINK"), 1, 200);

        let orderbook = await dex.getOrderBook(web3.utils.fromUtf8("LINK"), 0);
        assert(orderbook.length > 0);

        await console.log(orderbook);

        for (let i = 0; i < orderbook.length - 1; i++) {
            assert(
                orderbook[i].price >= orderbook[i + 1].price,
                String(i + 1).concat(
                    " element is not in right order place (BUY)"
                )
            );
        }
    });

    // #04
    // The SELL order book should be ordered on price from lowest to highest starting at index 0
    it("The SELL order book should be ordered on price from lowest to highest starting at index 0", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();
        await dex.addToken(web3.utils.fromUtf8("LINK"), link.address);
        await link.approve(dex.address, 1000);
        await dex.deposit(900, web3.utils.fromUtf8("LINK"));

        await dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 1, 300);
        await dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 1, 100);
        await dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 1, 200);

        let orderbook = await dex.getOrderBook(web3.utils.fromUtf8("LINK"), 1);
        assert(orderbook.length > 0);

        await console.log(orderbook);

        for (let i = 0; i < orderbook.length - 1; i++) {
            assert(
                orderbook[i].price <= orderbook[i + 1].price,
                String(i + 1).concat(
                    " element is not in right order place (SELL)"
                )
            );
        }
    });
});

// Market orders
contract("Dex", (accounts) => {
    // #01
    // When creating a SELL order, the seller needs to have enough tokens for the trade
    it("When creating a SELL order, the seller needs to have enough tokens for the trade", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();
        await dex.addToken(web3.utils.fromUtf8("LINK"), link.address);

        let balance = await dex.balances(
            accounts[0],
            web3.utils.fromUtf8("LINK")
        );

        assert(balance.toNumber() == 0, "Initial LINK balance is not 0!");

        await truffleAssert.passes(
            dex.createLimitOrder(0, web3.utils.fromUtf8("LINK"), 1, 0, {
                from: accounts[1],
            })
        );

        await truffleAssert.reverts(
            dex.createMarketOrder(1, web3.utils.fromUtf8("LINK"), 1)
        );

        await link.approve(dex.address, 1);
        await dex.deposit(1, web3.utils.fromUtf8("LINK"));
        await truffleAssert.passes(
            dex.createMarketOrder(1, web3.utils.fromUtf8("LINK"), 1)
        );
    });

    // #02
    // When creating a BUY market order, the buyer needs to have enough ETH for the trade
    it("When creating a BUY market order, the buyer needs to have enough ETH for the trade", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();

        let balance = await dex.balances(
            accounts[0],
            web3.utils.fromUtf8("ETH")
        );

        assert(balance.toNumber() == 0, "Initial ETH balance is not 0!");

        await link.transfer(accounts[1], 1);
        await link.approve(dex.address, 1, { from: accounts[1] });
        await dex.deposit(1, web3.utils.fromUtf8("LINK"), {
            from: accounts[1],
        });

        await truffleAssert.passes(
            dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 1, 500, {
                from: accounts[1],
            })
        );

        await truffleAssert.reverts(
            dex.createMarketOrder(0, web3.utils.fromUtf8("LINK"), 1)
        );

        await dex.depositEth({ value: 500 });
        await truffleAssert.passes(
            dex.createMarketOrder(0, web3.utils.fromUtf8("LINK"), 1)
        );
    });

    /*
    // #03
    // Market orders can be submitted even if the order book is empty
    it("Market orders can be submitted even if the order book is empty", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();
        let sellOrderbook = await dex.getOrderBook(
            web3.utils.fromUtf8("LINK"),
            1
        );
        let buyOrderbook = await dex.getOrderBook(
            web3.utils.fromUtf8("LINK"),
            0
        );

        assert(
            sellOrderbook.length == 0,
            "SELL side orderbook length is not 0"
        );

        assert(buyOrderbook.length == 0, "BUY side orderbook length is not 0");

        await link.approve(dex.address, 10);
        await dex.deposit(10, web3.utils.fromUtf8("LINK"));

        await truffleAssert.passes(
            dex.createMarketOrder(1, web3.utils.fromUtf8("LINK"), 10)
        );

        await truffleAssert.passes(
            dex.createMarketOrder(0, web3.utils.fromUtf8("LINK"), 10)
        );
    });

    // #04
    // Market order should not fill more limit orders than the market order amount
    it("Market order should not fill more limit orders than the market order amount", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();

        let sellOrderbook = await dex.getOrderBook(
            web3.utils.fromUtf8("LINK"),
            1
        );

        assert(
            sellOrderbook.length == 0,
            "SELL side orderbook is not empty at the start"
        );

        // Send LINK tokens to accounts 1, 2, 3 from account 0
        await link.transfer(accounts[1], 5);
        await link.transfer(accounts[2], 5);
        await link.transfer(accounts[3], 5);

        // Approve DEX for accounts 1, 2, 3
        await link.approve(dex.address, 5, { from: accounts[1] });
        await link.approve(dex.address, 5, { from: accounts[2] });
        await link.approve(dex.address, 5, { from: accounts[3] });

        // Deposit LINK into DEX for accounts 1, 2, 3
        await dex.deposit(5, web3.utils.fromUtf8("LINK"), {
            from: accounts[1],
        });
        await dex.deposit(5, web3.utils.fromUtf8("LINK"), {
            from: accounts[2],
        });
        await dex.deposit(5, web3.utils.fromUtf8("LINK"), {
            from: accounts[3],
        });

        // Fill up the sell order book with
        await dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 5, 300, {
            from: accounts[1],
        });
        await dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 5, 400, {
            from: accounts[2],
        });
        await dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 5, 500, {
            from: accounts[3],
        });

        await dex.depositEth({ value: 3500 });

        // Create market order that should fill 2/3 orders in the book
        await dex.createMarketOrder(0, web3.utils.fromUtf8("LINK"), 10);

        sellOrderbook = await dex.getOrderBook(web3.utils.fromUtf8("LINK"), 1);
        assert(
            sellOrderbook.length == 1,
            "SELL side orderbook should only have 1 order left"
        );
        assert(
            sellOrderbook[0].filled == 0,
            "SELL side order should have 0 filled"
        );
    });

    // #05
    // Market orders should be filled until the order boor is empty
    it("Market orders should be filled until the order boor is empty", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();

        let sellOrderbook = await dex.getOrderBook(
            web3.utils.fromUtf8("LINK"),
            1
        );
        assert(
            sellOrderbook.length == 1,
            "SELL side orderbook should have 1 order left"
        );

        await link.transfer(accounts[1], 5);
        await link.approve(dex.address, 5, { from: accounts[1] });
        await dex.deposit(5, web3.utils.fromUtf8("LINK"), {
            from: accounts[1],
        });

        // Fill the SELL order book
        await dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 5, 300, {
            from: accounts[1],
        });

        // Check buyer LINK balance before purchase
        let balanceBefore = await dex.balances(
            accounts[0],
            web3.utils.fromUtf8("LINK")
        );

        await dex.depositEth({ value: 4000 });

        // Create market order that should fill more orders than in the entire book
        await dex.createMarketOrder(0, web3.utils.fromUtf8("LINK"), 20);

        // Check buyer LINK balance after purchase
        let balanceAfter = await dex.balances(
            accounts[0],
            web3.utils.fromUtf8("LINK")
        );

        // Buyer should have 10 more LINK after, even though order was for 20
        assert(
            balanceBefore.toNumber() + 10 == balanceAfter.toNumber(),
            "LINK balance is not equal to the expected one"
        );

        sellOrderbook = await dex.getOrderBook(web3.utils.fromUtf8("LINK"), 1);
        assert(
            sellOrderbook.length == 0,
            "SELL side orderbook should not have unfilled order left"
        );
    });

    // #06
    // The ETH balance of the buyer should decrease with the filled amount
    it("The ETH balance of the buyer should decrease with the filled amount", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();

        // Seller deposits LINK and creates a sell limit order for 1 LINK for 300 wei
        await link.transfer(accounts[1], 1);
        await link.approve(dex.address, 1, { from: accounts[1] });
        await dex.deposit(1, web3.utils.fromUtf8("LINK"), {
            from: accounts[1],
        });
        await dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 1, 300, {
            from: accounts[1],
        });

        // Deposit ETH -> check balance before -> make a trade -> chack balance after
        await dex.depositEth({ value: 300 });
        let balanceBefore = await dex.balances(
            accounts[0],
            web3.utils.fromUtf8("ETH")
        );
        await dex.createMarketOrder(0, web3.utils.fromUtf8("LINK"), 1);
        let balanceAfter = await dex.balances(
            accounts[0],
            web3.utils.fromUtf8("ETH")
        );

        assert(balanceBefore - 300 == balanceAfter);
    });

    // #07
    // The token balance of the seller should decrease with the filled amount
    it("The token balance of the seller should decrease with the filled amount", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();

        let sellOrderbook = await dex.getOrderBook(
            web3.utils.fromUtf8("LINK"),
            1
        );
        assert(
            sellOrderbook.length == 0,
            "SELL side orderbook should be empty at the start"
        );

        await link.transfer(accounts[1], 1);
        await link.approve(dex.address, 1, { from: accounts[1] });
        await dex.deposit(1, web3.utils.fromUtf8("LINK"), {
            from: accounts[1],
        });

        await dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 1, 1000, {
            from: accounts[1],
        });

        // Check sellers LINK balances before trade
        let balanceBefore = await dex.balances(
            accounts[1],
            web3.utils.fromUtf8("LINK")
        );

        await dex.depositEth({ value: 1000 });

        // Account 0 creates market order to buy up both sell orders
        await dex.createMarketOrder(0, web3.utils.fromUtf8("LINK"), 1);

        // Check sellers LINK balances after trade
        let balanceAfter = await dex.balances(
            accounts[1],
            web3.utils.fromUtf8("LINK")
        );

        assert(
            balanceBefore.toNumber() - 1 == balanceAfter.toNumber(),
            "LINK balance is not equal to the expected one"
        );
    });

    // #08
    // Filled limit order should be removed from the orderbook
    it("Filled limit order should be removed from the orderbook", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();

        // Orderbook should be empty at the start
        let sellOrderbook = await dex.getOrderBook(
            web3.utils.fromUtf8("LINK"),
            1
        );
        assert(
            sellOrderbook.length == 0,
            "SELL side orderbook should be empty at the start"
        );

        await link.transfer(accounts[1], 1);
        await link.approve(dex.address, 1, { from: accounts[1] });
        await dex.deposit(1, web3.utils.fromUtf8("LINK"), {
            from: accounts[1],
        });
        await dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 1, 600, {
            from: accounts[1],
        });

        // Orderbook should have 1 limit order
        sellOrderbook = await dex.getOrderBook(web3.utils.fromUtf8("LINK"), 1);
        assert(
            sellOrderbook.length == 1,
            "SELL side orderbook should have 1 limit order"
        );

        await dex.depositEth({ value: 600 });
        await dex.createMarketOrder(0, web3.utils.fromUtf8("LINK"), 1);

        // Orderbook should be empty at the end
        sellOrderbook = await dex.getOrderBook(web3.utils.fromUtf8("LINK"), 1);
        assert(
            sellOrderbook.length == 0,
            "SELL side orderbook should be empty at the end"
        );
    });

    // #09
    // Limit order filled property should be set correctly after a trade
    it("Limit order filled property should be set correctly after a trade", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();

        let sellOrderbook = await dex.getOrderBook(
            web3.utils.fromUtf8("LINK"),
            1
        );
        assert(
            sellOrderbook.length == 0,
            "SELL side orderbook should be empty at the start"
        );

        await link.transfer(accounts[1], 5);
        await link.approve(dex.address, 5, { from: accounts[1] });
        await dex.deposit(5, web3.utils.fromUtf8("LINK"), {
            from: accounts[1],
        });
        await dex.createLimitOrder(1, web3.utils.fromUtf8("LINK"), 5, 300, {
            from: accounts[1],
        });

        sellOrderbook = await dex.getOrderBook(web3.utils.fromUtf8("LINK"), 1);
        console.log(sellOrderbook);

        await dex.depositEth({ value: 600 });
        await dex.createMarketOrder(0, web3.utils.fromUtf8("LINK"), 2);

        sellOrderbook = await dex.getOrderBook(web3.utils.fromUtf8("LINK"), 1);
        console.log(sellOrderbook);

        assert(sellOrderbook[0].filled == 2);
        assert(sellOrderbook[0].amount == 5);
    });
	*/
});
