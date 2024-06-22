namespace ConsoleRaycasting;

internal struct Player
{
	public float _moveSpeed { get; init; } = 1.0f;

	public Player(float moveSpeed)
	{
		if (moveSpeed <= 0)
			throw new InvalidDataException("Movement speed must be greater than zero");
		_moveSpeed = moveSpeed;
	}
}