using BookFlix.API.Services.Interfaces;
using Pgvector;
using Python.Runtime;

namespace BookFlix.API.Services
{
    public class PythonEmbeddingService : IEmbeddingService
    {
        public async Task<Vector> GetEmbeddingAsync(string text)
        {
            var floatArray = GetEmbeddingFromPython(text);
            if (floatArray.Count() != 384)
            {
                throw new Exception("The generated embedding is not of sufficient length.");
            }

            return new Vector(floatArray);
        }

        private float[] GetEmbeddingFromPython(string text)
        {
            return PythonEngineSingleton.Instance.RunWithGIL(() =>
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(Environment.GetEnvironmentVariable("PythonScriptsFolder"));
                dynamic embeddingModule = Py.Import("embedding_service");
                var embedding = embeddingModule.get_embedding(text);

                return ((PyObject)embedding).As<float[]>();
            });
        }
    }
}
