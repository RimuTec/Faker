﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimuTec.Faker.Tests {
   [TestFixture]
   public class EducatorTests : FixtureBase {
      [Test]
      public void Course_Happy_Days() {
         // arrange

         // act
         var course = Educator.Course();

         // assert
         Assert.IsFalse(string.IsNullOrWhiteSpace(course));
         Assert.Greater(RegexMatchesCount(course, @" "), 0);
      }

      [Test]
      public void SecondarySchool_Happy_Days() {
         // arrange

         // act
         var secondarySchool = Educator.SecondarySchool();

         // assert
         Assert.IsFalse(string.IsNullOrWhiteSpace(secondarySchool));
         Assert.Greater(RegexMatchesCount(secondarySchool, @" "), 0);
      }

      [Test]
      public void University_Happy_Days() {
         // arrange

         // act
         var university = Educator.University();

         // assert
         Assert.IsFalse(string.IsNullOrWhiteSpace(university));
         Assert.Greater(RegexMatchesCount(university, @" "), 0);
      }
   }
}
