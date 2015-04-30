// TODO: Async support
using System;

namespace Hopefully
{
    /// <summary>
    /// Provides hopeful ways to execute arbitrary procedures.
    /// </summary>
    public static class Procedure
    {
        /// <summary>
        /// Retries a procedure up to a given number of attempts.
        /// </summary>
        /// <param name="what">The procedure to attempt.</param>
        /// <param name="attempts">The number of attempts before giving up.</param>
        public static void Retry(Action what, int attempts = 5)
        {
            int _;
            Retry(what, out _, attempts);
        }

        /// <summary>
        /// Retries a procedure up to a given number of attempts.
        /// </summary>
        /// <param name="what">The procedure to attempt.</param>
        /// <param name="attempts">The number of attempts before giving up.</param>
        /// <typeparam name="T">The return type of the procedure.</typeparam>
        public static T Retry<T>(Func<T> what, int attempts = 5)
        {
            int _;
            return Retry<T>(what, out _, attempts);
        }

        /// <summary>
        /// Retries a procedure up to a given number of attempts.
        /// </summary>
        /// <param name="what">The procedure to attempt.</param>
        /// <param name="failedAttempts">Set to the number of attempts that failed when retrying.</param>
        /// <param name="attempts">The number of attempts before giving up.</param>
        public static void Retry(Action what, out int failedAttempts, int attempts = 5)
        {
            failedAttempts = 0;
            while (attempts > 0)
            {
                try
                {
                    what();
                    return;
                }
                catch
                {
                    attempts--;
                    failedAttempts++;
                    if (attempts == 0)
                        throw;
                }
            }
        }

        /// <summary>
        /// Retries a procedure up to a given number of attempts.
        /// </summary>
        /// <param name="what">The procedure to attempt.</param>
        /// <param name="failedAttempts">Set to the number of attempts that failed when retrying.</param>
        /// <param name="attempts">The number of attempts before giving up.</param>
        /// <typeparam name="T">The return type of the procedure.</typeparam>
        public static T Retry<T>(Func<T> what, out int failedAttempts, int attempts = 5)
        {
            failedAttempts = 0;
            while (attempts > 0)
            {
                try
                {
                    return what();
                }
                catch
                {
                    attempts--;
                    failedAttempts++;
                    if (attempts == 0)
                        throw;
                }
            }
            return default(T); // Never reached
        }
    }
}