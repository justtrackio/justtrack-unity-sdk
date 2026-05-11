using System.Collections.Generic;
using System.Text;

namespace JustTrack
{
    /// <summary>
    /// Represents a user-generated event with associated data.
    /// </summary>
    public class AppEvent
    {
        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the collection of dimensions (key-value pairs) associated with this event.
        /// </summary>
        internal Dictionary<string, string> Dimensions { get; private set; }

        /// <summary>
        /// Gets the numeric value associated with this event.
        /// </summary>
        internal double Value { get; private set; }

        /// <summary>
        /// Gets the unit of measurement for the event value, if specified.
        /// </summary>
        internal Unit? Unit { get; private set; }

        /// <summary>
        /// Gets the currency code for monetary values, if specified.
        /// </summary>
        internal string? Currency { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppEvent"/> class with the specified name.
        /// </summary>
        /// <param name="pName">The name of the event.</param>
        public AppEvent(string pName)
        {
            this.Name = pName;
            this.Dimensions = new Dictionary<string, string>();
            this.Value = 0.0;
            this.Unit = null;
            this.Currency = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppEvent"/> class with the specified name, value, and unit.
        /// </summary>
        /// <param name="pName">The name of the event.</param>
        /// <param name="pValue">The numeric value of the event.</param>
        /// <param name="pUnit">The unit of measurement for the value.</param>
        public AppEvent(string pName, double pValue, Unit pUnit)
        {
            this.Name = pName;
            this.Dimensions = new Dictionary<string, string>();
            this.Value = pValue;
            this.Unit = pUnit;
            this.Currency = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppEvent"/> class with the specified name and monetary value.
        /// </summary>
        /// <param name="pName">The name of the event.</param>
        /// <param name="pMoney">The monetary value associated with the event.</param>
        public AppEvent(string pName, Money pMoney)
        {
            this.Name = pName;
            this.Dimensions = new Dictionary<string, string>();
            this.Value = pMoney.Value;
            this.Unit = null;
            this.Currency = pMoney.Currency;
        }

        /// <summary>
        /// Adds a dimension with the specified key and value to the event.
        /// </summary>
        /// <param name="pDimension">The key of the dimension.</param>
        /// <param name="pValue">The value of the dimension.</param>
        /// <returns>The current AppEvent instance for method chaining.</returns>
        public AppEvent AddDimension(string pDimension, string pValue)
        {
            if (!string.IsNullOrEmpty(pDimension))
            {
                if (string.IsNullOrEmpty(pValue))
                {
                    this.Dimensions.Remove(pDimension);
                }
                else
                {
                    this.Dimensions.Add(pDimension, pValue);
                }
            }

            return this;
        }

        /// <summary>
        /// Adds a dimension with the specified enum key and value to the event.
        /// </summary>
        /// <param name="pDimension">The enum key of the dimension.</param>
        /// <param name="pValue">The value of the dimension.</param>
        /// <returns>The current AppEvent instance for method chaining.</returns>
        public AppEvent AddDimension(Dimension pDimension, string pValue)
        {
            return this.AddDimension(DimensionConversions.DimensionToString(pDimension), pValue);
        }

        /// <summary>
        /// Removes a dimension with the specified key from the event.
        /// </summary>
        /// <param name="pDimension">The key of the dimension to remove.</param>
        /// <returns>The current AppEvent instance for method chaining.</returns>
        public AppEvent RemoveDimension(string pDimension)
        {
            if (!string.IsNullOrEmpty(pDimension))
            {
                this.Dimensions.Remove(pDimension);
            }

            return this;
        }

        /// <summary>
        /// Removes a dimension with the specified enum key from the event.
        /// </summary>
        /// <param name="pDimension">The enum key of the dimension to remove.</param>
        /// <returns>The current AppEvent instance for method chaining.</returns>
        public AppEvent RemoveDimension(Dimension pDimension)
        {
            return this.RemoveDimension(DimensionConversions.DimensionToString(pDimension));
        }

        /// <summary>
        /// Sets the value and unit of measurement for the event.
        /// </summary>
        /// <param name="pValue">The numeric value to set.</param>
        /// <param name="pUnit">The unit of measurement for the value.</param>
        /// <returns>The current AppEvent instance for method chaining.</returns>
        public AppEvent SetValue(double pValue, Unit pUnit)
        {
            this.Value = pValue;
            this.Unit = pUnit;
            this.Currency = null;

            return this;
        }

        /// <summary>
        /// Sets the monetary value for the event.
        /// </summary>
        /// <param name="pMoney">The monetary value to set.</param>
        /// <returns>The current AppEvent instance for method chaining.</returns>
        public AppEvent SetValue(Money pMoney)
        {
            this.Value = pMoney.Value;
            this.Unit = null;
            this.Currency = pMoney.Currency;

            return this;
        }

        /// <summary>
        /// Sets the count value for the event.
        /// </summary>
        /// <param name="pCount">The count value to set.</param>
        /// <returns>The current AppEvent instance for method chaining.</returns>
        public AppEvent SetCount(double pCount)
        {
            return this.SetValue(pCount, JustTrack.Unit.Count);
        }

        /// <summary>
        /// Sets the duration in seconds for the event.
        /// </summary>
        /// <param name="pSeconds">The duration in seconds to set.</param>
        /// <returns>The current AppEvent instance for method chaining.</returns>
        public AppEvent SetSeconds(double pSeconds)
        {
            return this.SetValue(pSeconds, JustTrack.Unit.Seconds);
        }

        /// <summary>
        /// Sets the duration in milliseconds for the event.
        /// </summary>
        /// <param name="pMilliseconds">The duration in milliseconds to set.</param>
        /// <returns>The current AppEvent instance for method chaining.</returns>
        public AppEvent SetMilliseconds(double pMilliseconds)
        {
            return this.SetValue(pMilliseconds, JustTrack.Unit.Milliseconds);
        }

#if UNITY_IOS
            internal string EncodeDimensions() {
                StringBuilder sb = new StringBuilder();
                sb.Append('{');
                bool first = true;
                foreach (var item in Dimensions) {
                    if (first) {
                        first = false;
                    } else {
                        sb.Append(',');
                    }
                    sb.Append($"\"{item.Key}\":\"");
                    foreach (var c in item.Value) {
                        if (c < ' ' || c == '"' || c == '\\' || c > '~') {
                            sb.Append("\\u").Append(((int) c).ToString("X4"));
                        } else {
                            sb.Append(c);
                        }
                    }
                    sb.Append('"');
                }
                sb.Append('}');

                return sb.ToString();
            }
#endif
    }
}