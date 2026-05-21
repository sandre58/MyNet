// -----------------------------------------------------------------------
// <copyright file="ProgresserExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Utilities.Progress;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides extension methods for <see cref="IProgresser"/> to allow using string messages directly without having to create <see cref="ProgressMessage"/> instances explicitly.
/// </summary>
public static class ProgresserExtensions
{
    // ── Begin (root step) ────────────────────────────────────────────────────
    extension(IProgresser progresser)
    {
        /// <summary>Starts a new single-step session with a string message.</summary>
        public IProgressStep<ProgressMessage> Begin(string message, params object[] parameters)
            => progresser.Begin(new(message, parameters));

        /// <summary>Starts a new session divided into <paramref name="numberOfSteps"/> equal sub-steps.</summary>
        public IProgressStep<ProgressMessage> Begin(int numberOfSteps, string message, params object[] parameters)
            => progresser.Begin(numberOfSteps, new(message, parameters));

        /// <summary>Starts a new session with custom sub-step weightings and a string message.</summary>
        public IProgressStep<ProgressMessage> Begin(IEnumerable<double> subStepWeightings, string message, params object[] parameters)
            => progresser.Begin(subStepWeightings, new(message, parameters));

        /// <summary>Starts a new cancellable session with a string message.</summary>
        public IProgressStep<ProgressMessage> Begin(Action cancelAction, string message, params object[] parameters)
            => progresser.Begin(new(message, parameters), cancelAction);

        /// <summary>Starts a new cancellable session divided into <paramref name="numberOfSteps"/> equal sub-steps.</summary>
        public IProgressStep<ProgressMessage> Begin(int numberOfSteps, Action cancelAction, string message, params object[] parameters)
            => progresser.Begin(numberOfSteps, new(message, parameters), cancelAction);

        /// <summary>Starts a new cancellable session with custom sub-step weightings and a string message.</summary>
        public IProgressStep<ProgressMessage> Begin(IEnumerable<double> subStepWeightings, Action cancelAction, string message, params object[] parameters)
            => progresser.Begin(subStepWeightings, new(message, parameters), cancelAction);

        /// <summary>Pushes a cancellable child step with a string message.</summary>
        public IProgressStep<ProgressMessage> StartStep(string message, params object[] parameters)
            => progresser.StartStep(new(message, parameters));

        /// <summary>Pushes a cancellable child step divided into <paramref name="numberOfSteps"/> equal sub-steps.</summary>
        public IProgressStep<ProgressMessage> StartStep(int numberOfSteps, string message, params object[] parameters)
            => progresser.StartStep(numberOfSteps, new(message, parameters));

        /// <summary>Pushes a cancellable child step with custom sub-step weightings and a string message.</summary>
        public IProgressStep<ProgressMessage> StartStep(IEnumerable<double> subStepWeightings, string message, params object[] parameters)
            => progresser.StartStep(subStepWeightings, new(message, parameters));

        /// <summary>Pushes a non-cancellable child step with a string message.</summary>
        public IProgressStep<ProgressMessage> StartStepUncancellable(string message, params object[] parameters)
            => progresser.StartStep(new(message, parameters), canCancel: false);

        /// <summary>Pushes a non-cancellable child step divided into <paramref name="numberOfSteps"/> equal sub-steps.</summary>
        public IProgressStep<ProgressMessage> StartStepUncancellable(int numberOfSteps, string message, params object[] parameters)
            => progresser.StartStep(numberOfSteps, new(message, parameters), canCancel: false);

        /// <summary>Pushes a non-cancellable child step with custom sub-step weightings and a string message.</summary>
        public IProgressStep<ProgressMessage> StartStepUncancellable(IEnumerable<double> subStepWeightings, string message, params object[] parameters)
            => progresser.StartStep(subStepWeightings, new(message, parameters), canCancel: false);
    }

    // ── StartStep (child step) ───────────────────────────────────────────────
}
