namespace Paradise.DataAccess.Database.Interceptors.Base;

/// <summary>
/// Contains additional transaction information.
/// </summary>
public sealed class DbContextEventProperties
{
    #region Properties
    /// <summary>
    /// Transaction time.
    /// </summary>
    public DateTimeOffset TransactionTime { get; init; }
    #endregion
}