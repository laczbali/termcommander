namespace Display.Models;
public class WindowSize
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public int RowOrigin { get; set; }
    public int ColumnsOrigin { get; set; }

    public WindowSize(int rows, int columns, int rowOrigin, int columnsOrigin)
    {
        Rows = rows;
        Columns = columns;
        RowOrigin = rowOrigin;
        ColumnsOrigin = columnsOrigin;
    }

    public int GetHorizontalCenter(int offset = 0) => (Columns / 2) + offset;
    public int GetVerticalCenter(int offset = 0) => (Rows / 2) + offset;

    public WindowSize GetHorizontalPartial(int divideBy = 2, int index = 0)
    {
        var result = GetPartial(divideBy, index, Columns, Rows, ColumnsOrigin, RowOrigin);
        return new WindowSize
        (
            rows: result.secondary,
            columns: result.primary,
            rowOrigin: result.secondaryOrigin,
            columnsOrigin: result.primaryOrigin
        );
    }

    public WindowSize GetVerticalPartial(int divideBy = 2, int index = 0)
    {
        var result = GetPartial(divideBy, index, Rows, Columns, RowOrigin, ColumnsOrigin);
        return new WindowSize
        (
            rows: result.primary,
            columns: result.secondary,
            rowOrigin: result.primaryOrigin,
            columnsOrigin: result.secondaryOrigin
        );
    }

    private (int primary, int secondary, int primaryOrigin, int secondaryOrigin) GetPartial(int divideBy, int index, int primary, int secondary, int primaryOrigin, int secondaryOrigin)
    {
        if (divideBy < 2) throw new ArgumentException("divideBy must be 2 or more");
        if (index < 0 || index >= divideBy) throw new ArgumentException("index must be between 0 and divideBy - 1");
        return (primary / divideBy, secondary, primaryOrigin + (primary / divideBy * index), secondaryOrigin);
    }

    public static WindowSize Null => new WindowSize(0, 0, 0, 0);
    public static WindowSize Fullscreen => new WindowSize(Console.WindowHeight, Console.WindowWidth, 0, 0);

    public override bool Equals(object? target)
    {
        if (target is null) return false;
        if (target is not WindowSize) return false;
        var targetSize = (WindowSize)target;
        return Rows == targetSize.Rows
            && Columns == targetSize.Columns
            && RowOrigin == targetSize.RowOrigin
            && ColumnsOrigin == targetSize.ColumnsOrigin;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Rows, Columns, RowOrigin, ColumnsOrigin);
    }
}
