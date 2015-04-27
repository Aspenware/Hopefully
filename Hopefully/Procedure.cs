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
                    if (attempts == 0)
                        throw;
                }
            }
        }

        /// <summary>
        /// Retries a procedure up to a given number of attempts.
        /// </summary>
        /// <param name="what">The procedure to attempt.</param>
        /// <param name="attempts">The number of attempts before giving up.</param>
        /// <typeparam name="T">The return type of the procedure.</typeparam>
        public static T Retry<T>(Func<T> what, int attempts = 5)
        {
            while (attempts > 0)
            {
                try
                {
                    return what();
                }
                catch
                {
                    attempts--;
                    if (attempts == 0)
                        throw;
                }
            }
            return default(T); // Never reached
        }
    }
}