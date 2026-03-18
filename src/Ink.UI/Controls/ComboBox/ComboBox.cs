using Avalonia;

namespace Ink.UI.Controls;

public class ComboBox : Avalonia.Controls.ComboBox
{
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsDropDownOpenProperty)
        {
            if (change.GetNewValue<bool>())
                InkOverlay.Show(this);
            else
                InkOverlay.Hide(this);
        }
    }
}
