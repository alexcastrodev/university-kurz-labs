using System.Runtime.ExceptionServices;

namespace ExceptionLab;

public static class Handlers
{
    public static Failure Classify(Action work)
    {
        // TODO-00: Run the work and map what it throws to a Failure, using several catch clauses:
        //   nothing thrown                      -> None
        //   FileNotFoundException               -> NotFound
        //   any other IOException               -> Io (order the clauses: the specific one first)
        //   RemoteException with StatusCode>=500 -> Transient (use an exception filter, `when`)
        //   any other RemoteException           -> Permanent
        //   anything else                       -> Unexpected
        throw new NotImplementedException("Not implemented yet.");
    }

    public static void RunWithCleanup(Action work, List<string> log)
    {
        // TODO-01: Add "start" to the log, run the work, and add "cleanup" to the log whether the
        //   work succeeds or throws. The exception must still reach the caller (do not catch it).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static void ObserveTwoPasses(Action work, List<string> log)
    {
        // TODO-02: Wrap the work in an inner try/finally that logs "finally", and an outer
        //   `catch (Exception) when (Record(log, "filter"))` that logs "catch". Record is already
        //   written for you. Run the tests and look at the order of the log: filters run before
        //   the inner finally blocks, because the runtime searches for a handler first (pass 1)
        //   and only then unwinds (pass 2).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool Record(List<string> log, string entry)
    {
        log.Add(entry);
        return true;
    }

    public static ExceptionDispatchInfo? TryCapture(Action work)
    {
        // TODO-03: Run the work. If it throws, return ExceptionDispatchInfo.Capture(exception) so
        //   the exception can be rethrown later (info.Throw()) without losing its original stack
        //   trace. Return null when nothing was thrown.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static IReadOnlyList<string> CollectMessages(Task all)
    {
        // TODO-04: Block on the task with Wait(). When it faults, you get an AggregateException:
        //   flatten it and return the message of every inner exception. Return an empty list when
        //   the task succeeds.
        throw new NotImplementedException("Not implemented yet.");
    }
}
