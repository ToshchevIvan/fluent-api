using System;
using System.IO;
using NUnit.Framework;
using FluentAssertions;


namespace ObjectPrinting.Tests
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class ObjectPrinter_should
    {
        private Person person;

        [SetUp]
        public void SetUp()
        {
            person = new Person
            {
                Name = "Alex",
                Age = 19,
                Height = 170,
                Father = new Person
                {
                    Name = "Philippus Aureolus Theophrastus Bombastus von Hohenheim",
                    Age = 50,
                    Height = 172
                },
                Mother = new Person
                {
                    Name = "Elisabeth",
                    Age = 47,
                    Height = 165
                }
            };
        }

        [Test]
        public void PrintWithoutModifiers()
        {
            person.PrintToString()
                .Should()
                .Be(ReadCase("PrintWithoutModifiers"));
        }

        [Test]
        public void ExcludeTypes()
        {
            ObjectPrinter.For<Person>()
                .Excluding<Guid>()
                .Excluding<Person>()
                .Excluding<int>()
                .Excluding<double>()
                .PrintToString(person)
                .Should()
                .Be(ReadCase("ExcludeTypes"));
        }

        [Test]
        public void ExcludeProperties()
        {
            ObjectPrinter.For<Person>()
                .Excluding(p => p.Father)
                .Excluding(p => p.Mother)
                .PrintToString(person)
                .Should()
                .Be(ReadCase("ExcludeProperties"));
        }

        [Test]
        public void PrintNullsCorrectly()
        {
            ObjectPrinter.For<Person>()
                .PrintToString(person.Mother)
                .Should()
                .Be(ReadCase("PrintNullsCorrectly"));
        }

        [Test]
        public void TrimStringsToLength()
        {
            ObjectPrinter.For<Person>()
                .Printing<string>().TrimmedToLength(10)
                .PrintToString(person)
                .Should()
                .Be(ReadCase("TrimStringsToLength"));
        }

        [Test]
        public void ExcludePropertiesRecursively()
        {
            ObjectPrinter.For<Person>()
                .Excluding(p => p.Father)
                .PrintToString(person)
                .Should()
                .Be(ReadCase("ExcludePropertiesRecursively"));
        }

        [Test]
        public void BeAbleToUseCustomSerializer()
        {
            Console.WriteLine(ObjectPrinter.For<Person>()
                .Printing(p => p.Height).Using(n => $"{(int) (n / 100)}.{n % 100} m")
                .PrintToString(person));
            ObjectPrinter.For<Person>()
                .Printing(p => p.Height).Using(n => $"{(int) (n / 100)}.{n % 100} m")
                .PrintToString(person)
                .Should()
                .Be(ReadCase("BeAbleToUseCustomSerializer"));
        }

        private static string ReadCase(string caseName)
        {
            return File.ReadAllText(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, @"Tests/TestCases/", $"{caseName}.txt"));
        }
    }
}
