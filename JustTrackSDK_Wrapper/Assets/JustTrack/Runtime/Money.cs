namespace JustTrack
{
    /// <summary>
    /// Represents a monetary value with currency information for the justtrack SDK.
    /// </summary>
    public class Money
    {
        /// <summary>
        /// Gets or sets the monetary value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Money"/> class.
        /// </summary>
        /// <param name="pValue">The monetary value.</param>
        /// <param name="pCurrency">The currency code.</param>
        public Money(double pValue, string pCurrency)
        {
            this.Value = pValue;
            this.Currency = pCurrency;
        }
    }
}
