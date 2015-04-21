using UnityEngine;
using System.Collections;

public class GameTile : MonoBehaviour 
{
	private MeshCollider _col;
	private Renderer _renderer;
	private bool _isWallTile;
	public bool IsWallTile
	{
		get { return _isWallTile; }
		set { _isWallTile = value; }
	}

	public Trap currentTrap;


	private Room _parentRoom;
	public Room parentRoom
	{
		get
		{
			if (_parentRoom == null)
			{
				_parentRoom = transform.parent.GetComponent<Room>();
			}
			return _parentRoom;
		}
		set { _parentRoom = value; }
	}

	void Awake()
	{
		_col = this.gameObject.GetComponent<MeshCollider>();
		_renderer = this.gameObject.GetComponent<Renderer>();
	}
	
	void OnMouseUp()
	{
		if (GameManager.GetInstance().IsTileValid(this))
		{
			if((parentRoom != null) && (parentRoom.CanHaveTraps))
			{
				CTEventManager.FireEvent(new PlaceTrapEvent() { gameTile = this });
			}
			else if (parentRoom == null)
			{
				CTEventManager.FireEvent(new PlaceTrapEvent() { gameTile = this });
			}
			else
			{
				CTEventManager.FireEvent (new PlaySFXEvent () {assetName = "audio/sfx/negatory"}); //events for everyone
			}
		}
		else
		{
			CTEventManager.FireEvent (new PlaySFXEvent () {assetName = "audio/sfx/negatory"}); //events for everyone
		}
	}
	

	public void SetTileSelected()
	{

	}

	public void SetTexture(string texPath)
	{
		Material tex = Resources.Load <Material>(texPath);
		this.gameObject.GetComponent<Renderer>().material = tex;
	}
}