namespace Display.Models;

/// <summary>
/// It is recommended to use the CursesColor enum for the Foreground and Background properties,
/// and a custom enum (starting with 1) for the Id property.
/// </summary>
public interface IColorPair
{
    int Id { get; }
    short Foreground { get; }
    short Background { get; }
}
