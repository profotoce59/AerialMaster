using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Logger
{
    private readonly string _logFilePath;
    private readonly ConcurrentQueue<string> _logQueue;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Task _loggingTask;

    public Logger(string logFilePath)
    {
        _logFilePath = logFilePath;
        _logQueue = new ConcurrentQueue<string>();
        _cancellationTokenSource = new CancellationTokenSource();

        // Lancer la tâche asynchrone pour vider la file d'attente et écrire dans le fichier
        _loggingTask = Task.Run(() => ProcessLogQueue(_cancellationTokenSource.Token));
    }

    public void Log(string message)
    {
        // Ajouter le message à la file d'attente
        _logQueue.Enqueue(message);
    }

    private async Task ProcessLogQueue(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (_logQueue.TryDequeue(out string logMessage))
            {
                // Écrire le message dans le fichier
                using (StreamWriter writer = new StreamWriter(_logFilePath, true))
                {
                    await writer.WriteLineAsync(logMessage);
                }
            }

            // Ajouter un petit délai pour éviter une utilisation CPU excessive
            await Task.Delay(10);
        }
    }

    public void StopLogging()
    {
        // Arrêter la tâche de logging
        _cancellationTokenSource.Cancel();
        _loggingTask.Wait();
    }
}