// TODO: Async support
using System;
using System.Threading.Tasks;

namespace Hopefully
{
    /// <summary>
    /// Provides hopeful ways to execute arbitrary procedures.
    /// </summary>
    public static class Procedure
    {
        private static readonly TimeSpan defaultWaitTime = new TimeSpan(0,0,0);

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
        public static async Task RetryAsync(Action what, int attempts = 5)
        {
            int _;
            await Task.Run(() => Retry(what, out _, attempts));
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
        /// <param name="attempts">The number of attempts before giving up.</param>
        /// <param name="wait">The time to await between each attempt</param>
        /// <typeparam name="T">The return type of the procedure.</typeparam>
        public static async Task<T> RetryAsync<T>(Func<T> what, int attempts = 5, TimeSpan wait = default(TimeSpan))
        {
            int _;
            return await Task.Run<T>(() => Retry(what, out _, attempts, wait));
        }

        /// <summary>
        /// Retries a procedure up to a given number of attempts.
        /// </summary>
        /// <param name="what">The procedure to attempt.</param>
        /// <param name="failedAttempts">Set to the number of attempts that failed when retrying.</param>
        /// <param name="attempts">The number of attempts before giving up.</param>
        public static void Retry(Action what, out int failedAttempts, int attempts = 5, TimeSpan wait = default(TimeSpan))
        {
            failedAttempts = 0;
            while (attempts > 0)
            {
                try
                {
                    what();
                    return;
                }
                catch(Exception ex)
                {
                    attempts = FailedAttempt(ref failedAttempts, attempts, wait, ex);
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
        public static T Retry<T>(Func<T> what, out int failedAttempts, int attempts = 5, TimeSpan wait = default(TimeSpan))
        {
            failedAttempts = 0;
            while (attempts > 0)
            {
                try
                {
                    return what();
                }
                catch(Exception ex)
                {
                    attempts = FailedAttempt(ref failedAttempts, attempts, wait, ex);   
                }
            }
            return default(T); // Never reached
        }

        private static int FailedAttempt(ref int failedAttempts, int attempts, TimeSpan wait, Exception ex)
        {
            attempts--;
            failedAttempts++;
            if (attempts == 0)
                throw new ApplicationException("Reached the max number of attempts", ex);
            System.Threading.Thread.Sleep(wait);
            return attempts;
        }
    }
}