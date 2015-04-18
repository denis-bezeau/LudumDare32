
/// <summary>
/// A special room that allows spawning of people
/// </summary>
public class SpawnRoom : Room
{

	public override bool CanHaveTraps {
		get{ return false;}
	}

	public void Awake()
	{
		// Check for open path immediately
	}

	/// <summary>
	/// Check if we can get to the exit through open doors
	/// TODO: We probably don't want this here
	/// </summary>
	/// <returns><c>true</c>, if path to exit exists was opened, <c>false</c> otherwise.</returns>
	private bool OpenPathToExitExists(Room currentRoom)
	{
		// recurse through rooms
		return true;
	}
}
