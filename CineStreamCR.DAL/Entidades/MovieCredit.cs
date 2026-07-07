namespace CineStreamCR.DAL.Entidades;

public sealed class MovieCredit
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public CreditType CreditType { get; set; }
    public string? CharacterName { get; set; }
    public int BillingOrder { get; set; }
}
