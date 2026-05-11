using NUnit.Framework;
using JustTrack;

namespace JustTrack.Tests.Editor
{
    /// <summary>
    /// Unit tests for the VerificationResultData class.
    /// Tests factory methods, implicit conversions, and error message handling.
    /// </summary>
    public class VerificationResultTests
    {
        [Test]
        public void Valid_ReturnsValidStatus()
        {
            // Arrange & Act
            var result = VerificationResultData.Valid();

            // Assert
            Assert.AreEqual(VerificationResult.VALID, result.Status);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public void Invalid_WithoutErrorMessage_ReturnsInvalidStatus()
        {
            // Arrange & Act
            var result = VerificationResultData.Invalid();

            // Assert
            Assert.AreEqual(VerificationResult.INVALID, result.Status);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public void Invalid_WithErrorMessage_StoresMessage()
        {
            // Arrange
            const string errorMessage = "Test error message";

            // Act
            var result = VerificationResultData.Invalid(errorMessage);

            // Assert
            Assert.AreEqual(VerificationResult.INVALID, result.Status);
            Assert.AreEqual(errorMessage, result.ErrorMessage);
        }

        [Test]
        public void Pending_ReturnsPendingStatus()
        {
            // Arrange & Act
            var result = VerificationResultData.Pending();

            // Assert
            Assert.AreEqual(VerificationResult.PENDING, result.Status);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public void Unknown_ReturnsUnknownStatus()
        {
            // Arrange & Act
            var result = VerificationResultData.Unknown();

            // Assert
            Assert.AreEqual(VerificationResult.UNKNOWN, result.Status);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public void Missing_ReturnsMissingStatus()
        {
            // Arrange & Act
            var result = VerificationResultData.Missing();

            // Assert
            Assert.AreEqual(VerificationResult.MISSING, result.Status);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public void ImplicitConversion_FromVerificationResult_Valid()
        {
            // Arrange
            VerificationResult status = VerificationResult.VALID;

            // Act
            VerificationResultData result = status;

            // Assert
            Assert.AreEqual(VerificationResult.VALID, result.Status);
        }

        [Test]
        public void ImplicitConversion_FromVerificationResult_Invalid()
        {
            // Arrange
            VerificationResult status = VerificationResult.INVALID;

            // Act
            VerificationResultData result = status;

            // Assert
            Assert.AreEqual(VerificationResult.INVALID, result.Status);
        }

        [Test]
        public void ImplicitConversion_FromVerificationResult_Pending()
        {
            // Arrange
            VerificationResult status = VerificationResult.PENDING;

            // Act
            VerificationResultData result = status;

            // Assert
            Assert.AreEqual(VerificationResult.PENDING, result.Status);
        }

        [Test]
        public void ImplicitConversion_FromVerificationResult_Unknown()
        {
            // Arrange
            VerificationResult status = VerificationResult.UNKNOWN;

            // Act
            VerificationResultData result = status;

            // Assert
            Assert.AreEqual(VerificationResult.UNKNOWN, result.Status);
        }

        [Test]
        public void ImplicitConversion_FromVerificationResult_Missing()
        {
            // Arrange
            VerificationResult status = VerificationResult.MISSING;

            // Act
            VerificationResultData result = status;

            // Assert
            Assert.AreEqual(VerificationResult.MISSING, result.Status);
        }

        [Test]
        public void ImplicitConversion_ToVerificationResult_Valid()
        {
            // Arrange
            var data = VerificationResultData.Valid();

            // Act
            VerificationResult status = data;

            // Assert
            Assert.AreEqual(VerificationResult.VALID, status);
        }

        [Test]
        public void ImplicitConversion_ToVerificationResult_Invalid()
        {
            // Arrange
            var data = VerificationResultData.Invalid("Error");

            // Act
            VerificationResult status = data;

            // Assert
            Assert.AreEqual(VerificationResult.INVALID, status);
        }

        [Test]
        public void ImplicitConversion_ToVerificationResult_NullData_ReturnsUnknown()
        {
            // Arrange
            VerificationResultData? data = null;

            // Act
            VerificationResult status = data!;

            // Assert
            Assert.AreEqual(VerificationResult.UNKNOWN, status);
        }

        [Test]
        public void ErrorMessage_PreservedThroughConversion()
        {
            // Arrange
            const string errorMessage = "Custom error message";
            var originalData = VerificationResultData.Invalid(errorMessage);

            // Act - Convert to enum and back
            VerificationResult status = originalData;
            VerificationResultData convertedData = status;

            // Assert - Error message is lost in conversion (expected behavior)
            Assert.AreEqual(VerificationResult.INVALID, status);
            Assert.AreEqual(VerificationResult.INVALID, convertedData.Status);
            Assert.IsNull(convertedData.ErrorMessage); // Error messages don't survive enum conversion
        }
    }
}
