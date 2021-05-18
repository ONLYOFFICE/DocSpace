using System;
using System.Collections.Generic;

namespace JSIStudios.SimpleRESTServices.Core
{
    /// <summary>
    /// Provides the behavior for executing a callback method with configurable success
    /// values, number of retries, and the retry delay.
    /// </summary>
    /// <typeparam name="T">The operation return type</typeparam>
    /// <typeparam name="T2">The type of the value used to represent the operation's success or failure</typeparam>
#if NET35
    public interface IRetryLogic<T, T2>
#else
    public interface IRetryLogic<T, in T2>
#endif
    {
        /// <summary>
        /// Executes a user-defined operation with the specified number of retry attempts
        /// if a failure occurs and delay time between retry attempts.
        /// </summary>
        /// <param name="logic">The user-defined operation to execute.</param>
        /// <param name="retryCount">The number of times to retry a failed operation. This parameter is optional. The default value is 1.</param>
        /// <param name="retryDelay">The delay between retry operations. This parameter is optional. If the value is <c>null</c>, the default is <see cref="TimeSpan.Zero"/> (no delay).</param>
        /// <returns>Returns the result of a successful execution of <paramref name="logic"/>. If
        /// <paramref name="logic"/> failed and the maximum number of retries has been reached,
        /// the method returns the last (unsuccessful) result returned by <paramref name="logic"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="logic"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="retryCount"/> is less than zero.
        /// <para>-or-</para>
        /// <para>If <paramref name="retryDelay"/> is less than <see cref="TimeSpan.Zero"/>.</para>
        /// </exception>
        T Execute(Func<T> logic, int retryCount = 0, TimeSpan? retryDelay = null);

        /// <summary>
        /// Executes a user-defined operation with the specified "success" values, number of
        /// retry attempts if a failure occurs, and delay time between retry attempts.
        /// </summary>
        /// <param name="logic">The user-defined operation to execute.</param>
        /// <param name="successValues">A collection of values which are generally considered failures, but should be treated as success values for this call.</param>
        /// <param name="retryCount">The number of times to retry a failed operation. This parameter is optional. The default value is 1.</param>
        /// <param name="retryDelay">The delay between retry operations. This parameter is optional. If the value is <c>null</c>, the default is <see cref="TimeSpan.Zero"/> (no delay).</param>
        /// <returns>Returns the result of a successful execution of <paramref name="logic"/>. If
        /// <paramref name="logic"/> failed and the maximum number of retries has been reached,
        /// the method returns the last (unsuccessful) result returned by <paramref name="logic"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="logic"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="retryCount"/> is less than zero.
        /// <para>-or-</para>
        /// <para>If <paramref name="retryDelay"/> is less than <see cref="TimeSpan.Zero"/>.</para>
        /// </exception>
        T Execute(Func<T> logic, IEnumerable<T2> successValues, int retryCount = 0, TimeSpan? retryDelay = null);
    }
}
