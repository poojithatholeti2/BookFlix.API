using System.Diagnostics;
using Python.Runtime;

namespace BookFlix.API
{
    public sealed class PythonEngineSingleton
    {
        private static readonly PythonEngineSingleton instance = new PythonEngineSingleton();
        public static PythonEngineSingleton Instance => instance;

        private PythonEngineSingleton()
        {
            Console.WriteLine("Initializing Python and its dependencies. This might take a moment. ");

            var stopwatch = Stopwatch.StartNew();

            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();

            //assigning it to discard operator, as embedding for 'warmup' can be ignored
            _ = RunWithGIL(() =>
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.append(Environment.GetEnvironmentVariable("PythonScriptsFolder"));
                    dynamic embeddingModule = Py.Import("embedding_service");
                    return embeddingModule.get_embedding("warmup"); //dummy call
                });

            stopwatch.Stop();
            Console.WriteLine($"Initialized Python and model warm-up time is: {stopwatch.ElapsedMilliseconds} ms");
        }
        
        // T is python object
        public T RunWithGIL<T>(Func<T> func)
        {
            //'using' releases threads used in GIL
            using (Py.GIL())
            {
                return func();
            }
        }

        public void Shutdown()
        {
            PythonEngine.Shutdown();
        }
    }
}
