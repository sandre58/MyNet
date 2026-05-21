// -----------------------------------------------------------------------
// <copyright file="ProcessHelper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyNet.Utilities.IO;

/// <summary>
/// Provides helper methods for starting processes and opening files or folders.
/// </summary>
public static class ProcessHelper
{
    /// <summary>
    /// Starts a process using the specified URI. The URI can be a file path, folder path, or URL. The method uses the default application associated with the URI type to open it.
    /// </summary>
    /// <param name="uri">The URI to open.</param>
    /// <exception cref="ArgumentException">Thrown when the URI is null or whitespace.</exception>
    public static void Start(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);

        _ = Process.Start(new ProcessStartInfo { FileName = uri, UseShellExecute = true });
    }

    /// <summary>
    /// Attempts to start a process using the specified URI. Returns <c>true</c> if the process was started successfully; otherwise, <c>false</c>. The method catches any exceptions that occur during the process start and returns <c>false</c> in such cases.
    /// </summary>
    /// <param name="uri">The URI to open.</param>
    /// <returns><c>true</c> if the process was started successfully; otherwise, <c>false</c>.</returns>
    public static bool TryStart(string uri)
    {
        try
        {
            Start(uri);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Starts a process using the specified executable and arguments. The method uses the default application associated with the executable to open it. The arguments are passed as a list of strings, which are added to the process start info's argument list.
    /// </summary>
    /// <param name="executable">The executable to run.</param>
    /// <param name="arguments">The arguments to pass to the executable.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="executable"/> is null or whitespace.</exception>
    public static void Open(string executable, params IEnumerable<string> arguments)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executable);

        var startInfo = new ProcessStartInfo { FileName = executable, UseShellExecute = true };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        _ = Process.Start(startInfo);
    }

    /// <summary>
    /// Opens a file using the default application associated with its file type. The method validates that the provided file path is not null or whitespace before attempting to open it.
    /// </summary>
    /// <param name="filePath">The path of the file to open.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is null or whitespace.</exception>
    public static void OpenFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        Start(filePath);
    }

    /// <summary>
    /// Opens a file in Excel using the default application associated with its file type. This method is essentially an alias for <see cref="OpenFile(string)"/>, as it relies on the default application to open the file. The method validates that the provided file path is not null or whitespace before attempting to open it.
    /// </summary>
    /// <param name="filePath">The path of the file to open in Excel.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is null or whitespace.</exception>
    public static void OpenInExcel(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        OpenFile(filePath);
    }

    /// <summary>
    /// Opens a folder using the default file explorer. The method validates that the provided folder path is not null or whitespace before attempting to open it.
    /// </summary>
    /// <param name="folderPath">The path of the folder to open.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="folderPath"/> is null or whitespace.</exception>
    public static void OpenFolder(string folderPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        Open("explorer.exe", folderPath);
    }
}
