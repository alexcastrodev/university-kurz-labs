using System.Runtime.ExceptionServices;

namespace ExceptionLab;

public static class Handlers
{
    public static Failure Classify(Action work)
    {
        try
        {
            work();
            return Failure.None;
        }
        catch (FileNotFoundException)
        {
            return Failure.NotFound;
        }
        catch (IOException)
        {
            return Failure.Io;
        }
        catch (RemoteException e) when (e.StatusCode >= 500)
        {
            return Failure.Transient;
        }
        catch (RemoteException)
        {
            return Failure.Permanent;
        }
        catch (Exception)
        {
            return Failure.Unexpected;
        }
    }

    public static void RunWithCleanup(Action work, List<string> log)
    {
        log.Add("start");
        try
        {
            work();
        }
        finally
        {
            log.Add("cleanup");
        }
    }

    public static void ObserveTwoPasses(Action work, List<string> log)
    {
        try
        {
            try
            {
                work();
            }
            finally
            {
                log.Add("finally");
            }
        }
        catch (Exception) when (Record(log, "filter"))
        {
            log.Add("catch");
        }
    }

    public static bool Record(List<string> log, string entry)
    {
        log.Add(entry);
        return true;
    }

    public static ExceptionDispatchInfo? TryCapture(Action work)
    {
        try
        {
            work();
            return null;
        }
        catch (Exception e)
        {
            return ExceptionDispatchInfo.Capture(e);
        }
    }

    public static IReadOnlyList<string> CollectMessages(Task all)
    {
        try
        {
            all.Wait();
            return [];
        }
        catch (AggregateException e)
        {
            return e.Flatten().InnerExceptions.Select(x => x.Message).ToList();
        }
    }
}
