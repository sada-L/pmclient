using Avalonia;
using Avalonia.Controls.Primitives;

namespace pmclient.Controls;

public class IconControl : TemplatedControl
{
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<IconControl, object?>(nameof(Icon));

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<IconControl, object?>(nameof(Header));

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly StyledProperty<bool> HeaderVisibleProperty =
        AvaloniaProperty.Register<IconControl, bool>(nameof(HeaderVisible), true);

    public bool HeaderVisible
    {
        get => GetValue(HeaderVisibleProperty);
        set => SetValue(HeaderVisibleProperty, value);
    }
}