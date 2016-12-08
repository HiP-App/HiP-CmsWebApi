using NSubstitute;
using NUnit.Framework;

namespace Api.Tests.Nunit
{
    [TestFixture]
    public class CalculatorTest
    {
        [Test]
        public void ShouldAddTwoNumbers()
        {
            var calc = Substitute.For<ICalculator>();
            calc.Add(7, 8).Returns(15);

            int expectedResult = calc.Add(7, 8);

            Assert.That(expectedResult, Is.EqualTo(15));
        }
    }
}
