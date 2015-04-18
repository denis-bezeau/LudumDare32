
public class SmokeTrap : RoomTrap
{
	private const string _title = "default";
	public virtual string Title { get { return _title; } }
	
	private const int _cost = 1;
	public virtual int Cost { 
		get { return _cost; } 
	}

	public SmokeTrap (Room parent) : base(parent)
	{

	}
}