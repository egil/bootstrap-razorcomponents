﻿using Egil.RazorComponents.Bootstrap.Base;
using Egil.RazorComponents.Bootstrap.Utilities.Spacing;
using Microsoft.AspNetCore.Components;
using System;

namespace Egil.RazorComponents.Bootstrap.Components.Html
{
    public class Heading : ParentComponentBase
    {
        public static readonly string[] HeadingTags = { "h1", "h2", "h3", "h4", "h5", "h6" };
        protected int _size = 1;

        [Parameter]
        public int Size
        {
            get => _size;
            set
            {
                if (value < 1 || value > 6) throw new ArgumentOutOfRangeException("A HTML heading size can only between 1 and 6.");
                _size = value;
            }
        }

        /// <summary>
        /// Gets or sets the padding of the component, using Bootstrap.NETs spacing syntax.
        /// </summary>
        [Parameter] public SpacingParameter<PaddingSpacing> Padding { get; set; } = SpacingParameter<PaddingSpacing>.None;

        /// <summary>
        /// Gets or sets the margin of the component, using Bootstrap.NETs spacing syntax.
        /// </summary>
        [Parameter] public SpacingParameter<MarginSpacing> Margin { get; set; } = SpacingParameter<MarginSpacing>.None;

        protected override void OnCompomnentParametersSet()
        {
            DefaultElementTag = HeadingTags[_size - 1];
        }
    }

    public sealed class H1 : Heading
    {
        public H1() { Size = 1; }
    }
    public sealed class H2 : Heading
    {
        public H2() { Size = 2; }
    }
    public sealed class H3 : Heading
    {
        public H3() { Size = 3; }
    }
    public sealed class H4 : Heading
    {
        public H4() { Size = 4; }
    }
    public sealed class H5 : Heading
    {
        public H5() { Size = 5; }
    }
    public sealed class H6 : Heading
    {
        public H6() { Size = 6; }
    }
}