using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace JustTrack
{
    /// <summary>
    /// Provides coroutine execution capabilities in the Unity editor.
    /// </summary>
    [InitializeOnLoad]
    internal class CoroutineRuntime
    {
        private static List<IEnumerator> coroutinesInProgress = new List<IEnumerator>();
        private static int currentExecutingCoroutine = 0;

        // Allows us to run coroutines in the editor - however, the value yielded by the coroutine is not
        // interpreted. Thus, every yield just pauses until the next frame, not necessarily what was expected
        // from the yielded value (e.g., WaitForSeconds). This also means you can observe some things you
        // would not observe normally with Unity, e.g., a web request being in progress (the documentation
        // sounds like Unity pauses the coroutine until the request either failed or succeeded).

        /// <summary>
        /// Starts a coroutine for execution in the editor.
        /// </summary>
        /// <param name="newCoroutine">The coroutine to start.</param>
        internal static void StartCoroutine(IEnumerator newCoroutine)
        {
            coroutinesInProgress.Add(newCoroutine);
        }

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            EditorApplication.update += ExecuteCoroutine;
        }

        private static void ExecuteCoroutine()
        {
            if (coroutinesInProgress.Count <= 0)
            {
                return;
            }

            currentExecutingCoroutine = (currentExecutingCoroutine + 1) % coroutinesInProgress.Count;
            bool finish = !coroutinesInProgress[currentExecutingCoroutine].MoveNext();

            if (finish)
            {
                coroutinesInProgress.RemoveAt(currentExecutingCoroutine);
            }
        }

        /// <summary>
        /// Converts an enumerator to an enumerable.
        /// </summary>
        /// <param name="enumerator">The enumerator to convert.</param>
        /// <returns>An enumerable wrapper for the enumerator.</returns>
        internal static IEnumerable ToEnumerable(IEnumerator enumerator)
        {
            return new EnumeratorAdapter(enumerator);
        }

        private class EnumeratorAdapter : IEnumerable
        {
            private IEnumerator enumerator;

            internal EnumeratorAdapter(IEnumerator enumerator)
            {
                this.enumerator = enumerator;
            }

            public IEnumerator GetEnumerator()
            {
                return enumerator;
            }
        }
    }
}
