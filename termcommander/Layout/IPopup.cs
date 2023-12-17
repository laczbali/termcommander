namespace ConsoleApp.Layout;
public interface IPopup
{
	static int MinCols { get; }
	static int MinRows { get; }

	void Show();
}
