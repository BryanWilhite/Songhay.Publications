using System;
using System.ComponentModel.DataAnnotations;

namespace Songhay.Publications.Models
{
    /// <summary>
    /// Defines metadata of <see cref="Fragment"/>
    /// for validation and display.
    /// </summary>
    /// <seealso cref="Songhay.Publications.Models.IFragment" />
    public class FragmentMetadata : IFragment
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        [Display(Name = "Client ID", Order = 2)]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the incept date.
        /// </summary>
        /// <value>
        /// The incept date.
        /// </value>
        [Display(Name = "Incept Date", Order = 4)]
        [Required]
        public DateTime? InceptDate { get; set; }

        /// <summary>
        /// Gets or sets the document identifier.
        /// </summary>
        /// <value>
        /// The document identifier.
        /// </value>
        [Display(Name = "Document ID", Order = 0)]
        public int? DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        [Display(Name = "Expiration Date", Order = 7)]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the display name of the fragment.
        /// </summary>
        /// <value>
        /// The display name of the fragment.
        /// </value>
        [Display(Name = "Display Name", Order = 3)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string FragmentDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the fragment identifier.
        /// </summary>
        /// <value>
        /// The fragment identifier.
        /// </value>
        [Display(Name = "Fragment ID", Order = 1)]
        public int FragmentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the fragment.
        /// </summary>
        /// <value>
        /// The name of the fragment.
        /// </value>
        [Display(Name = "Item Name", Order = 2)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string FragmentName { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        /// <value>
        /// The is active.
        /// </value>
        [Display(Name = "Is Active?", Order = 10)]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the is next.
        /// </summary>
        /// <value>
        /// The is next.
        /// </value>
        [Display(Name = "Is Next?", Order = 11)]
        public bool? IsNext { get; set; }

        /// <summary>
        /// Gets or sets the is previous.
        /// </summary>
        /// <value>
        /// The is previous.
        /// </value>
        [Display(Name = "Is Previous?", Order = 12)]
        public bool? IsPrevious { get; set; }

        /// <summary>
        /// Gets or sets the is wrapper.
        /// </summary>
        /// <value>
        /// The is wrapper.
        /// </value>
        [Display(Name = "Is Wrapper?", Order = 13)]
        public bool? IsWrapper { get; set; }

        /// <summary>
        /// Gets or sets the item character.
        /// </summary>
        /// <value>
        /// The item character.
        /// </value>
        [Display(AutoGenerateField = false)]
        public string ItemChar { get; set; }

        /// <summary>
        /// Gets or sets the item text.
        /// </summary>
        /// <value>
        /// The item text.
        /// </value>
        [Display(AutoGenerateField = false)]
        public string ItemText { get; set; }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        /// <value>
        /// The modification date.
        /// </value>
        [Display(Name = "Modification Date", Order = 5)]
        [Required]
        public DateTime? ModificationDate { get; set; }

        /// <summary>
        /// Gets or sets the next fragment identifier.
        /// </summary>
        /// <value>
        /// The next fragment identifier.
        /// </value>
        [Display(Name = "Next Fragment ID", Order = 7)]
        public int? NextFragmentId { get; set; }

        /// <summary>
        /// Gets or sets the previous fragment identifier.
        /// </summary>
        /// <value>
        /// The previous fragment identifier.
        /// </value>
        [Display(Name = "Previous Fragment ID", Order = 8)]
        public int? PrevFragmentId { get; set; }

        /// <summary>
        /// Gets or sets the sort ordinal.
        /// </summary>
        /// <value>
        /// The sort ordinal.
        /// </value>
        [Display(Name = "Sort Ordinal", Order = 6)]
        public byte? SortOrdinal { get; set; }
    }
}
