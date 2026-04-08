namespace Riverbed.Pricing.Paint.Shared.Services;

/// <summary>
/// Manages application theme (light/dark) state and notifies subscribers on change.
/// </summary>
public sealed class ThemeService
{
    private bool _isDarkMode;

    /// <summary>
    /// Gets or sets whether dark mode is active.
    /// Setting this property raises <see cref="OnChanged"/>.
    /// </summary>
    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (_isDarkMode == value)
                return;

            _isDarkMode = value;
            OnChanged?.Invoke(_isDarkMode);
        }
    }

    /// <summary>
    /// Raised when the dark mode state changes.
    /// The boolean parameter is <c>true</c> when dark mode is enabled.
    /// </summary>
    public event Action<bool>? OnChanged;
}
