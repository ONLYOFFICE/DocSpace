namespace Textile;

/// <summary>
/// Interface through which the HTML formatted text
/// will be sent.
/// </summary>
/// Clients of the TextileFormatter class will have to provide
/// an outputter that implements this interface. Most of the
/// time, it'll be the WebForm itself.
public interface IOutputter
{
    /// <summary>
    /// Method called just before the formatted text
    /// is sent to the outputter.
    /// </summary>
    void Begin();

    /// <summary>
    /// Metohd called whenever the TextileFormatter wants to
    /// print some text.
    /// </summary>
    /// <param name="text">The formatted HTML text.</param>
    void Write(string text);
    /// <summary>
    /// Metohd called whenever the TextileFormatter wants to
    /// print some text. This should automatically print an
    /// additionnal end of line character.
    /// </summary>
    /// <param name="line">The formatted HTML text.</param>
    void WriteLine(string line);

    /// <summary>
    /// Method called at the end of the formatting.
    /// </summary>
    void End();
}
