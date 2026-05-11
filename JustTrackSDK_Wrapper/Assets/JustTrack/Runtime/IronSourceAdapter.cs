using System;

/// <summary>
/// Interface for IronSource adapter implementations.
/// </summary>
public interface IronSourceAdapter
{
    /// <summary>
    /// Sets the impression handler for IronSource ad impressions.
    /// </summary>
    /// <param name="proxy">The impression handler proxy.</param>
    void SetIronSourceOnImpressionHandler(Action<object> proxy);
}
