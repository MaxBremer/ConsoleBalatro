using ConsoleBalatro.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleBalatro.Tests
{
    public class VoucherTests : TestClassBase
    {
        [Fact]
        public void AddVoucher_Grabber_CorrectlyAddsHandAndIncludesUpgrade()
        {
            ResetToBlindSelection();
            var oldHandCount = Globals.MaxHandsPerRound;
            Assert.False(VoucherIsInMarket("Nacho Tong"));
            AddVoucher("Grabber");
            Assert.Equal(oldHandCount + 1, Globals.MaxHandsPerRound);
            Assert.True(VoucherIsInMarket("Nacho Tong"));
        }
    }
}
