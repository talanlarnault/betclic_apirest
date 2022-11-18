namespace betclic_test.Memory
{
    public class BDD_Player
    {
        public BDD_Player(int playerId, string pseudo, int tournamentId)
        {
            PlayerId = playerId;
            Pseudo = pseudo;
            TournamentId = tournamentId;
        }
        public int PlayerId { get; }
        public string Pseudo { get; }
        public int TournamentId { get; }
        public int Score { get; set; }
        public int TournamentPosition { get; set; }
    }
}
