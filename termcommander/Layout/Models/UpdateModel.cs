namespace ConsoleApp.Layout.Models;
public class UpdateModel
{
	/// <summary>
	/// Set to true, if the other panel should be in focus now.
	/// Keep as false, if focus should remain on the current panel.
	/// </summary>
	public bool SwitchPanelFocus { get; set; } = false;

	/// <summary>
	/// Specifies the next focus direction in the current panel
	/// <br/>
	/// -1 for previous Window<br/>
	/// 0 for no focus change <br/>
	/// 1 for next Window<br/>
	/// </summary>
	public int SwitchViewFocus { get; set; } = 0;
}
