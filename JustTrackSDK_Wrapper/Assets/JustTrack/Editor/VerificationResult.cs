namespace JustTrack
{
    /// <summary>
    /// Represents the result of a verification operation for the justtrack SDK configuration.
    /// </summary>
    internal enum VerificationResult
    {
        VALID,
        INVALID,
        PENDING,
        UNKNOWN,
        MISSING,
    }

    /// <summary>
    /// The status of the verification result.
    /// </summary>
    internal class VerificationResultData
    {
        /// <summary>
        /// Gets the status of the verification result.
        /// </summary>
        public VerificationResult Status { get; private set; }

        /// <summary>
        /// Gets error message in case the verification result is INVALID.
        /// </summary>
        public string? ErrorMessage { get; private set; }

        private VerificationResultData(VerificationResult status, string? errorMessage = null)
        {
            Status = status;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Creates a verification result indicating valid status.
        /// </summary>
        /// <returns>A new VerificationResultData with valid status.</returns>
        public static VerificationResultData Valid() => new VerificationResultData(VerificationResult.VALID);

        /// <summary>
        /// Creates a verification result indicating invalid status.
        /// </summary>
        /// <param name="errorMessage">Optional error message describing why validation failed.</param>
        /// <returns>A new VerificationResultData with invalid status.</returns>
        public static VerificationResultData Invalid(string? errorMessage = null) => new VerificationResultData(VerificationResult.INVALID, errorMessage);

        /// <summary>
        /// Creates a verification result indicating pending status.
        /// </summary>
        /// <returns>A new VerificationResultData with pending status.</returns>
        public static VerificationResultData Pending() => new VerificationResultData(VerificationResult.PENDING);

        /// <summary>
        /// Creates a verification result indicating unknown status.
        /// </summary>
        /// <returns>A new VerificationResultData with unknown status.</returns>
        public static VerificationResultData Unknown() => new VerificationResultData(VerificationResult.UNKNOWN);

        /// <summary>
        /// Creates a verification result indicating missing status.
        /// </summary>
        /// <returns>A new VerificationResultData with missing status.</returns>
        public static VerificationResultData Missing() => new VerificationResultData(VerificationResult.MISSING);

        /// <summary>
        /// Implicitly converts a VerificationResult to VerificationResultData.
        /// </summary>
        /// <param name="status">The verification result status to convert.</param>
        public static implicit operator VerificationResultData(VerificationResult status)
        {
            switch (status)
            {
                case VerificationResult.VALID:
                    return Valid();
                case VerificationResult.INVALID:
                    return Invalid();
                case VerificationResult.PENDING:
                    return Pending();
                case VerificationResult.UNKNOWN:
                    return Unknown();
                case VerificationResult.MISSING:
                    return Missing();
                default:
                    return Unknown();
            }
        }

        /// <summary>
        /// Implicitly converts VerificationResultData to a VerificationResult.
        /// </summary>
        /// <param name="data">The verification result data to convert.</param>
        public static implicit operator VerificationResult(VerificationResultData data)
        {
            return data?.Status ?? VerificationResult.UNKNOWN;
        }
    }
}
