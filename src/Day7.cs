namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
using System.Linq;

public class Day7 : Day
{
    private static readonly int TOTAL_SPACE = 70000000;
    private static readonly int SPACE_REQUIRED = 30000000;

    private Directory _root = Directory.CreateRootDirectory();
    private List<Directory> _allDirs = new List<Directory>();

    public Day7(AOCEnvironment env) : base("2022", 7, env)
    {
        _allDirs.Add(_root);
    }

    protected override string ExecutePart1()
    {
        Directory currentDir = _root;

        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            string[] chunks = line.Split(' ');
            if (chunks[0] == "$")
            {
                // Command
                if (chunks[1] == "cd")
                {
                    switch (chunks[2])
                    {
                        case "/":
                            currentDir = _root;
                            break;

                        case "..":
                            currentDir = currentDir.Parent;
                            break;

                        default:
                            currentDir = currentDir.GetChildDirectory(chunks[2]);
                            break;
                    }
                }
            } else {
                // Listing
                if (chunks[0] == "dir")
                {
                    Directory newDir = currentDir.AddSubdirectory(chunks[1]);
                    _allDirs.Add(newDir);
                } else {
                    currentDir.AddFile(new File(chunks[1], int.Parse(chunks[0])));
                }
            }
        }

        return _allDirs
            .AsEnumerable()
            .Select<Directory, int>(dir => dir.TotalSize())
            .Where(size => size < 100000)
            .Sum()
            .ToString();
    }

    protected override string ExecutePart2()
    {
        int freeSpace = TOTAL_SPACE - _root.TotalSize();
        int spaceNeeded = SPACE_REQUIRED - freeSpace;
        return _allDirs
            .AsEnumerable()
            .Select<Directory, int>(dir => dir.TotalSize())
            .Where(size => size >= spaceNeeded)
            .Min()
            .ToString();
    }

    protected override string? GetExampleInput()
    {
        return @"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "95437";
    }

    protected override string? GetExamplePart2Answer()
    {
        return "24933642";
    }
}

internal class Directory
{
    private Dictionary<string, Directory> _children = new Dictionary<string, Directory>();
    private List<File> _contents = new List<File>();
    private int? _totalSize;

    internal Directory Parent { get; init; }

    private Directory(Directory parent)
    {
        this.Parent = parent;
    }

    internal static Directory CreateRootDirectory()
    {
        return new Directory(null!);
    }

    internal void AddFile(File file)
    {
        _contents.Add(file);
    }

    internal Directory AddSubdirectory(string name)
    {
        Directory newDir = new Directory(this);
        _children.Add(name, newDir);
        return newDir;
    }

    internal Directory GetChildDirectory(string name)
    {
        return _children[name];
    }

    internal int TotalSize()
    {
        if (_totalSize.HasValue)
        {
            return _totalSize.Value;
        } else {
            int size = 0;
            foreach (File file in _contents)
            {
                size += file.Size;
            }
            foreach (Directory child in _children.Values)
            {
                size += child.TotalSize();
            }
            _totalSize = size;
            return size;
        }
    }
}

internal class File
{
    internal string Name { get; init; }
    internal int Size { get; init; }

    internal File (string name, int size)
    {
        this.Name = name;
        this.Size = size;
    }
}