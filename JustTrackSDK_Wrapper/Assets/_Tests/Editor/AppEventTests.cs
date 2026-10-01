using NUnit.Framework;
using JustTrack;

namespace JustTrack.Tests.Editor
{
    public class AppEventTests
    {
        [Test]
        public void Constructor_SetsName()
        {
            var appEvent = new AppEvent("test_event");
            Assert.That(appEvent.Name, Is.EqualTo("test_event"));
        }

        [Test]
        public void Constructor_WithValueAndUnit_SetsName()
        {
            var appEvent = new AppEvent("test_event", 5.0, Unit.Count);
            Assert.That(appEvent.Name, Is.EqualTo("test_event"));
        }

        [Test]
        public void Constructor_WithMoney_SetsName()
        {
            var appEvent = new AppEvent("purchase", new Money(9.99, "USD"));
            Assert.That(appEvent.Name, Is.EqualTo("purchase"));
        }

        [Test]
        public void AddDimension_ReturnsSelf_ForChaining()
        {
            var appEvent = new AppEvent("test_event");
            var result = appEvent.AddDimension("key", "value");
            Assert.That(result, Is.SameAs(appEvent));
        }

        [Test]
        public void AddDimension_NullOrEmptyKey_DoesNotThrow()
        {
            var appEvent = new AppEvent("test_event");
            Assert.DoesNotThrow(() => appEvent.AddDimension("", "value"));
            Assert.DoesNotThrow(() => appEvent.AddDimension(null, "value"));
        }

        [Test]
        public void AddDimension_EmptyValue_DoesNotThrow()
        {
            var appEvent = new AppEvent("test_event");
            appEvent.AddDimension("key", "value");
            Assert.DoesNotThrow(() => appEvent.AddDimension("key", ""));
        }

        [Test]
        public void RemoveDimension_ReturnsSelf_ForChaining()
        {
            var appEvent = new AppEvent("test_event");
            appEvent.AddDimension("key", "value");
            var result = appEvent.RemoveDimension("key");
            Assert.That(result, Is.SameAs(appEvent));
        }

        [Test]
        public void RemoveDimension_NullOrEmptyKey_DoesNotThrow()
        {
            var appEvent = new AppEvent("test_event");
            Assert.DoesNotThrow(() => appEvent.RemoveDimension(""));
            Assert.DoesNotThrow(() => appEvent.RemoveDimension(null));
        }

        [Test]
        public void SetValue_WithUnit_ReturnsSelf()
        {
            var appEvent = new AppEvent("test_event");
            var result = appEvent.SetValue(42.0, Unit.Seconds);
            Assert.That(result, Is.SameAs(appEvent));
        }

        [Test]
        public void SetValue_WithMoney_ReturnsSelf()
        {
            var appEvent = new AppEvent("test_event");
            var result = appEvent.SetValue(new Money(1.5, "EUR"));
            Assert.That(result, Is.SameAs(appEvent));
        }

        [Test]
        public void SetCount_ReturnsSelf()
        {
            var appEvent = new AppEvent("test_event");
            var result = appEvent.SetCount(10);
            Assert.That(result, Is.SameAs(appEvent));
        }

        [Test]
        public void SetSeconds_ReturnsSelf()
        {
            var appEvent = new AppEvent("test_event");
            var result = appEvent.SetSeconds(3.5);
            Assert.That(result, Is.SameAs(appEvent));
        }

        [Test]
        public void SetMilliseconds_ReturnsSelf()
        {
            var appEvent = new AppEvent("test_event");
            var result = appEvent.SetMilliseconds(500);
            Assert.That(result, Is.SameAs(appEvent));
        }

        [Test]
        public void AddDimension_WithEnum_ReturnsSelf()
        {
            var appEvent = new AppEvent("test_event");
            var result = appEvent.AddDimension(Dimension.JT_CATEGORY, "test_value");
            Assert.That(result, Is.SameAs(appEvent));
        }

        [Test]
        public void RemoveDimension_WithEnum_ReturnsSelf()
        {
            var appEvent = new AppEvent("test_event");
            appEvent.AddDimension(Dimension.JT_CATEGORY, "test_value");
            var result = appEvent.RemoveDimension(Dimension.JT_CATEGORY);
            Assert.That(result, Is.SameAs(appEvent));
        }
    }
}
