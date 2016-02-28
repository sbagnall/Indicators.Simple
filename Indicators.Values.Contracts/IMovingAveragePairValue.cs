namespace SteveBagnall.Trading.Indicators.Values.Contracts
{
    public interface IMovingAveragePairValue
	{
		decimal Fast { get; }

		decimal Slow { get; }
	}
}
