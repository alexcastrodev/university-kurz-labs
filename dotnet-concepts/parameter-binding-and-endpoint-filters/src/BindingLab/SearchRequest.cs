namespace BindingLab;

// TODO-04: Bind each property from an explicit source: Category from the route,
// Page from the query string and Tenant from the X-Tenant header.
public record SearchRequest(string Category, int Page, string Tenant);
