namespace betclic_test.Memory
{
    public class BDD_Tournament
    {
        public BDD_Tournament(int tournamentId, string TournamentName)
        {
            TournamentId = tournamentId;
            this.TournamentName = TournamentName;
        }

        public int TournamentId { get; }
        public string TournamentName { get; }
    }
}
