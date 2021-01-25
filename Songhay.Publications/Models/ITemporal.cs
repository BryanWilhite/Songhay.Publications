using System;
using Songhay.Publications.Models;

/// <summary>
/// Centralizes time-based editorial properties
/// for <see cref="Segment" />, <see cref="Document" />
/// and <see cref="Fragment" /> models
/// </summary>
public interface ITemporal
{
        /// <summary>
        /// Gets or sets the incept date.
        /// </summary>
        /// <value>
        /// The incept date.
        /// </value>
        DateTime? InceptDate { get; set; }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        /// <value>
        /// The modification date.
        /// </value>
        DateTime? ModificationDate { get; set; }
}