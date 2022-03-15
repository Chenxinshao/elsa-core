﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Elsa.Attributes;
using Elsa.Contracts;
using Elsa.Models;

namespace Elsa.Modules.Scheduling.Triggers;

/// <summary>
/// Represents a timer to periodically trigger the workflow.
/// </summary>
public class Timer : EventGenerator
{
    [JsonConstructor]
    public Timer()
    {
    }

    public Timer(TimeSpan interval) : this(new Input<TimeSpan>(interval))
    {
    }

    public Timer(Input<TimeSpan> interval)
    {
        Interval = interval;
    }
    
    [Input] public Input<TimeSpan> Interval { get; set; } = default!;

    protected override IEnumerable<object> GetTriggerData(TriggerIndexingContext context)
    {
        var interval = context.ExpressionExecutionContext.Get(Interval);
        var clock = context.ExpressionExecutionContext.GetRequiredService<ISystemClock>();
        var executeAt = clock.UtcNow.Add(interval);
        yield return new TimerPayload(executeAt, interval);
    }

    public static Timer FromTimeSpan(TimeSpan value) => new(value);
    public static Timer FromSeconds(double value) => FromTimeSpan(TimeSpan.FromSeconds(value));
}

public record TimerPayload(DateTimeOffset StartAt, TimeSpan Interval);