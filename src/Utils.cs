namespace Patersoft.AOC.AOC22.Utils;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Reads a data source line by line. The source can be a file, a stream,
/// or a text reader. In any case, the source is only opened when the
/// enumerator is fetched, and is closed when the iterator is disposed.
/// </summary>
public sealed class LineReader : IEnumerable<string>
{
    /// <summary>
    /// Means of creating a TextReader to read from.
    /// </summary>
    readonly Func<TextReader> dataSource;

    /// <summary>
    /// Creates a LineReader from a stream source. The delegate is only
    /// called when the enumerator is fetched. UTF-8 is used to decode
    /// the stream into text.
    /// </summary>
    /// <param name="streamSource">Data source</param>
    public LineReader(Func<Stream> streamSource)
        : this(streamSource, Encoding.UTF8)
    {
    }

    /// <summary>
    /// Creates a LineReader from a stream source. The delegate is only
    /// called when the enumerator is fetched.
    /// </summary>
    /// <param name="streamSource">Data source</param>
    /// <param name="encoding">Encoding to use to decode the stream
    /// into text</param>
    public LineReader(Func<Stream> streamSource, Encoding encoding)
        : this(() => new StreamReader(streamSource(), encoding))
    {
    }

    /// <summary>
    /// Creates a LineReader from a filename. The file is only opened
    /// (or even checked for existence) when the enumerator is fetched.
    /// UTF8 is used to decode the file into text.
    /// </summary>
    /// <param name="filename">File to read from</param>
    public LineReader(string filename)
        : this(filename, Encoding.UTF8)
    {
    }

    /// <summary>
    /// Creates a LineReader from a filename. The file is only opened
    /// (or even checked for existence) when the enumerator is fetched.
    /// </summary>
    /// <param name="filename">File to read from</param>
    /// <param name="encoding">Encoding to use to decode the file
    /// into text</param>
    public LineReader(string filename, Encoding encoding)
        : this(() => new StreamReader(filename, encoding))
    {
    }

    /// <summary>
    /// Creates a LineReader from a TextReader source. The delegate
    /// is only called when the enumerator is fetched
    /// </summary>
    /// <param name="dataSource">Data source</param>
    public LineReader(Func<TextReader> dataSource)
    {
        this.dataSource = dataSource;
    }

    /// <summary>
    /// Enumerates the data source line by line.
    /// </summary>
    public IEnumerator<string> GetEnumerator()
    {
        using (TextReader reader = dataSource())
        {
            string line;
#pragma warning disable CS8600
            while ((line = reader.ReadLine()) != null)
#pragma warning restore CS8600
            {
                yield return line;
            }
        }
    }

    /// <summary>
    /// Enumerates the data source line by line.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class MathUtils
{
    public static int GreatestCommonFactor(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static int LowestCommonMultiple(int a, int b)
    {
        return (a / GreatestCommonFactor(a, b)) * b;
    }
}

public class Position
{
    protected int _row;
    protected int _col;
    public int Row { get { return _row; } }
    public int Col { get { return _col; } }

    public Position(int row, int col)
    {
        _row = row;
        _col = col;
    }

    public Position() : this(0, 0) { }

    public Position Clone()
    {
        return new Position(_row, _col);
    }

    public override bool Equals(object? obj)
    {
        var item = obj as Position;
        if (item == null)
        {
            return false;
        }

        return _row == item._row && _col == item._col;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_row, _col);
    }
}
