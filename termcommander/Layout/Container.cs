using ConsoleApp.Layout.Models;
using Mindmagma.Curses;
using System.Diagnostics;
using System.Text;

namespace ConsoleApp.Layout;

/// <summary>
/// Top-level container
/// </summary>
public abstract class Container : NcWindow
{
	public List<Panel> Panels { get; set; }

	private int lastActivePanelIndex = 0;
	private bool IsActive
	{
		get => Panels.Any(p => p.IsActive);
		set => Panels[lastActivePanelIndex].IsActive = value;
	}

	public Container(WindowSize size) : base(size)
	{
		var startupPanels = GetStartupPanels();
		if (startupPanels.left is null || startupPanels.right is null)
		{
			throw new InvalidOperationException($"{nameof(GetStartupPanels)} must return two non-null panels");
		}
		Panels = new() { startupPanels.left, startupPanels.right };
	}

	/// <summary>
	/// Panels to show upon application startup.
	/// MUST return two items.
	/// </summary>
	/// <returns></returns>
	protected abstract (Panel left, Panel right) GetStartupPanels();

	public void Start()
	{
		Update(null);

		string? keyPressed;
		do
		{
			var keyCode = NCurses.GetChar();
			keyPressed = GetKeyboardString(keyCode);

			if (keyPressed is not null)
			{
				Update(keyPressed);
			}
		}
		while (keyPressed != "esc");
	}

	private void Update(string? keyPressed)
	{
		if (!Panels.Any(p => p.IsActive))
		{
			Panels.First().IsActive = true;
		}

		if (keyPressed is null || keyPressed == string.Empty)
		{
			Panels.ForEach(p => p.Update(null));
			return;
		}

		// update right panel if it is active, default to left panel
		var activePanelIndex = Panels.FindIndex(v => v.IsActive);
		activePanelIndex = activePanelIndex > -1 ? activePanelIndex : 0;
		var activePanel = Panels[activePanelIndex];
		var panelResult = activePanel.Update(keyPressed);

		if (panelResult.SwitchPanelFocus)
		{
			// handle requested panel change
			var nextActivePanelIndex = 1 ^ activePanelIndex;
			var nextActivePanel = Panels[nextActivePanelIndex];
			Panels[lastActivePanelIndex].IsActive = false;
			Panels[nextActivePanelIndex].IsActive = true;
			lastActivePanelIndex = nextActivePanelIndex;
		}
	}

	private string? GetKeyboardString(int keyCode)
	{
		if (keyCode == -1)
		{
			return null;
		}

		if (keyCode > 32 && keyCode < 127)
		{
			// visible characters (letter, numbers, symbols)
			return Encoding.ASCII.GetString(new byte[] { (byte)keyCode });
		}

		var keyStr = keyCode switch
		{
			8 => "backspace",
			9 => "tab",
			10 => "enter",
			13 => "enter",
			27 => "esc",
			32 => "space",
			127 => "del",
			258 => "down",
			259 => "up",
			260 => "left",
			261 => "right",
			263 => "backspace",
			330 => "del",
			_ => null
		};

		if (keyStr is null)
		{
			Debug.WriteLine($"Failed to assign string value to keycode [{keyCode}]");
		}

		return keyStr;
	}
}
