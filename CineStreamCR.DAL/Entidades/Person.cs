namespace CineStreamCR.DAL.Entidades;

public sealed class Person
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string InformationSourceUrl { get; set; } = string.Empty;
    public string ImageSourceUrl { get; set; } = string.Empty;
    public ICollection<MovieCredit> Credits { get; set; } = new List<MovieCredit>();
}
