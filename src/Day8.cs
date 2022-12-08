namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
//using System.Linq;

public class Day8 : Day
{
    List<List<Tree>> trees = new List<List<Tree>>();

    public Day8(AOCEnvironment env) : base("2022", 8, env) { }

    protected override string ExecutePart1()
    {
        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            List<Tree> row = new List<Tree>(line.Length);
            trees.Add(row);
            int[] heights = line.ToCharArray().AsEnumerable().Select<char, int>(c => c - '0').ToArray();
            foreach (int height in heights)
            {
                row.Add(new Tree(height));
            }
        }

        for (int row = 0; row < trees.Count; row++)
        {
            int maxHeight = -1;
            for (int col = 0; col < trees[0].Count; col++)
            {
                Tree tree = trees[row][col];
                if (tree.Height > maxHeight)
                {
                    tree.Visible = true;
                    maxHeight = tree.Height;
                }
                if (tree.Height == 9)
                {
                    break;
                }
            }
        }

        for (int row = 0; row < trees.Count; row++)
        {
            int maxHeight = -1;
            for (int col = trees[0].Count - 1; col >= 0; col--)
            {
                Tree tree = trees[row][col];
                if (tree.Visible)
                {
                    // Minor optimisation: a tree that's already marked visible
                    // was visible from the left, so nothing left of it will be
                    // visible from the right.
                    break;
                }
                if (tree.Height > maxHeight)
                {
                    tree.Visible = true;
                    maxHeight = tree.Height;
                }
                if (tree.Height == 9)
                {
                    break;
                }
            }
        }

        for (int col = 0; col < trees[0].Count; col++)
        {
            int maxHeight = -1;
            for (int row = 0; row < trees.Count; row++)
            {
                Tree tree = trees[row][col];
                if (tree.Height > maxHeight)
                {
                    tree.Visible = true;
                    maxHeight = tree.Height;
                }
                if (tree.Height == 9)
                {
                    break;
                }
            }
        }

        for (int col = 0; col < trees[0].Count; col++)
        {
            int maxHeight = -1;
            for (int row = trees.Count - 1; row >= 0; row--)
            {
                Tree tree = trees[row][col];
                if (tree.Height > maxHeight)
                {
                    tree.Visible = true;
                    maxHeight = tree.Height;
                }
                if (tree.Height == 9)
                {
                    break;
                }
            }
        }

        return trees.Select<List<Tree>, int>(row => row.Where(tree => tree.Visible).Count()).Sum().ToString();
    }

    protected override string ExecutePart2()
    {
        return "";
    }

    protected override string? GetExampleInput()
    {
        return @"30373
25512
65332
33549
35390";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "21";
    }

    protected override string? GetExamplePart2Answer()
    {
        return null;
    }
}

internal class Tree
{
    internal int Height { get; init; }
    internal bool Visible { get; set; }

    internal Tree (int height)
    {
        this.Height = height;
        this.Visible = false;
    }
}