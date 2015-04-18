
/// <summary>
/// Special room that a person wants to get to
/// </summary>
public class EscapeRoom : Room
{

	public override bool CanHaveTraps {
		get{ return false;}
	}

}