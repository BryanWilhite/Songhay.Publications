using System.ComponentModel.DataAnnotations;

namespace Songhay.Publications.Models;

/// <summary>
/// Publications Fragment
/// </summary>
public class Fragment : IFragment
{
    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    [Display(Name = "Client ID", Order = 2)]
    [DisallowNull]
    public string? ClientId { get => _clientId; set => _clientId = value.ToReferenceTypeValueOrThrow(); }

    /// <summary>
    /// Gets or sets the incept date.
    /// </summary>
    [Display(Name = "Incept Date", Order = 4)]
    public DateTime? InceptDate { get; set; }

    /// <summary>
    /// Gets or sets the document identifier.
    /// </summary>
    [Display(Name = "Document ID", Order = 0)]
    [DisallowNull]
    public int? DocumentId { get => _documentId; set => _documentId = value.ToValueOrThrow(); }

    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    [Display(Name = "Expiration Date", Order = 7)]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the display name of the fragment.
    /// </summary>
    [Display(Name = "Display Name", Order = 3)]
    [DisplayFormat(ConvertEmptyStringToNull = true)]
    public string? FragmentDisplayName { get; set; }

    /// <summary>
    /// Gets or sets the fragment identifier.
    /// </summary>
    [Display(Name = "Fragment ID", Order = 1)]
    [DisallowNull]
    public int? FragmentId { get => _fragmentId; set => _fragmentId = value.ToValueOrThrow(); }

    /// <summary>
    /// Gets or sets the name of the fragment.
    /// </summary>
    [Display(Name = "Item Name", Order = 2)]
    [DisplayFormat(ConvertEmptyStringToNull = true)]
    public string? FragmentName { get; set; }

    /// <summary>
    /// Gets or sets the is active.
    /// </summary>
    [Display(Name = "Is Active?", Order = 10)]
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the is next.
    /// </summary>
    [Display(Name = "Is Next?", Order = 11)]
    public bool? IsNext { get; set; }

    /// <summary>
    /// Gets or sets the is previous.
    /// </summary>
    [Display(Name = "Is Previous?", Order = 12)]
    public bool? IsPrevious { get; set; }

    /// <summary>
    /// Gets or sets the is wrapper.
    /// </summary>
    [Display(Name = "Is Wrapper?", Order = 13)]
    public bool? IsWrapper { get; set; }

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the item text.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public string? ItemText { get; set; }

    /// <summary>
    /// Gets or sets the modification date.
    /// </summary>
    [Display(Name = "Modification Date", Order = 5)]
    public DateTime? ModificationDate { get; set; }

    /// <summary>
    /// Gets or sets the next fragment identifier.
    /// </summary>
    [Display(Name = "Next Fragment ID", Order = 7)]
    public int? NextFragmentId { get; set; }

    /// <summary>
    /// Gets or sets the previous fragment identifier.
    /// </summary>
    [Display(Name = "Previous Fragment ID", Order = 8)]
    public int? PrevFragmentId { get; set; }

    /// <summary>
    /// Gets or sets the sort ordinal.
    /// </summary>
    [Display(Name = "Sort Ordinal", Order = 6)]
    public byte? SortOrdinal { get; set; }

    /// <summary>
    /// Gets or sets the document.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public Document? Document { get; set; }

    /// <summary>
    /// Gets or sets the next fragments.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public ICollection<Fragment> NextFragments { get; init; } = new List<Fragment>();

    /// <summary>
    /// Gets or sets the next fragment.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public Fragment? NextFragment { get; set; }

    /// <summary>
    /// Gets or sets the previous fragments.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public ICollection<Fragment> PrevFragments { get; init; } = new List<Fragment>();

    /// <summary>
    /// Gets or sets the previous fragment.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public Fragment? PrevFragment { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return this.ToDisplayText();
    }

    string? _clientId;
    int? _documentId;
    int? _fragmentId;
}
