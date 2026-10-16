namespace NullLab;

public static class Operations
{
    public static void Rename(Customer? customer, Func<string> newName) =>
        customer?.Name = newName();

    public static int? Birthday(Customer? customer) =>
        customer?.Age += 1;

    public static void EnsureNickname(Customer? customer, string fallback) =>
        customer?.Nickname ??= fallback;

    public static void SetCustomerName(Order? order, string name) =>
        order?.Customer?.Name = name;

    public static void Subscribe(Button? button, Action handler) =>
        button?.Clicked += handler;

    public static void Put(Dictionary<string, int>? map, string key, Func<int> value) =>
        map?[key] = value();

    public static bool IsShort(int[]? numbers) =>
        (numbers?.Length ?? 0) < 2;

    public static int Total(int? bonus) =>
        (bonus ?? 0) + 10;

    public static bool BothValid(Func<bool> first, Func<bool> second) =>
        first() & second();

    public static void BirthdayOf(Func<Customer?> getCustomer) =>
        getCustomer()?.Age += 10;

    public static bool TryRename(Customer? customer, string name)
    {
        string? written = customer?.Name = name;
        return written is not null;
    }
}
