﻿using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace RimuTec.Faker.Tests
{
   [TestFixture]
   [TestFixtureSource(typeof(AddressFixtureData), nameof(AddressFixtureData.FixtureParams))]
   public class AddressTests : FixtureBase
   {
      public AddressTests(string locale, Dictionary<string, string> localeSpecificRegex)
      {
         Locale = locale;
         // defaults
         DefaultRegex[nameof(BuildingNumber_HappyDays)] = "^[0-9]{3,5}";
         DefaultRegex[nameof(CityPrefix_HappyDays)] = "^[A-Z][a-z]+";
         DefaultRegex[nameof(CitySuffix_HappyDays)] = "^[a-z]{1,}";
         DefaultRegex[nameof(Community_HappyDays)] = "^[A-Z][a-z]+ [A-Z][a-z]+";
         DefaultRegex[nameof(Postcode_With_Default_Value)] = "^([0-9]{5}[-][0-9]{4})$|^([0-9]{5})$";
         // locale specific
         if (localeSpecificRegex != null)
         {
            foreach (var key in localeSpecificRegex.Keys)
            {
               DefaultRegex[key] = localeSpecificRegex[key];
            }
         }
      }

      private readonly Dictionary<string, string> DefaultRegex = new Dictionary<string, string>();

      private string Locale { get; }

      [SetUp]
      public void SetUp()
      {
         Config.Locale = Locale;
      }

      [Test]
      public void BuildingNumber_HappyDays()
      {
         var buildingNumber = Address.BuildingNumber();
         Assert.IsFalse(string.IsNullOrWhiteSpace(buildingNumber));
         Assert.IsFalse(buildingNumber.Contains("#"));
         Assert.IsFalse(buildingNumber.Contains("?"));
         Assert.GreaterOrEqual(Regex.Matches(buildingNumber, DefaultRegex[nameof(BuildingNumber_HappyDays)]).Count, 1,
            $"Locale {Locale}: Incorrect value is: '{buildingNumber}'. Regex used: '{DefaultRegex[nameof(BuildingNumber_HappyDays)]}'");
      }

      [Test]
      public void City_HappyDays()
      {
         var city = Address.City();
         Assert.IsFalse(string.IsNullOrWhiteSpace(city));
         Assert.IsFalse(city.Contains("#"), $"Incorrect value is: '{city}'");
         Assert.IsFalse(city.Contains("?"), $"Incorrect value is: '{city}'");
      }

      [Test]
      public void CityPrefix_HappyDays()
      {
         var cityPrefix = Address.CityPrefix();
         Assert.GreaterOrEqual(Regex.Matches(cityPrefix, DefaultRegex[nameof(CityPrefix_HappyDays)]).Count, 1,
            $"Locale {Locale}: Incorrect value is: '{cityPrefix}'. Regex used: '{DefaultRegex[nameof(CityPrefix_HappyDays)]}'");
      }

      [Test]
      public void CitySuffix_HappyDays()
      {
         var citySuffix = Address.CitySuffix();
         Assert.GreaterOrEqual(Regex.Matches(citySuffix, DefaultRegex[nameof(CitySuffix_HappyDays)]).Count, 1,
            $"Locale {Locale}: Incorrect value is: '{citySuffix}'. Regex used: '{DefaultRegex[nameof(CitySuffix_HappyDays)]}'");
         //Assert.IsFalse(string.IsNullOrWhiteSpace(citySuffix));
         Assert.IsFalse(citySuffix.Contains("#"));
         Assert.IsFalse(citySuffix.Contains("?"));
      }

      [Test]
      public void Community_HappyDays()
      {
         var community = Address.Community();
         Assert.IsFalse(string.IsNullOrWhiteSpace(community));
         Assert.GreaterOrEqual(Regex.Matches(community, DefaultRegex[nameof(Community_HappyDays)]).Count, 1,
            $"Locale {Locale}: Incorrect value is: '{community}'. Regex used: '{DefaultRegex[nameof(Community_HappyDays)]}'");
      }

      [Test]
      public void Country_HappyDays()
      {
         var country = Address.Country();
         Assert.IsFalse(string.IsNullOrWhiteSpace(country));
         Assert.IsFalse(country.Contains("#"));
         Assert.IsFalse(country.Contains("?"));
         var countries = Fetch("address.country");
         Assert.IsTrue(countries.Contains(country));
      }

      [Test]
      public void CountryCode_HappyDays()
      {
         var countryCode = Address.CountryCode();
         Assert.IsFalse(string.IsNullOrWhiteSpace(countryCode));
         Assert.IsFalse(countryCode.Contains("#"));
         Assert.IsFalse(countryCode.Contains("?"));
         var countryCodes = Fetch("address.country_code");
         Assert.IsTrue(countryCodes.Contains(countryCode));
      }

      [Test]
      public void CountryCodeLong_HappyDays()
      {
         var countryCodeLong = Address.CountryCodeLong();
         Assert.IsFalse(string.IsNullOrWhiteSpace(countryCodeLong));
         Assert.IsFalse(countryCodeLong.Contains("#"));
         Assert.IsFalse(countryCodeLong.Contains("?"));
         var countryCodesLong = Fetch("address.country_code_long");
         Assert.IsTrue(countryCodesLong.Contains(countryCodeLong));
      }

      [Test]
      public void FullAddress_HappyDays()
      {
         try
         {
            var fullAddress = Address.FullAddress();
            Assert.IsFalse(string.IsNullOrWhiteSpace(fullAddress));
            Assert.IsFalse(fullAddress.Contains("#"), $"Incorrect value is: '{fullAddress}'");
            Assert.IsFalse(fullAddress.Contains("?"), $"Incorrect value is: '{fullAddress}'");
         }
         catch
         {
            Assert.Fail($"Locale {Locale}");
         }
      }

      [Test]
      public void Latitude_HappyDays()
      {
         var tries = RandomNumber.Next(5, 15);
         while (tries-- > 0)
         {
            var latitude = Address.Latitude();
            Assert.GreaterOrEqual(latitude, -90);
            Assert.Less(latitude, 90);
         }
      }

      [Test]
      public void Longitude_HappyDays()
      {
         var tries = RandomNumber.Next(5, 15);
         while (tries-- > 0)
         {
            var longitude = Address.Longitude();
            Assert.GreaterOrEqual(longitude, -180);
            Assert.Less(longitude, 180);
         }
      }

      [Test]
      public void Postcode_With_Default_Value()
      {
         try
         {
            var postcode = Address.Postcode();
            Assert.IsFalse(string.IsNullOrWhiteSpace(postcode));
            Assert.IsFalse(postcode.Contains("#"));
            Assert.IsFalse(postcode.Contains("?"));
            Assert.AreEqual(1, Regex.Matches(postcode, DefaultRegex[nameof(Postcode_With_Default_Value)]).Count,
               $"Locale {Locale}: Incorrect value is: '{postcode}'. Regex used: '{DefaultRegex[nameof(Postcode_With_Default_Value)]}'");
            //Assert.GreaterOrEqual(Regex.Matches(postcode, "[0-9]").Count, 3);
         }
         catch
         {
            Assert.Fail($"Locale {Locale}");
         }
      }

      [Test]
      public void Postcode_For_Specific_US_State()
      {
         var postcode = Address.Postcode("ME");
         Assert.IsTrue(postcode.Length == 5 || postcode.Length == 10);
         Assert.IsTrue(postcode.StartsWith("042"));
      }

      [Test]
      public void Postcode_For_Invalid_State_Abbreviation()
      {
         var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Address.Postcode("ZZZ"));
         Assert.AreEqual("Must be one of the US state abbreviations or empty. (Parameter 'stateAbbreviation')", ex.Message);
      }

      [Test]
      public void Postcode_With_Null_As_Abbreviation()
      {
         var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Address.Postcode(null));
         Assert.AreEqual("Must be one of the US state abbreviations or empty. (Parameter 'stateAbbreviation')", ex.Message);
      }

      [Test]
      public void SecondaryAddress_HappyDays()
      {
         var secondaryAddress = Address.SecondaryAddress();
         Assert.IsFalse(string.IsNullOrWhiteSpace(secondaryAddress));
         Assert.IsFalse(secondaryAddress.Contains("#"));
         Assert.IsFalse(secondaryAddress.Contains("?"));
      }

      [Test]
      public void State_HappyDays()
      {
         var state = Address.State();
         Assert.IsFalse(string.IsNullOrWhiteSpace(state));
         Assert.IsFalse(state.Contains("#"));
         Assert.IsFalse(state.Contains("?"));
         var states = Fetch("address.state");
         Assert.IsTrue(states.Contains(state));
      }

      [Test]
      public void StateAbbr_HappyDays()
      {
         var stateAbbr = Address.StateAbbr();
         Assert.IsFalse(string.IsNullOrWhiteSpace(stateAbbr));
         Assert.IsFalse(stateAbbr.Contains("#"));
         Assert.IsFalse(stateAbbr.Contains("?"));
         var stateAbbrs = Fetch("address.state_abbr");
         Assert.IsTrue(stateAbbrs.Contains(stateAbbr));
      }

      [Test]
      public void StreetAddress_With_Default_Values_LocaleEn()
      {
         Config.Locale = "en";
         var streetAddress = Address.StreetAddress();
         Assert.IsFalse(string.IsNullOrWhiteSpace(streetAddress));
         Assert.IsFalse(streetAddress.Contains("#"));
         Assert.IsFalse(streetAddress.Contains("?"));
      }

      [Test]
      public void StreetAddress_With_Default_Values_LocaleRu()
      {
         Config.Locale = "ru";
         var streetAddress = Address.StreetAddress();
         Assert.IsFalse(string.IsNullOrWhiteSpace(streetAddress));
         Assert.IsFalse(streetAddress.Contains("#"));
         Assert.IsFalse(streetAddress.Contains("?"));
      }

      [Test]
      public void StreetAddress_With_SecondaryAddress_LocaleEn()
      {
         Config.Locale = "en";
         var streetAddress = Address.StreetAddress(true);
         Assert.IsFalse(streetAddress.Contains("#"));
         Assert.IsFalse(streetAddress.Contains("?"));
         var words = streetAddress.Split(' ');
         var suffixes = Fetch("address.street_suffix");
         var intersect = words.Intersect(suffixes);
         Assert.Greater(intersect.Count(), 0, $"words were '{string.Join("|", words)}' - suffixes were '{string.Join("|", suffixes.ToArray())}' - streetAddress was '{streetAddress}'");
      }

      [Test]
      public void StreetAddress_With_SecondaryAddress_LocaleRu()
      {
         Config.Locale = "ru";
         var streetAddress = Address.StreetAddress(true);
         Assert.IsFalse(streetAddress.Contains("#"));
         Assert.IsFalse(streetAddress.Contains("?"));
         var words = streetAddress.Split(' ', ',');
         var suffixes = Fetch("address.street_suffix");
         var intersect = words.Intersect(suffixes);
         Assert.Greater(intersect.Count(), 0, $"words were '{string.Join("|", words)}' - suffixes were '{string.Join("|", suffixes.ToArray())}' - streetAddress was '{streetAddress}'");
      }

      [Test]
      public void StreetAddress_With_SecondaryAddress_LocalePl()
      {
         Config.Locale = "pl";
         var streetAddress = Address.StreetAddress(true);
         Assert.IsFalse(streetAddress.Contains("#"));
         Assert.IsFalse(streetAddress.Contains("?"));
         var words = streetAddress.Split(' ');
         var suffixes = Fetch("address.street_prefix"); // pl has no 'street_suffix'
         var intersect = words.Intersect(suffixes);
         Assert.Greater(intersect.Count(), 0, $"words were '{string.Join("|", words)}' - suffixes were '{string.Join("|", suffixes.ToArray())}' - streetAddress was '{streetAddress}'");
      }

      [Test]
      public void StreetAddress_With_SecondaryAddress_LocaleDe()
      {
         Config.Locale = "de";
         var streetAddress = Address.StreetAddress(true);
         Assert.IsFalse(streetAddress.Contains("#"));
         Assert.IsFalse(streetAddress.Contains("?"));
         var words = streetAddress.Split(' ');
         var suffixes = new string[] { "Apt.", "Zimmer", "OG" }; // de has no 'street_suffix'
         var intersect = words.Intersect(suffixes);
         Assert.Greater(intersect.Count(), 0, $"words were '{string.Join("|", words)}' - suffixes were '{string.Join("|", suffixes.ToArray())}' - streetAddress was '{streetAddress}'");
      }

      [Test]
      public void StreetAddress_With_LocaleEn()
      {
         Config.Locale = "en";
         Address.StreetAddress(); // should not fail
      }

      [Test]
      public void StreetAddress_With_LocaleRu()
      {
         Config.Locale = "ru";
         Address.StreetAddress(); // should not fail
      }

      [Test]
      public void StreetName_With_LocaleEn()
      {
         Config.Locale = "en";
         var streetName = Address.StreetName();
         Assert.IsFalse(string.IsNullOrWhiteSpace(streetName));
         Assert.IsFalse(streetName.Contains("#"));
         Assert.IsFalse(streetName.Contains("?"));
      }

      [Test]
      public void StreetName_With_LocaleRu()
      {
         Config.Locale = "ru";
         var streetName = Address.StreetName();
         Assert.IsFalse(string.IsNullOrWhiteSpace(streetName));
         Assert.IsFalse(streetName.Contains("#"));
         Assert.IsFalse(streetName.Contains("?"));
      }

      [Test]
      public void StreetSuffix_HappyDays()
      {
         var suffix = Address.StreetSuffix();
         Assert.IsFalse(string.IsNullOrWhiteSpace(suffix));
         Assert.IsFalse(suffix.Contains("#"));
         Assert.IsFalse(suffix.Contains("?"));
         var suffixes = Fetch("address.street_suffix");
         Assert.IsTrue(suffixes.Contains(suffix));
      }

      [Test]
      public void TimeZone_HappyDays()
      {
         var timezone = Address.TimeZone();
         Assert.IsFalse(string.IsNullOrWhiteSpace(timezone));
         Assert.IsFalse(timezone.Contains("#"));
         Assert.IsFalse(timezone.Contains("?"));
      }

      [Test]
      public void Zip_With_Default_Value()
      {
         var zip = Address.Zip();
         Assert.IsFalse(string.IsNullOrWhiteSpace(zip));
         Assert.IsFalse(zip.Contains("#"));
         Assert.IsFalse(zip.Contains("?"));
         Assert.AreEqual(1, Regex.Matches(zip, DefaultRegex[nameof(Postcode_With_Default_Value)]).Count,
            $"Locale {Locale}. Value was {zip}. Regex used {DefaultRegex[nameof(Postcode_With_Default_Value)]}"
         );
         // Assert.GreaterOrEqual(Regex.Matches(zip, "[0-9]").Count, 3,
         //    $"Locale {Locale}"
         // );
      }

      [Test]
      public void Zip_For_Specific_US_State()
      {
         var zip = Address.Zip("ME");
         Assert.IsFalse(zip.Contains("#"));
         Assert.IsFalse(zip.Contains("?"));
         Assert.IsTrue(zip.StartsWith("042"));
      }

      [Test]
      public void Zip_For_Invalid_State_Abbreviation()
      {
         var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Address.Zip("ZZZ"));
         Assert.AreEqual("Must be one of the US state abbreviations or empty. (Parameter 'stateAbbreviation')", ex.Message);
      }

      [Test]
      public void Zip_With_Null_As_Abbreviation()
      {
         var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Address.Zip(null));
         Assert.AreEqual("Must be one of the US state abbreviations or empty. (Parameter 'stateAbbreviation')", ex.Message);
      }

      [Test]
      public void ZipCode_With_Default_Value()
      {
         var zipCode = Address.ZipCode();
         Assert.IsFalse(string.IsNullOrWhiteSpace(zipCode));
         Assert.IsFalse(zipCode.Contains("#"));
         Assert.IsFalse(zipCode.Contains("?"));
         Assert.AreEqual(1, Regex.Matches(zipCode, DefaultRegex[nameof(Postcode_With_Default_Value)]).Count,
            $"Locale {Locale}. Value was {zipCode}. Regex used {DefaultRegex[nameof(Postcode_With_Default_Value)]}"
         );
      }

      [Test]
      public void ZipCode_For_Specific_US_State()
      {
         var zipCode = Address.ZipCode("ME");
         Assert.IsFalse(zipCode.Contains("#"));
         Assert.IsFalse(zipCode.Contains("?"));
         Assert.IsTrue(zipCode.StartsWith("042"));
      }

      [Test]
      public void ZipCode_For_Invalid_State_Abbreviation()
      {
         var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Address.ZipCode("ZZZ"));
         Assert.AreEqual("Must be one of the US state abbreviations or empty. (Parameter 'stateAbbreviation')", ex.Message);
      }

      [Test]
      public void ZipCode_With_Null_As_Abbreviation()
      {
         var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Address.ZipCode(null));
         Assert.AreEqual("Must be one of the US state abbreviations or empty. (Parameter 'stateAbbreviation')", ex.Message);
      }
   }

   public static class AddressFixtureData
   {
      public static IEnumerable FixtureParams
      {
         get
         {
            yield return new TestFixtureData("ca", null);
            yield return new TestFixtureData("ca-CAT", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "( s/n.)|(, [0-9]{1,2})|( [0-9]{1,2})" }
            });
            yield return new TestFixtureData("da-DK", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,3}" },
               { nameof(AddressTests.CityPrefix_HappyDays), "[A-Z\u00d8][a-z\u00f8\u00e6]+" },
               { nameof(AddressTests.CitySuffix_HappyDays), "[a-z\u00f8\u00e6\u00e5]+" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-9]{4}$" }
            });
            yield return new TestFixtureData("de", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,3}|[0-9][0-9][abc]" },
               { nameof(AddressTests.CityPrefix_HappyDays), "[A-Z\u00c4\u00d6\u00dc][a-z\u00e4\u00f6\u00fc\u00df]+"}
            });
            yield return new TestFixtureData("de-AT", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,3}|[0-9][0-9][abc]" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-9]{4}$" }
            });
            yield return new TestFixtureData("bg", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,3}" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-9]{4}$" }
            });
            yield return new TestFixtureData("de-CH", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,3}|[0-9][0-9][abc]" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[1-9][0-9]{3}$" }
            });
            yield return new TestFixtureData("ee", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{2,4}" }
            });
            yield return new TestFixtureData("en", null);
            yield return new TestFixtureData("en-AU", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{2,4}" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-7][0-9]{3}$" }
            });
            yield return new TestFixtureData("en-au-ocker", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{2,4}" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-7][0-9]{3}$" }
            });
            yield return new TestFixtureData("en-BORK", null);
            yield return new TestFixtureData("en-CA", new Dictionary<string, string>{
               { nameof(AddressTests.Postcode_With_Default_Value), "^[A-CEGHJ-NPR-TVXY][0-9][A-CEJ-NPR-TV-Z] ?[0-9][A-CEJ-NPR-TV-Z][0-9]$" }
            });
            yield return new TestFixtureData("en-GB", new Dictionary<string, string>{
               { nameof(AddressTests.Postcode_With_Default_Value), "^[A-PR-UWYZ]([A-HK-Y][0-9][ABEHMNPRVWXY0-9]?|[0-9][ABCDEFGHJKPSTUW0-9]?) [0-9][ABD-HJLNP-UW-Z]{2}$" }
            });
            yield return new TestFixtureData("en-IND", new Dictionary<string, string>{
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-9]{6}$" }
            });
            yield return new TestFixtureData("en-MS", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{2}" }
            });
            yield return new TestFixtureData("en-NEP", null);
            yield return new TestFixtureData("en-NG", null);
            yield return new TestFixtureData("en-NZ", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "([0-9]{2,4})|([0-9]/[0-9]{1,2})|([0-9][a-z])|([a-z]/[0-9]{2,2}[a-z]{0,1})" },
               { nameof(AddressTests.Community_HappyDays), "([A-Z][a-z\u012b\u014d ]+){1,}" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-9]{4}$" }
            });
            yield return new TestFixtureData("en-PAK", null);
            yield return new TestFixtureData("en-SG", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,3}" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-9]{6}$" }
            });
            yield return new TestFixtureData("en-UG", null);
            yield return new TestFixtureData("en-US", null);
            yield return new TestFixtureData("en-ZA", null);
            yield return new TestFixtureData("es", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "( s/n.)|(, [0-9]{1,2})|( [0-9]{1,2})" },
               { nameof(AddressTests.CityPrefix_HappyDays), "[A-Z\u00c1\u00d3][a-z\u00e1\u00f3]+" }
            });
            yield return new TestFixtureData("es-MX", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "(S/N)|([0-9]{1,5})" },
               { nameof(AddressTests.CityPrefix_HappyDays), "^$" },
               { nameof(AddressTests.CitySuffix_HappyDays), "^$" }
            });
            yield return new TestFixtureData("fa", null);
            yield return new TestFixtureData("fi-FI", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,3}" }
            });
            yield return new TestFixtureData("fr", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,4}" }
            });
            yield return new TestFixtureData("fr-CA", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,4}" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[A-CEGHJ-NPR-TVXY][0-9][A-CEJ-NPR-TV-Z] ?[0-9][A-CEJ-NPR-TV-Z][0-9]$" }
            });
            yield return new TestFixtureData("fr-CH", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,4}" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[1-9][0-9]{3}$" }
            });
            yield return new TestFixtureData("he", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,2}" },
               { nameof(AddressTests.CityPrefix_HappyDays), "[A-Z\u0590-\u05fe]+" }
            });
            yield return new TestFixtureData("id", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{2}" }
            });
            yield return new TestFixtureData("it", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,3}" }
            });
            yield return new TestFixtureData("ja", new Dictionary<string, string>{
               { nameof(AddressTests.CityPrefix_HappyDays), "[一-龯]{1,}" },
               { nameof(AddressTests.CitySuffix_HappyDays), "[一-龯]{1,}" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-9]{3}-[0-9]{4}$" }
            });
            // All common and uncommon Kanji: ([一-龯])
            // Source: https://gist.github.com/terrancesnyder/1345094
            yield return new TestFixtureData("ko", new Dictionary<string, string>{
               //{ nameof(AddressTests.CityPrefix_HappyDays), "[一-龯]{1,}" },
               { nameof(AddressTests.CitySuffix_HappyDays), "([\uac00-\ud7af]|[\u1100-\u11ff]|[\u3130-\u318f]|[\ua960-\ua97f]|[\ud7b0-\ud7ff]){1,}" }
            });
            // Unicode range for hangul: [\uac00-\ud7af]|[\u1100-\u11ff]|[\u3130-\u318f]|[\ua960-\ua97f]|[\ud7b0-\ud7ff]
            // Source: https://stackoverflow.com/a/52989471/411428
            yield return new TestFixtureData("lv", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{2,4}" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^LV-[0-9]{4}$" }
            });
            yield return new TestFixtureData("nb-NO", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,2}" },
               { nameof(AddressTests.CitySuffix_HappyDays), "^[a-zøå]{1,}" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-9]{4}$" }
            });
            yield return new TestFixtureData("nl", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,3}[abc]?|[0-9]{3} [I]{1,3}" },
               { nameof(AddressTests.CitySuffix_HappyDays), "dam|berg| aan de Rijn| aan de IJssel|swaerd|endrecht|recht|ambacht|enmaes|wijk|sland|stroom|sluus|dijk|dorp|burg|veld|sluis|koop|lek|hout|geest|kerk|woude|hoven|hoten|ingen|plas|meer" },
               { nameof(AddressTests.Postcode_With_Default_Value), @"^\d{4} (AA|AB|AC|AD|AE|AF|AG|AH|AI|AJ|AK|AL|AM|AN|AO|AP|AQ|AR|AS|AT|AU|AV|AW|AX|AY|AZ|BA|BB|BC|BD|BE|BF|BG|BH|BI|BJ|BK|BL|BM|BN|BO|BP|BQ|BR|BS|BT|BU|BV|BW|BX|BY|BZ|CA|CB|CC|CD|CE|CF|CG|CH|CI|CJ|CK|CL|CM|CN|CO|CP|CQ|CR|CS|CT|CU|CV|CW|CX|CY|CZ|DA|DB|DC|DD|DE|DF|DG|DH|DI|DJ|DK|DL|DM|DN|DO|DP|DQ|DR|DS|DT|DU|DV|DW|DX|DY|DZ|EA|EB|EC|ED|EE|EF|EG|EH|EI|EJ|EK|EL|EM|EN|EO|EP|EQ|ER|ES|ET|EU|EV|EW|EX|EY|EZ|FA|FB|FC|FD|FE|FF|FG|FH|FI|FJ|FK|FL|FM|FN|FO|FP|FQ|FR|FS|FT|FU|FV|FW|FX|FY|FZ|GA|GB|GC|GD|GE|GF|GG|GH|GI|GJ|GK|GL|GM|GN|GO|GP|GQ|GR|GS|GT|GU|GV|GW|GX|GY|GZ|HA|HB|HC|HD|HE|HF|HG|HH|HI|HJ|HK|HL|HM|HN|HO|HP|HQ|HR|HS|HT|HU|HV|HW|HX|HY|HZ|IA|IB|IC|ID|IE|IF|IG|IH|II|IJ|IK|IL|IM|IN|IO|IP|IQ|IR|IS|IT|IU|IV|IW|IX|IY|IZ|JA|JB|JC|JD|JE|JF|JG|JH|JI|JJ|JK|JL|JM|JN|JO|JP|JQ|JR|JS|JT|JU|JV|JW|JX|JY|JZ|KA|KB|KC|KD|KE|KF|KG|KH|KI|KJ|KK|KL|KM|KN|KO|KP|KQ|KR|KS|KT|KU|KV|KW|KX|KY|KZ|LA|LB|LC|LD|LE|LF|LG|LH|LI|LJ|LK|LL|LM|LN|LO|LP|LQ|LR|LS|LT|LU|LV|LW|LX|LY|LZ|MA|MB|MC|MD|ME|MF|MG|MH|MI|MJ|MK|ML|MM|MN|MO|MP|MQ|MR|MS|MT|MU|MV|MW|MX|MY|MZ|NA|NB|NC|ND|NE|NF|NG|NH|NI|NJ|NK|NL|NM|NN|NO|NP|NQ|NR|NS|NT|NU|NV|NW|NX|NY|NZ|OA|OB|OC|OD|OE|OF|OG|OH|OI|OJ|OK|OL|OM|ON|OO|OP|OQ|OR|OS|OT|OU|OV|OW|OX|OY|OZ|PA|PB|PC|PD|PE|PF|PG|PH|PI|PJ|PK|PL|PM|PN|PO|PP|PQ|PR|PS|PT|PU|PV|PW|PX|PY|PZ|QA|QB|QC|QD|QE|QF|QG|QH|QI|QJ|QK|QL|QM|QN|QO|QP|QQ|QR|QS|QT|QU|QV|QW|QX|QY|QZ|RA|RB|RC|RD|RE|RF|RG|RH|RI|RJ|RK|RL|RM|RN|RO|RP|RQ|RR|RS|RT|RU|RV|RW|RX|RY|RZ|SB|SC|SE|SF|SG|SH|SI|SJ|SK|SL|SM|SN|SO|SP|SQ|SR|ST|SU|SV|SW|SX|SY|SZ|TA|TB|TC|TD|TE|TF|TG|TH|TI|TJ|TK|TL|TM|TN|TO|TP|TQ|TR|TS|TT|TU|TV|TW|TX|TY|TZ|UA|UB|UC|UD|UE|UF|UG|UH|UI|UJ|UK|UL|UM|UN|UO|UP|UQ|UR|US|UT|UU|UV|UW|UX|UY|UZ|VA|VB|VC|VD|VE|VF|VG|VH|VI|VJ|VK|VL|VM|VN|VO|VP|VQ|VR|VS|VT|VU|VV|VW|VX|VY|VZ|WA|WB|WC|WD|WE|WF|WG|WH|WI|WJ|WK|WL|WM|WN|WO|WP|WQ|WR|WS|WT|WU|WV|WW|WX|WY|WZ|XA|XB|XC|XD|XE|XF|XG|XH|XI|XJ|XK|XL|XM|XN|XO|XP|XQ|XR|XS|XT|XU|XV|XW|XX|XY|XZ|YA|YB|YC|YD|YE|YF|YG|YH|YI|YJ|YK|YL|YM|YN|YO|YP|YQ|YR|YS|YT|YU|YV|YW|YX|YY|YZ|ZA|ZB|ZC|ZD|ZE|ZF|ZG|ZH|ZI|ZJ|ZK|ZL|ZM|ZN|ZO|ZP|ZQ|ZR|ZS|ZT|ZU|ZV|ZW|ZX|ZY|ZZ)$" }
            });
            // Not testing locale 'no' since the file no.yml has an incorrect format at the time of writing.
            // Note that we won't fix the content as we take the file 'as-is' from https://github.com/faker-ruby/faker
            //yield return new TestFixtureData("no", null);
            yield return new TestFixtureData("pl", new Dictionary<string, string>{
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-9]{2}-[0-9]{3}$" }
            });
            yield return new TestFixtureData("pt", new Dictionary<string, string>{
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-9]{4}$" },
               { nameof(AddressTests.CityPrefix_HappyDays), "^$" },
               { nameof(AddressTests.CitySuffix_HappyDays), "^$" }
            });
            yield return new TestFixtureData("pt-BR", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{3,5}|s/n" },
               { nameof(AddressTests.CityPrefix_HappyDays), "^[A-Z][a-zí]+ ?[a-zí]+$" },
               { nameof(AddressTests.CitySuffix_HappyDays), "^ d[oe]{1} [DNS][a-z]+( Senhora)?$" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-9]{5}-[0-9]{3}$" }
            });
            yield return new TestFixtureData("ru", new Dictionary<string, string>{
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-9]{6}$" },
            });
            yield return new TestFixtureData("sk", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,3}" },
               { nameof(AddressTests.Postcode_With_Default_Value), "^[0-9]{5}|[0-9]{2} [0-9]{3}|[0-9]{3} [0-9]{2}$" },
            });
            yield return new TestFixtureData("sv", new Dictionary<string, string>{
               { nameof(AddressTests.BuildingNumber_HappyDays), "[0-9]{1,3}" },
               { nameof(AddressTests.CityPrefix_HappyDays), "^[A-ZÖ][a-zäö]+$" },
            });
            yield return new TestFixtureData("tr", null);
            // yield return new TestFixtureData("uk", null);
            // yield return new TestFixtureData("vi", null);
            // yield return new TestFixtureData("zh-CN", null);
            // yield return new TestFixtureData("zh-TW", null);
         }
      }
   }
}
