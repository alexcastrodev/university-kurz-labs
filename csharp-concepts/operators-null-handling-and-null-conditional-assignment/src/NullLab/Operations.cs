namespace NullLab;

public static class Operations
{
    public static void Rename(Customer? customer, Func<string> newName)
    {
        // TODO-00: Assign customer.Name from newName() only when the customer exists, using the
        //   null-conditional assignment of C# 14. When customer is null, newName must not even run.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int? Birthday(Customer? customer)
    {
        // TODO-01: Add one year to the age with a compound null-conditional assignment and return the
        //   expression's value: the new age, or null when there is no customer.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static void EnsureNickname(Customer? customer, string fallback)
    {
        // TODO-02: Set the nickname to the fallback only when the customer exists AND the nickname is
        //   still null. Combine ?. with ??= in a single statement.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static void SetCustomerName(Order? order, string name)
    {
        // TODO-03: Set the name of the order's customer. Either the order or its customer may be
        //   null: the chain must stop silently at the first null.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static void Subscribe(Button? button, Action handler)
    {
        // TODO-04: Subscribe the handler to the button's Clicked event, only if there is a button.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static void Put(Dictionary<string, int>? map, string key, Func<int> value)
    {
        // TODO-05: Assign through the indexer only when the dictionary exists. value() must not run
        //   for a null dictionary.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool IsShort(int[]? numbers)
    {
        // TODO-06: A sequence is short when it has fewer than two elements and a missing one counts as
        //   empty. Beware: `numbers?.Length < 2` is false for null, because comparing a null int? is
        //   always false. Say what null means with ??.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int Total(int? bonus)
    {
        // TODO-07: Return the bonus (0 when null) plus 10. Beware of precedence: `bonus ?? 0 + 10`
        //   parses as `bonus ?? (0 + 10)`.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool BothValid(Func<bool> first, Func<bool> second)
    {
        // TODO-08: Return true when both checks pass, but always run BOTH of them (for example to
        //   collect every validation error). && would skip the second one: use the eager operator.
        throw new NotImplementedException("Not implemented yet.");
    }
}
