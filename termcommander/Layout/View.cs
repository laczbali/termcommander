using ConsoleApp.Layout.Models;

namespace ConsoleApp.Layout;

/// <summary>
/// Has a single-responsibility of showing something (eg file list)
/// </summary>
public abstract class View : NcWindow, IWindow
{
	/// <summary>
	/// How many of the same view type can be shown in a single panel.
	/// Set to 0 (default) for no limit.
	/// </summary>
	public virtual int MaxCountPerPanel { get => 0; }

	private bool isActive = false;

	protected View(WindowSize size) : base(size)
	{
	}

	public bool IsActive
	{
		get => isActive;
		set
		{
			isActive = value;
			if (isActive)
			{
				FocusGained();
			}
			else
			{
				FocusLost();
			}
		}
	}

	protected abstract void FocusGained();
	public abstract void FocusLost();
	protected abstract UpdateModel UpdateInner(string? keyPressed);

	public UpdateModel Update(string? keyPressed)
	{
		return UpdateInner(keyPressed);
	}
}
