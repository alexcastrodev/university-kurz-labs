using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Bunit;
using BlazorFormsLab;

namespace BlazorFormsLab.Tests;

public class FormsTests : BunitContext
{
    public FormsTests()
    {
        // Pin the culture: InputNumber parses with the current culture.
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
    }

    private static List<ValidationResult> Run(Signup s)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(s, new ValidationContext(s), results, validateAllProperties: true);
        return results;
    }

    private static string[] Messages(IEnumerable<ValidationResult> r) => r.Select(x => x.ErrorMessage!).ToArray();

    // ---- TODO-00: annotations and IValidatableObject on Signup ----

    [Fact]
    public void Name_is_required_and_between_3_and_10_chars()
    {
        Assert.Contains(Run(new Signup { Name = null }), r => r.MemberNames.Contains("Name"));
        Assert.Contains(Run(new Signup { Name = "ab" }), r => r.MemberNames.Contains("Name"));
        Assert.Contains(Run(new Signup { Name = "abcdefghijk" }), r => r.MemberNames.Contains("Name"));
        Assert.DoesNotContain(Run(new Signup { Name = "alice", Terms = true }), r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Age_must_be_18_to_120_and_email_must_look_like_an_email()
    {
        Assert.Contains(Run(new Signup { Name = "alice", Age = 17 }), r => r.MemberNames.Contains("Age"));
        Assert.Contains(Run(new Signup { Name = "alice", Age = 121 }), r => r.MemberNames.Contains("Age"));
        Assert.Contains(Run(new Signup { Name = "alice", Email = "nope" }), r => r.MemberNames.Contains("Email"));
        Assert.Empty(Run(new Signup { Name = "alice", Age = 30, Email = "a@b.com", Terms = true }));
    }

    [Fact]
    public void Admin_is_reserved_by_the_model_level_rule()
    {
        var r = Run(new Signup { Name = "AdMin", Terms = true });
        var single = Assert.Single(r);
        Assert.Equal("admin is reserved.", single.ErrorMessage);
        Assert.Contains("Name", single.MemberNames);
    }

    // ---- TODO-01: EditForm wiring (validator + OnValidSubmit/OnInvalidSubmit) ----

    private (IRenderedComponent<SignupForm> cut, List<Signup> saved, int[] rejected) RenderSignup()
    {
        var saved = new List<Signup>();
        var rejected = new[] { 0 };
        var cut = Render<SignupForm>(p => p
            .Add(c => c.OnSaved, (Signup s) => saved.Add(s))
            .Add(c => c.OnRejected, () => { rejected[0]++; }));
        return (cut, saved, rejected);
    }

    private static void FillValid(IRenderedComponent<SignupForm> cut)
    {
        cut.Find("#name").Change("alice");
        cut.Find("#age").Change("30");
        cut.Find("#terms").Change(true);
    }

    [Fact]
    public void Invalid_submit_shows_messages_and_raises_OnRejected_only()
    {
        var (cut, saved, rejected) = RenderSignup();

        cut.Find("form").Submit();

        Assert.Equal(1, rejected[0]);
        Assert.Empty(saved);
        Assert.Contains("The Name field is required.", cut.Find(".validation-errors").TextContent);
        Assert.NotEmpty(cut.FindAll(".validation-errors li"));
        Assert.Contains(cut.FindAll("div.validation-message"), d => d.TextContent.Contains("The Name field is required."));
    }

    [Fact]
    public void Valid_submit_raises_OnSaved_with_the_typed_values_from_every_input_type()
    {
        var (cut, saved, rejected) = RenderSignup();
        FillValid(cut);
        cut.Find("#plan").Change("pro");

        cut.Find("form").Submit();

        Assert.Equal(0, rejected[0]);
        var s = Assert.Single(saved);
        Assert.Equal("alice", s.Name);
        Assert.Equal(30, s.Age);
        Assert.Equal("pro", s.Plan);
        Assert.True(s.Terms);
    }

    [Fact]
    public void Editing_a_field_runs_only_its_property_rules_and_submit_runs_the_model_rule()
    {
        var (cut, _, _) = RenderSignup();

        cut.Find("#name").Change("admin");
        Assert.DoesNotContain("admin is reserved.", cut.Markup);

        // IValidatableObject only runs when every property rule passes, so accept the terms first.
        cut.Find("#terms").Change(true);
        cut.Find("form").Submit();
        Assert.Contains("admin is reserved.", cut.Markup);
    }

    [Fact]
    public void Typing_text_in_a_number_input_is_a_binding_error_and_keeps_the_old_value()
    {
        var (cut, saved, _) = RenderSignup();
        FillValid(cut);

        cut.Find("#age").Change("abc");
        Assert.Contains("The Age field must be a number.", cut.Markup);

        cut.Find("form").Submit();
        Assert.Empty(saved);
    }

    [Fact]
    public void OnSubmit_cannot_be_combined_with_OnValidSubmit()
    {
        // Documents framework behaviour: this test passes in the starter too.
        Assert.ThrowsAny<Exception>(() => Render<Microsoft.AspNetCore.Components.Forms.EditForm>(p => p
            .Add(c => c.Model, new Signup())
            .Add(c => c.OnSubmit, (Microsoft.AspNetCore.Components.Forms.EditContext _) => { })
            .Add(c => c.OnValidSubmit, (Microsoft.AspNetCore.Components.Forms.EditContext _) => { })));
    }

    // ---- TODO-02: custom ValidationAttribute ----

    [Fact]
    public void NoBannedWords_rejects_banned_names_ignoring_case_and_reports_the_member()
    {
        var model = new UsernameModel { UserName = "ROOT" };
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);

        var r = Assert.Single(results);
        Assert.Equal("ROOT is banned.", r.ErrorMessage);
        Assert.Contains("UserName", r.MemberNames);

        model.UserName = "bob";
        results.Clear();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), results, true));
    }

    [Fact]
    public void Banned_word_message_appears_as_soon_as_the_field_changes()
    {
        var cut = Render<UsernameForm>();
        cut.Find("#username").Change("root");
        Assert.Contains("root is banned.", cut.Markup);
    }

    // ---- TODO-03: server-side errors with ValidationMessageStore ----

    [Fact]
    public void Taken_name_gets_a_store_message_and_editing_the_field_clears_it()
    {
        var saved = new List<string>();
        var cut = Render<UsernameForm>(p => p
            .Add(c => c.IsTaken, (string n) => n == "ana")
            .Add(c => c.OnSaved, (string n) => saved.Add(n)));

        cut.Find("#username").Change("ana");
        cut.Find("form").Submit();
        Assert.Contains("ana is already taken.", cut.Markup);
        Assert.Empty(saved);

        cut.Find("#username").Change("bob");
        Assert.DoesNotContain("already taken", cut.Markup);

        cut.Find("form").Submit();
        Assert.Equal(["bob"], saved);
    }

    // ---- TODO-04: field CSS classes ----

    [Fact]
    public void Default_pristine_field_has_the_pristine_class()
    {
        var cut = Render<SignupForm>();
        Assert.Equal("pristine", cut.Find("#name").ClassName);
    }

    [Fact]
    public void Custom_provider_marks_touched_ok_and_touched_bad()
    {
        var cut = Render<SignupForm>();

        cut.Find("#name").Change("ab");
        Assert.Equal("touched bad", cut.Find("#name").ClassName);

        cut.Find("#name").Change("alice");
        Assert.Equal("touched ok", cut.Find("#name").ClassName);
    }

    // ---- Bonus ----

    [Fact, Trait("Category", "Bonus")]
    public void Bonus_05_unsaved_marker_shows_after_edit_and_disappears_after_a_valid_save()
    {
        var (cut, saved, _) = RenderSignup();
        Assert.Empty(cut.FindAll(".dirty"));

        FillValid(cut);
        Assert.Single(cut.FindAll(".dirty"));

        cut.Find("form").Submit();
        Assert.Single(saved);
        Assert.Empty(cut.FindAll(".dirty"));
    }

    [Fact, Trait("Category", "Bonus")]
    public void Bonus_06_terms_must_be_accepted()
    {
        var (cut, saved, rejected) = RenderSignup();
        cut.Find("#name").Change("alice");

        cut.Find("form").Submit();

        Assert.Equal(1, rejected[0]);
        Assert.Empty(saved);
        Assert.Contains("You must accept the terms.", cut.Markup);
    }
}
