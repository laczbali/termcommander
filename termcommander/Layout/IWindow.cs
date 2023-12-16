using ConsoleApp.Layout.Models;

namespace ConsoleApp.Layout;
public interface IWindow
{
	/// <summary>
	/// When a user-action or event requires the update of a window,
	/// the top level <see cref="Container"/> will find the first
	/// leaf-window where <b>IsActive</b> is set,
	/// and will call its <see cref="Update"/> method
	/// <br/><br/>
	/// A window can be requested to be active or inactive,
	/// by setting the <b>IsActive field</b>.
	/// </summary>
	bool IsActive { get; set; }

	/// <summary>
	/// Called when a refresh of the window is needed.
	/// A key pressed by the user MAY be passed,
	/// but a refresh could be requested without a keypress (null) as well.
	/// <br/>
	/// Update could be called regardless of <see cref="IsActive"/>.
	/// </summary>
	UpdateModel Update(string? keyPressed);
}
