using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using LetterQuizBot;
using Xunit;
using Xunit.Sdk;

namespace Test.LetterQuizBot
{
    public class DataStorageTest
    {

        public DataStorageTest()
        {
            Console.WriteLine("const");
            Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TestDirectory"));
        }

        [Fact]
        public static void AddUserPairShouldWork()
        {
            Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TestDirectory"));

            string adding_data = "JohnDoe#1234";
            DataStorage.AddUserPair(adding_data);
            Assert.True(DataStorage.GetUserData(adding_data).ContainsKey(adding_data));
        }
        [InlineData(0,10,10)]
        [InlineData(0,120,120)]
        [InlineData(0,20,20)]
        [InlineData(0,30,30)]
        [InlineData(0,40,40)]
        [Theory]
        public static void GetUniqueRandomArrayShouldWork(int min,int max,int size)
        {
            Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TestDirectory"));
            int[] a = DataStorage.GetUniqueRandomArray(min, max, size);
            Assert.True(a.Length == a.Distinct().Count());

        }


        [InlineData("광명역", "구명역", "해양,대한민국 철도역")]
        [Theory]
        public static void ChceckIfSynonymShouldWork(string userAns, string actualAns,string actualTheme)
        {
            Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TestDirectory"));
            Assert.True(DataStorage.ChceckIfSynonym(userAns,actualAns,actualTheme));
        }

        [Fact]
        public static void AddEntirePairToJsonShouldWork()
        {
            Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TestDirectory"));
            DataStorage.AddUserPair("JohnDoe#1234");
            DataStorage.SaveEntirePairsToJson();    
        }
        
        [InlineData("10","가톨릭")]
        [InlineData("100","군사")]
        [InlineData("130","논리")]
        [InlineData("230","생물")]
        [InlineData("ELW","엘소드")]
        [Theory]
        public static void GetThemeToSlangShouldWork(string expected,string theme)
        {
            Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TestDirectory"));
            string actual = DataStorage.themeToSlang[theme];
            Assert.Equal(expected,actual);
        }

        [Fact]
        public static void AddEntirePairToJsonShouldNotWork()
        {
            //assuming JohnDoe#1234 is already added 
            Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TestDirectory"));
            Assert.Throws<System.ArgumentException>(() => DataStorage.SaveEntirePairsToJson());
        }

        //[InlineData("LOL", "리그 오브 레전드")]
        //[Theory]
        //public static void GetThemeFromSlangShouldWork(string theme, string actual)
        //{
        //    Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TestDirectory"));
        //    List<string> expected = DataStorage.GetThemeFromSlangs(new string[]{theme});
        //    Assert.Equal(expected, actual);
        //}



        [Fact]
        public static void GetLangDataFromJasonShouldWork()
        {
            Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TestDirectory"));
            Assert.NotEmpty(DataStorage.GetLangDataFromJason());
        }

        [InlineData(Option.ANSWER)]
        [InlineData(Option.GENERATED)]
        [InlineData(Option.CORRECT_STREAK)]
        [Theory]
        public static void GetUserOptionValShouldWork(int op)
        {
            Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TestDirectory"));
            string nameTag = "JohnDoe#1234";
            Assert.NotNull(DataStorage.GetUserOptionVal(nameTag,op));
        }
        [Fact]
        public static void GetUserDataShouldWork()
        {
            //assuming JohnDoe#1234 is already added 
            Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TestDirectory"));
            string data = "JohnDoe#1234";
            Assert.IsType<Dictionary<string, dynamic>>(DataStorage.GetUserData(data));
        }

        [Fact]
        public static void FetchWordShouldWork()
        {
            Assert.NotEmpty(DataStorage.FetchWord("random"));
        }


        [Fact]
        public static void FetchColumnDataShouldWork()
        {
            Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("TestDirectory"));
            string current_theme = DataStorage.GetUserOptionVal("TechyTechy#1747", Option.SET_THEME);
            Dictionary<string, dynamic> query_data = DataStorage.FetchWord("random");
            string correctAnswer = query_data["_id"];

            string[] theme = query_data["theme"].Split(",");
        }
    }
}
