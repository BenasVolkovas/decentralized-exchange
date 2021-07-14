const Dex = artifacts.require("Dex");
const Link = artifacts.require("Link");
const truffleAssert = require("truffle-assertions");

contract("Dex", (accounts) => {
    it("should only be the owner to add tokens", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();
        await truffleAssert.passes(
            dex.addToken(web3.utils.fromUtf8(link.symbol()), link.address, {
                from: accounts[0],
            })
        );
        await truffleAssert.reverts(
            dex.addToken(web3.utils.fromUtf8(link.symbol()), link.address, {
                from: accounts[1],
            })
        );
    });

    it("should handle deposits correctly", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();
        await dex.addToken(web3.utils.fromUtf8(link.symbol()), link.address);
        await link.approve(dex.address, 500);
        await dex.deposit(100, web3.utils.fromUtf8(link.symbol()));
        let balance = await dex.balances(
            accounts[0],
            web3.utils.fromUtf8(link.symbol())
        );
        assert.equal(balance.toNumber(), 100);
    });

    it("should handle faulty withdrawals correctly", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();
        await dex.addToken(web3.utils.fromUtf8(link.symbol()), link.address);
        await link.approve(dex.address, 500);
        await dex.deposit(100, web3.utils.fromUtf8(link.symbol()));

        await truffleAssert.reverts(
            dex.withdraw(500, web3.utils.fromUtf8(link.symbol()))
        );
    });

    it("should handle correct withdrawals correctly", async () => {
        let dex = await Dex.deployed();
        let link = await Link.deployed();
        await dex.addToken(web3.utils.fromUtf8(link.symbol()), link.address);
        await link.approve(dex.address, 500);
        await dex.deposit(100, web3.utils.fromUtf8(link.symbol()));

        await truffleAssert.passes(
            dex.withdraw(100, web3.utils.fromUtf8(link.symbol()))
        );
    });
});
