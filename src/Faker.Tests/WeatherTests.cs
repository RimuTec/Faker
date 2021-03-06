﻿using NUnit.Framework;
using System;
using System.Linq;

namespace RimuTec.Faker.Tests
{
   [TestFixture]
   [TestFixtureSource(typeof(DefaultFixtureData), nameof(DefaultFixtureData.FixtureParams))]
   public class WeatherTests
   {
      public WeatherTests(string locale)
      {
         Locale = locale;
      }

      [SetUp]
      public void SetUp()
      {
         Config.Locale = Locale;
      }

      private string Locale { get; }

      [Test]
      public void Summary_HappyDays()
      {
         var weather = Weather.Summary();
         Assert.IsTrue(summaries.Contains(weather));
      }

      [Test]
      public void Forecast_HappyDays()
      {
         var now = DateTime.UtcNow;
         var forecast = Weather.Forecast();
         Assert.AreEqual(5, forecast.Count());
         foreach (var daily in forecast)
         {
            Assert.IsTrue(daily.Date >= now);
            Assert.IsTrue(daily.TemperatureC >= -20 && daily.TemperatureC < 55,
               $"daily.TemperatureC is '{daily.TemperatureC}'");
            Assert.IsTrue(summaries.Contains(daily.Summary));
         }
      }

      [Test]
      public void Forecost_MultipleTimes()
      {
         for (var i = 0; i < 100; i++)
         {
            Weather.Forecast();
         }
      }

      [Test]
      public void Forecast_10Days()
      {
         const int numberOfDays = 10;
         var forecast = Weather.Forecast(numberOfDays);
         Assert.AreEqual(numberOfDays, forecast.Count());
      }

      [Test]
      public void Forecast_Rejects_Negative_NumberOfDays()
      {
         const int numberOfDays = -1;
         var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Weather.Forecast(numberOfDays));
         Assert.AreEqual("Must not be negative. (Parameter 'numberOfDays')", ex.Message);
      }

      private readonly string[] summaries = new string[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
   }
}
