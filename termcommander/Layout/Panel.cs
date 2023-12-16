using ConsoleApp.Layout.Models;

namespace ConsoleApp.Layout;

/// <summary>
/// Left or right part of the screeen.
/// Can display one or more views.
/// </summary>
public abstract class Panel : NcWindow, IWindow
{
	/// <summary>
	/// Views showed by the panel. Order is top-to-bottom.
	/// </summary>
	public List<View> Views { get; set; }

	private int lastActiveViewIndex = 0;
	public bool IsActive
	{
		get => Views.Any(v => v.IsActive);
		set => Views[lastActiveViewIndex].IsActive = value;
	}

	public Panel(WindowSize size) : base(size)
	{
		var startupViews = SetStartupViews();
		if (startupViews?.Any() != true)
		{
			throw new InvalidOperationException($"{nameof(SetStartupViews)} must return at least one item");
		}
		Views = new();
		Views.AddRange(startupViews);
	}

	/// <summary>
	/// Which view(s) to show upon application startup.
	/// MUST return at least one item.
	/// Order is top-to-bottom.
	/// </summary>
	/// <returns></returns>
	public abstract List<View> SetStartupViews();

	public UpdateModel Update(string? keyPressed)
	{
		if (keyPressed is null || keyPressed == string.Empty)
		{
			Views.ForEach(v => v.Update(null));
			// we don't know what happened, so no change is needed from this level
			return new UpdateModel();
		}

		// find and update current view
		var activeViewIndex = Views.FindIndex(v => v.IsActive);
		activeViewIndex = activeViewIndex > -1 ? activeViewIndex : 0;
		var activeView = Views[activeViewIndex];
		var viewResult = activeView.Update(keyPressed);

		if (viewResult.SwitchPanelFocus)
		{
			// panel change requested, push the request up
			return viewResult;
		}

		if (viewResult.SwitchViewFocus != 0)
		{
			// handle requested view change
			var nextActiveViewIndex = activeViewIndex + viewResult.SwitchViewFocus;
			nextActiveViewIndex = nextActiveViewIndex > -1 ? nextActiveViewIndex : Views.Count - 1;
			nextActiveViewIndex = nextActiveViewIndex < Views.Count ? nextActiveViewIndex : 0;

			var nextActiveView = Views[nextActiveViewIndex];
			activeView.IsActive = false;
			nextActiveView.IsActive = true;
			lastActiveViewIndex = nextActiveViewIndex;
		}

		// we already handled the view change (if it was needed)
		return new UpdateModel();
	}
}
