using Cored.Extensions;
using Xunit;

namespace UnitTests.ArrayTests
{
    public class ArrayExtensionsTest
    {
        #region Happy Path

        [Theory]
        [InlineData("Test", "Home", 3, "First Test")]
        [InlineData("Test", "Away", 15, "Second Test")]
        [InlineData("Test", "Neutral", 33, "Third Test")]
        public void AppendToArray_ArrayNotEmpty(string origin, string filePath, int lineNumber, string message)
        {
            object[] args = { };

           args = args.Append(origin, filePath, lineNumber, message);

           // Asset that array contains the provided objects
            Assert.NotEmpty(args);

            //Then assert that the array contains specified object
            Assert.Contains("Test", args);

            Assert.Single(args, lineNumber);
        }

        [Theory]
        [InlineData("Test", "Home", 3, "First Test")]
        [InlineData("Test", "Away", 15, "Second Test")]
        [InlineData("Test", "Neutral", 33, "Third Test")]
        public void PrependToArray_ArrayContainsValue(string origin, string filePath, int lineNumber, string message)
        {
            object[] args = { };

           args = args.Prepend(origin, filePath, lineNumber, message);

           // Assert that array contains provided objects
           Assert.NotEmpty(args);

           // Then assert that the array contains specified object.
           Assert.Contains("Test", args);

           Assert.Single(args, lineNumber);
        }

        #endregion

        #region Error Path

        [Theory]
        [InlineData("Test", "Home", 3, "First Test")]
        [InlineData("Test", "Away", 15, "Second Test")]
        [InlineData("Test", "Neutral", 33, "Third Test")]
        public void AppendToArray_ArrayIsEmpty(string origin, string filePath, int lineNumber, string message)
        {
            object[] args = { };

            args = args.Append(origin, filePath, lineNumber, message);

            Assert.Empty(args);
        }

        [Theory]
        [InlineData("Test", "Home", 3, "First Test")]
        [InlineData("Test", "Away", 15, "Second Test")]
        [InlineData("Test", "Neutral", 33, "Third Test")]
        public void PrependToArray_ArrayIsEmpty(string origin, string filePath, int lineNumber, string message)
        {
            object[] args = { };

            args = args.Prepend(origin, filePath, lineNumber, message);

            Assert.Empty(args);
        }

        #endregion

        #region Edge Cases

        [Theory]
        [InlineData("Test", "Home", 3, "First Test")]
        [InlineData("Test", "Away", 15, "Second Test")]
        [InlineData("Test", "Neutral", 33, "Third Test")]
        public void PrependAndAppend_ArrayIsEqual(string origin, string filePath, int lineNumber, string message)
        {
            object[] append = { };

            object[] prepend = { };

            append = append.Append(origin, filePath, lineNumber, message);

            prepend = prepend.Prepend(message, lineNumber, filePath, origin);

            Assert.Equal(append, prepend);
        }

        #endregion
    }
}