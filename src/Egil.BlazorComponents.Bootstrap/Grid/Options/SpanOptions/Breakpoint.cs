﻿using System;

namespace Egil.BlazorComponents.Bootstrap.Grid.Options
{
    public class Breakpoint : SpanOption
    {
        private BreakpointType type;

        public Breakpoint(BreakpointType type)
        {
            this.type = type;
        }

        public override string Value => type switch
        {
            BreakpointType.Small => "sm",
            BreakpointType.Medium => "md",
            BreakpointType.Large => "lg",
            BreakpointType.ExtraLarge => "xl",
            _ => throw new InvalidOperationException($"Unknown {nameof(BreakpointType)} specified in {nameof(Breakpoint)}.")
        };

        public static BreakpointNumber operator -(Breakpoint breakpoint, int width)
        {
            return new BreakpointNumber(breakpoint, width);
        }

        public static BreakpointFirst operator -(Breakpoint breakpoint, First option)
        {
            return new BreakpointFirst(breakpoint, option);
        }

        public static BreakpointLast operator -(Breakpoint breakpoint, Last option)
        {
            return new BreakpointLast(breakpoint, option);
        }

        public static BreakpointAuto operator -(Breakpoint breakpoint, Auto option)
        {
            return new BreakpointAuto(breakpoint, option);
        }
    }

}