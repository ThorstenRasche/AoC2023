﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Numerics;


namespace AoC23.Domain;

[Day(13)]
public partial class Day13Calculator : IDayCalculator
{
    public long CalculatePart1(string filePath)
    {
       var input = File.ReadAllText(filePath);
        string[][] maps = input.ReplaceLineEndings("\n").Split("\n\n", StringSplitOptions.TrimEntries)
            .Select(paragraph => paragraph.Split('\n'))
            .ToArray();
        long sum = 0;
        foreach (var map in maps)
        {
            int vertical = FindVerticalMirror(map);
            if (vertical > 0)
            {
                sum += vertical;
            }
            else
            {
                int horizontal = FindHorizontalMirror(map);
                sum += 100 * horizontal;
            }
        }
        return sum;
    }

    public long CalculatePart2(string filePath)
    {
        var input = File.ReadAllText(filePath);
        string[][] maps = input.ReplaceLineEndings("\n").Split("\n\n", StringSplitOptions.TrimEntries)
            .Select(paragraph => paragraph.Split('\n'))
            .ToArray();
        long sum = 0;
        foreach (var map in maps)
        {
            int vertical = FindVerticalMirrorSmudged(map);
            if (vertical > 0)
            {
                sum += vertical;
            }
            else
            {
                int horizontal = FindHorizontalMirrorSmudged(map);
                sum += 100 * horizontal;
            }
        }
        return sum;
    }
    internal static int FindVerticalMirror(string[] map)
    {
        int height = map.Length;
        int width = map[0].Length;
        for (int mirror = 1; mirror < width; mirror++)
        {
            bool isReflection = true;
            int rightSide = width - mirror;
            int reflectionWidth = Math.Min(mirror, rightSide);
            for (int d = 0; d < reflectionWidth; d++)
            {
                for (int row = 0; row < height; row++)
                {
                    if (map[row][mirror - 1 - d] != map[row][mirror + d])
                    {
                        isReflection = false;
                    }
                }
            }
            if (isReflection)
            {
                return mirror;
            }
        }
        return 0;
    }

    internal static int FindHorizontalMirror(string[] map)
    {
        int height = map.Length;
        int width = map[0].Length;
        for (int mirror = 1; mirror < height; mirror++)
        {
            bool isReflection = true;
            int bottomSide = height - mirror;
            int reflectionHeight = Math.Min(mirror, bottomSide);
            for (int d = 0; d < reflectionHeight; d++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (map[mirror - 1 - d][col] != map[mirror + d][col])
                    {
                        isReflection = false;
                    }
                }
            }
            if (isReflection)
            {
                return mirror;
            }
        }
        return 0;
    }
    internal static int FindVerticalMirrorSmudged(string[] map)
    {
        int height = map.Length;
        int width = map[0].Length;
        for (int mirror = 1; mirror < width; mirror++)
        {
            int smudges = 0;
            int rightSide = width - mirror;
            int reflectionWidth = Math.Min(mirror, rightSide);
            for (int d = 0; d < reflectionWidth && smudges <= 1; d++)
            {
                for (int row = 0; row < height && smudges <= 1; row++)
                {
                    if (map[row][mirror - 1 - d] != map[row][mirror + d])
                    {
                        smudges++;
                    }
                }
            }
            if (smudges == 1)
            {
                return mirror;
            }
        }
        return 0;
    }

    internal static int FindHorizontalMirrorSmudged(string[] map)
    {
        int height = map.Length;
        int width = map[0].Length;
        for (int mirror = 1; mirror < height; mirror++)
        {
            int smudges = 0;
            int bottomSide = height - mirror;
            int reflectionHeight = Math.Min(mirror, bottomSide);
            for (int d = 0; d < reflectionHeight && smudges <= 1; d++)
            {
                for (int col = 0; col < width && smudges <= 1; col++)
                {
                    if (map[mirror - 1 - d][col] != map[mirror + d][col])
                    {
                        smudges++;
                    }
                }
            }
            if (smudges == 1)
            {
                return mirror;
            }
        }
        return 0;
    }

}