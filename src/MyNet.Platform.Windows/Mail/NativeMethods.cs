// -----------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
#pragma warning disable SA1401 // FieldsMustBePrivate
#pragma warning disable SA1202 // ElementsMustBeOrderedByAccess
#pragma warning disable CA5392

// ReSharper disable All
namespace MyNet.Platform.Windows.Mail;

public static partial class NativeMethods
{
    [DllImport("MAPI32.DLL")]
    private static extern int MAPISendMail(IntPtr sess, IntPtr hwnd, MapiMessage message, int flg, int rsv);

    public static bool SendMail(string addr, string title) => SendMail(addr, title, string.Empty, string.Empty);

    public static bool SendMail(string addr, string title, string body) => SendMail(addr, title, body, string.Empty);

    public static bool SendMail(string addr, string title, string body, string fileName)
    {
        var recipientPointer = IntPtr.Zero;
        var attachmentPointer = IntPtr.Zero;

        try
        {
            recipientPointer = GetRecipient(addr);
            var msg = new MapiMessage
            {
                recips = recipientPointer,
                recipCount = 1,
                subject = title,
                noteText = body,
                fileCount = 0,
                files = IntPtr.Zero
            };

            if (!string.IsNullOrEmpty(fileName))
            {
                attachmentPointer = GetAttachment(fileName);
                msg.files = attachmentPointer;
                msg.fileCount = 1;
            }

            var result = MAPISendMail(IntPtr.Zero, IntPtr.Zero, msg, 0x1 | 0x8, 0);
            return result is <= 1 and >= 0;
        }
        catch
        {
            return false;
        }
        finally
        {
            if (recipientPointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(recipientPointer);
            }

            if (attachmentPointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(attachmentPointer);
            }
        }
    }

    private static IntPtr GetRecipient(string address)
    {
        var recipient = new MapiRecipDesc
        {
            recipClass = 1,
            name = address
        };

        var size = Marshal.SizeOf<MapiRecipDesc>();
        var intPtr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(recipient, intPtr, false);

        return intPtr;
    }

    private static IntPtr GetAttachment(string fileName)
    {
        var size = Marshal.SizeOf<MapiFileDesc>();
        var intPtr = Marshal.AllocHGlobal(size);

        var mapiFileDesc = new MapiFileDesc
        {
            // An integer used to indicate where in the message text to render the attachment.
            position = -1,

            name = System.IO.Path.GetFileName(fileName),
            path = fileName
        };
        Marshal.StructureToPtr(mapiFileDesc, intPtr, false);

        return intPtr;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private sealed class MapiMessage
    {
        public int reserved;

        public string subject = string.Empty;

        public string noteText = string.Empty;
        public string messageType = string.Empty;
        public string dateReceived = string.Empty;
        public string conversationID = string.Empty;
        public int flags;
        public IntPtr originator;

        public int recipCount;

        public IntPtr recips;

        public int fileCount;
        public IntPtr files;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private sealed class MapiRecipDesc
    {
        public int reserved;

        public int recipClass;

        public string name = string.Empty;
        public string address = string.Empty;
        public int eIDSize;
        public IntPtr entryID;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private sealed class MapiFileDesc
    {
        public int reserved;
        public int flags;

        public int position;

        public string path = string.Empty;

        public string name = string.Empty;
        public IntPtr type;
    }
}
