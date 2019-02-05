using System;
using System.Collections.Generic;
using System.Text;
using LetterQuizBot;
using Xunit;

namespace Test.LetterQuizBot
{
    public class KoreanLetterDividerTest
    {
        [Theory]
        [InlineData('끊','ㄲ')]
        [InlineData('끦','ㄲ')]
        [InlineData('꺊','ㄲ')]
        [InlineData('럃','ㄹ')]
        [InlineData('뺩','ㅃ')]
        [InlineData('샤','ㅅ')]
        [InlineData('쌰','ㅆ')]
        public static void DevideJasoShouldWork(char c,char expected)
        {
            char actual = KoreanLetterDivider.DevideJaso(c).chars[0];

            Assert.Equal(expected,actual);
        }

    }
}
