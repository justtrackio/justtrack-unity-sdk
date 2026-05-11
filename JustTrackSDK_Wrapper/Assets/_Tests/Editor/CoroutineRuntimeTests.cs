using NUnit.Framework;
using JustTrack;
using System.Collections;
using System.Collections.Generic;

namespace JustTrack.Tests.Editor
{
    /// <summary>
    /// Unit tests for the CoroutineRuntime class.
    /// Tests coroutine execution utilities for the Unity editor.
    /// </summary>
    public class CoroutineRuntimeTests
    {
        #region ToEnumerable Tests

        [Test]
        public void ToEnumerable_ReturnsNonNull()
        {
            // Arrange
            var enumerator = SimpleEnumerator();

            // Act
            var enumerable = CoroutineRuntime.ToEnumerable(enumerator);

            // Assert
            Assert.IsNotNull(enumerable);
        }

        [Test]
        public void ToEnumerable_ReturnsIEnumerable()
        {
            // Arrange
            var enumerator = SimpleEnumerator();

            // Act
            var enumerable = CoroutineRuntime.ToEnumerable(enumerator);

            // Assert
            Assert.IsInstanceOf<IEnumerable>(enumerable);
        }

        [Test]
        public void ToEnumerable_GetEnumerator_ReturnsSameEnumerator()
        {
            // Arrange
            var originalEnumerator = SimpleEnumerator();

            // Act
            var enumerable = CoroutineRuntime.ToEnumerable(originalEnumerator);
            var retrievedEnumerator = enumerable.GetEnumerator();

            // Assert
            Assert.AreSame(originalEnumerator, retrievedEnumerator);
        }

        [Test]
        public void ToEnumerable_CanIterateOverSimpleEnumerator()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };
            var enumerator = list.GetEnumerator();

            // Act
            var enumerable = CoroutineRuntime.ToEnumerable(enumerator);
            var result = new List<int>();

            foreach (var item in enumerable)
            {
                result.Add((int)item);
            }

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(3, result[2]);
        }

        #endregion

        #region StartCoroutine Tests

        [Test]
        public void StartCoroutine_AcceptsNonNullEnumerator()
        {
            // Arrange
            var enumerator = SimpleEnumerator();

            // Act & Assert - Should not throw
            Assert.DoesNotThrow(() => CoroutineRuntime.StartCoroutine(enumerator));
        }

        #endregion

        #region Helper Methods

        private IEnumerator SimpleEnumerator()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        #endregion
    }
}
