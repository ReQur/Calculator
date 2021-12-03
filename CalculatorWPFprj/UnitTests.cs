using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace CalculatorWPFprj
{
    public class Tests
    {
        private Calculator calc;
        [SetUp]
        public void Setup()
        {
            calc = new Calculator();
        }

        //[Test]
        //public void Test1()
        //{
        //    calc = new Calculator();

        //    calc.Calc("7+8*(10+(-5))").Should().Be("47");
        //}
        //[Test]
        //public void Test2()
        //{
        //    calc = new Calculator();

        //    calc.Calc("  -1 -1 -11-1-13").Should().Be("-27");
        //}

        //[Test]
        //public void Test3()
        //{
        //    calc = new Calculator();

        //    calc.Calc("5*5*5*5*5").Should().Be("3125");
        //}

        [Test]
        public void Should_BeSuccess_WhenCalculationIsRight()
        {
            calc = new Calculator();

            calc.Calc("4,5+5,5*3,6").Should().Be("24,3");
            calc.Calc("5*5*5*5*5").Should().Be("3125");
            calc.Calc("  -1 -1 -11-1-13").Should().Be("-27");
            calc.Calc("7+8*(10+(-5))").Should().Be("47");
        }

        [Test]
        public void Should_ThrowException_WhenStringIsEmpty()
        {
            calc = new Calculator();

            Assert.Throws<ArgumentException>(() => calc.Calc(" "));
        }


        [Test]
        public void Should_ThrowException_WhenStringIsIncorrect()
        {
            calc = new Calculator();

            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => calc.Calc(" 1**1"));
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => calc.Calc("randomstring"));
        }
    }
}
