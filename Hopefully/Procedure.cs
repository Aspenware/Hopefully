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
        /// <param name="wait">The amount of time to wait between failed attempts</param>
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
                    if(HandleFailedAttempt(ref failedAttempts, ref attempts, wait))
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
        /// <param name="wait">The amount of time to wait between failed attempts</param>
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
                    if (HandleFailedAttempt(ref failedAttempts, ref attempts, wait))
                        throw;
                }
            }
            return default(T); // Never reached
        }

        /// <summary>
        /// Handles the logic when one of the attempts failes
        /// </summary>
        /// <param name="failedAttempts">Number of failed attempts so far</param>
        /// <param name="attempts">Number of overall attempts so far</param>
        /// <param name="wait">The amount of time to wait between failed attempts</param>>
        /// <returns>Returns true if we have reached the maximun number of retries and should quit</returns>
        private static bool HandleFailedAttempt(ref int failedAttempts, ref int attempts, TimeSpan wait)
        {
            attempts--;
            failedAttempts++;
            if (attempts == 0) return true;
            if(wait.TotalMilliseconds != 0) System.Threading.Thread.Sleep(wait);
            return false;
        }
    }
}