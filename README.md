# Faker
RimuTec.Faker provides generators for fake, yet realistically looking data. Use it for testing, for creating screenshots to show off your cool software, and similar more. Generators include Lorem, Name, Address, Date, Company, Business, and many more.

RimuTec.Faker is a C# port of the Ruby Faker gem [https://github.com/stympy/faker](https://github.com/stympy/faker).

RimuTec.Faker targets .NET Standard 2.0 (netstandard2.0) and .NET Framework 4.6.2 (net462). The library including its source code are licensed under the MIT license. It supports 51 locales out of the box. And you can extend it with your own custom locales using yaml files.

| Metric      | Status      |
| ----- | ----- |
| Nuget       | [![NuGet Badge](https://buildstats.info/nuget/RimuTec.Faker)](https://www.nuget.org/packages/RimuTec.Faker/) |

# Usage

## Quick Start
1. Install NuGet package. See [https://www.nuget.org/packages/RimuTec.Faker](https://www.nuget.org/packages/RimuTec.Faker) for instructions
1. Add `using RimuTec.Faker;` at the beginning of your C# source file (or the equivalent for your preferred .NET language)
1. Generate fake data, e.g. `var firstName = Name.NameWithMiddle();` or `var paragraphs = Lorem.Paragraphs(4);`.

In case of name clashes with other classes in your code base, use one of [these techniques](https://github.com/RimuTec/Faker/wiki/Name-Clashes).

## Release Notes
Release notes are available at [https://github.com/RimuTec/Faker/blob/master/releasenotes.md](https://github.com/RimuTec/Faker/blob/master/releasenotes.md)

## Support & Suggestions
If you need support or have a suggestion for improvement please file an issue at [https://github.com/RimuTec/Faker/issues](https://github.com/RimuTec/Faker/issues). Thank you!

## Reporting Bugs
RimuTec.Faker has a test suite with about 270 unit tests. This does not guarantee absence of bugs. Please report all bugs at [https://github.com/RimuTec/Faker/issues](https://github.com/RimuTec/Faker/issues) ideally including steps to reproduce. We also consider pull requests (PR). All your feedback will help make the library more valuable for other users as well. Thank you!

# Available Fake Data Generators
The classes listed below are already ported. Our aim is to add the remaining classes and method over time. If you have preferences please file suggestions as issues on Github (see below). Thank you!

- Color
- [Address](https://github.com/RimuTec/Faker/wiki/Class-Address)
- [Business](https://github.com/RimuTec/Faker/wiki/Class-Business)
- [Code](https://github.com/RimuTec/Faker/wiki/Class-Code)
- [Company](https://github.com/RimuTec/Faker/wiki/Class-Company)
- [Date](https://github.com/RimuTec/Faker/wiki/Class-Date)
- [Educator](https://github.com/RimuTec/Faker/wiki/Class-Educator)
- [Finance](https://github.com/RimuTec/Faker/wiki/Class-Finance)
- [IdNumber](https://github.com/RimuTec/Faker/wiki/Class-IdNumber)
- [Internet](https://github.com/RimuTec/Faker/wiki/Class-Internet)
- [Job](https://github.com/RimuTec/Faker/wiki/Class-Job)
- [Lorem](https://github.com/RimuTec/Faker/wiki/Class-Lorem)
- [Name](https://github.com/RimuTec/Faker/wiki/Class-Name)
- [PhoneNumber](https://github.com/RimuTec/Faker/wiki/Class-PhoneNumber)
- [RandomNumber](https://github.com/RimuTec/Faker/wiki/Class-RandomNumber)

Class to set the locale to be used:
- [Config](https://github.com/RimuTec/Faker/wiki/Class-Config)

# Locales and Customization
The following 51 locales are supported out of the box (no extra files needed):

bg, ca, ca-CAT, da-DK, de, de-AT, de-CH, ee, en, en-AU, en-au-ocker, en-BORK, en-CA, en-GB, en-IND, en-MS, en-NEP, en-NG, en-NZ, en-PAK, en-SG, en-UG, en-US, en-ZA, es, es-MX, fa, fi-FI, fr, fr-CA, fr-CH, he, id, it, ja, ko, lv, nb-NO, nl, no, pl, pt, pt-BR, ru, sk, sv, tr, uk, vi, zh-CN, zh-TW

To set the locale use something like `Config.Locale = "de";`.

In addition you can use custom locale files for methods that are marked with an asterisk. Ensure that the custom locale file (yml) is copied to the directory that also contains RimuTec.Faker.dll, usually the output directory of your test project.

# How To Build
## Visual Studio 2017
Open Faker.sln in Visual Studio, select the desired configuration ("DEBUG" or "RELEASE") and then build the solution.

## Command Line
1. Open Powershell and navigate to directory containing Faker.sln
1. Execute the command `dotnet build --configuration RELEASE Faker.sln`. Replace RELEASE with DEBUG if you want build the DEBUG configuration

## Issues With Building
If you encounter issues with building the library please file an issue on GitHub, ideally with what you tried, what the expected and what the actual outcome was. Thank you!

# Credits
This project uses the yaml files from the [Ruby Faker gem](https://github.com/stympy/faker), licensed under the MIT license. Thank you to all their contributors!
