using Avalonia.Controls;
using Avalonia.Layout;
using Ink.Data.Queries;

namespace Ink.DataGrid.Filtering;

/// <summary>
/// A filter editor for boolean columns. Renders three toggle buttons: Any / Yes / No.
/// </summary>
public sealed class BoolFilterEditor : IColumnFilterEditor
{
    private bool? _applied;
    private bool? _pending;

    public Control BuildControl()
    {
        _pending = _applied;

        var anyBtn  = MakeToggle("Any",  null);
        var yesBtn  = MakeToggle("Yes",  true);
        var noBtn   = MakeToggle("No",   false);

        SetActive(anyBtn, yesBtn, noBtn, _pending);

        anyBtn.Click += (_, _) => { _pending = null;  SetActive(anyBtn, yesBtn, noBtn, _pending); };
        yesBtn.Click += (_, _) => { _pending = true;  SetActive(anyBtn, yesBtn, noBtn, _pending); };
        noBtn.Click  += (_, _) => { _pending = false; SetActive(anyBtn, yesBtn, noBtn, _pending); };

        var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4 };
        panel.Children.Add(anyBtn);
        panel.Children.Add(yesBtn);
        panel.Children.Add(noBtn);
        return panel;
    }

    public FilterNode? BuildFilter(string fieldName) =>
        _pending is { } v
            ? new FilterCondition(fieldName, FilterOp.Equal, [v ? "true" : "false"])
            : null;

    public void SetFilter(FilterNode? filter)
    {
        _applied = filter is FilterCondition { Values: [string v] }
            ? string.Equals(v, "true", System.StringComparison.OrdinalIgnoreCase)
            : null;
        _pending = _applied;
    }

    private static Button MakeToggle(string label, bool? value)
    {
        var btn = new Button { Content = label };
        return btn;
    }

    private static void SetActive(Button anyBtn, Button yesBtn, Button noBtn, bool? active)
    {
        SetPrimary(anyBtn,  active is null);
        SetPrimary(yesBtn,  active == true);
        SetPrimary(noBtn,   active == false);
    }

    private static void SetPrimary(Button btn, bool isPrimary)
    {
        if (isPrimary) btn.Classes.Add("ink-primary");
        else btn.Classes.Remove("ink-primary");
    }
}
