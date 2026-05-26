// -----------------------------------------------------------------------
// <copyright file="SmtpHelper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyNet.Mail.Smtp;

public static class SmtpHelper
{
    public static bool TestSmtpConnection(string? server, int port)
    {
        if (string.IsNullOrWhiteSpace(server) || port is < IPEndPoint.MinPort or > IPEndPoint.MaxPort)
        {
            return false;
        }

        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(server, port);
            if (!connectTask.Wait(TimeSpan.FromSeconds(5)))
            {
                return false;
            }

            client.SendTimeout = 5000;
            client.ReceiveTimeout = 5000;

            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.ASCII, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
            using var writer = new StreamWriter(stream, Encoding.ASCII, bufferSize: 1024, leaveOpen: true);
            writer.NewLine = "\r\n";
            writer.AutoFlush = true;

            if (!CheckResponse(reader, 220))
            {
                return false;
            }

            writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "EHLO {0}", Dns.GetHostName()));
            return CheckResponse(reader, 250);

            // if we got here it's that we can connect to the smtp server
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool CheckResponse(TextReader reader, int expectedCode)
    {
        while (true)
        {
            var responseLine = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(responseLine) || responseLine.Length < 3)
            {
                return false;
            }

            if (!int.TryParse(responseLine.AsSpan(0, 3), NumberStyles.None, CultureInfo.InvariantCulture, out var responseCode))
            {
                return false;
            }

            if (responseCode != expectedCode)
            {
                return false;
            }

            // SMTP multiline responses use "250-..." and end with "250 ...".
            if (responseLine.Length < 4 || responseLine[3] != '-')
            {
                return true;
            }
        }
    }
}
