using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Static class for parsing player commands
/// </summary>
public static class CommandParser
{
    private static readonly Regex moveForwardRegex = new Regex(@"^MOVE_FORWARD\((\d+(?:\.\d+)?)\)$", RegexOptions.IgnoreCase);
    private static readonly Regex rotateRegex = new Regex(@"^ROTATE\((-?\d+(?:\.\d+)?)\)$", RegexOptions.IgnoreCase);

    public static List<string> ParseCommands(string inputText)
    {
        List<string> commands = new List<string>();
        
        if (string.IsNullOrEmpty(inputText))
            return commands;

        string[] lines = inputText.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("//"))
                continue;

            if (ValidateCommand(trimmedLine))
                commands.Add(trimmedLine.ToUpper());
            else
                Debug.LogWarning($"Invalid command: {trimmedLine}");
        }

        return commands;
    }

    private static bool ValidateCommand(string command)
    {
        command = command.Trim();

        if (moveForwardRegex.IsMatch(command))
        {
            Match match = moveForwardRegex.Match(command);
            float distance = float.Parse(match.Groups[1].Value);
            return distance > 0 && distance <= 20;
        }

        if (rotateRegex.IsMatch(command))
        {
            Match match = rotateRegex.Match(command);
            float degrees = float.Parse(match.Groups[1].Value);
            return degrees >= -360 && degrees <= 360;
        }

        return false;
    }

    public static string GetCommandHelp()
    {
        return "Available Commands:\n\n" +
               "MOVE_FORWARD(distance)\n" +
               "- Moves robot forward\n" +
               "- Example: MOVE_FORWARD(5)\n\n" +
               "ROTATE(degrees)\n" +
               "- Rotates robot\n" +
               "- Positive = right, Negative = left\n" +
               "- Example: ROTATE(90) or ROTATE(-90)\n\n" +
               "Tips:\n" +
               "- One command per line\n" +
               "- Use // for comments";
    }
}