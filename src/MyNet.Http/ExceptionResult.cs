// -----------------------------------------------------------------------
// <copyright file="ExceptionResult.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using MyNet.Reflection;
using Newtonsoft.Json.Linq;

namespace MyNet.Http;

/// <summary>
/// Manage Exception for Business Layers.
/// </summary>
[Serializable]
public sealed class ExceptionResult : ISerializable
{
    public Exception Exception { get; } = null!;

    public ExceptionResult() { }

    public ExceptionResult(Exception exception) => Exception = exception;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "Impossible on JsonObject")]
    [System.Security.SecuritySafeCritical] // auto-generated
    private ExceptionResult(SerializationInfo info, StreamingContext context)
    {
        try
        {
            var exception = info.GetValue(nameof(Exception), typeof(Exception));

            var assemblyStr = (string?)info.GetValue("assembly", typeof(string))!;
            var assembly = TypeHelper.GetAssemblyByName(assemblyStr);

            if (assembly is not null)
            {
                var typeStr = (string?)info.GetValue("type", typeof(string))!;
                var type = assembly.GetType(typeStr)!;
                Exception = (Exception)Convert.ChangeType(exception, type, CultureInfo.InvariantCulture)!;
            }
            else
            {
                var e = (Exception)exception!;
                Exception = new HttpException(e.Message, e.InnerException);
            }
        }
        catch (Exception)
        {
            var jsonErrors = (JObject?)info.GetValue("errors", typeof(JObject));
            var title = (JValue?)info.GetValue("title", typeof(JValue));

            if (jsonErrors != null)
            {
                var errors = new List<HttpError>();
                foreach (var item in jsonErrors)
                {
                    if (item.Value?.ToString() is null)
                        continue;
                    var str = item.Value switch
                    {
                        JArray array => array.First?.ToString(),
                        _ => item.Value.ToString()
                    };

                    errors.Add(new(str ?? string.Empty));
                }

                Exception = new MultipleHttpException(title?.Value?.ToString() ?? string.Empty, errors);
            }
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("assembly", Exception.GetType().Assembly.FullName, typeof(string));
        info.AddValue("type", Exception.GetType().Name, typeof(string));
        info.AddValue(nameof(Exception), Exception, Exception.GetType());
    }
}
