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

	// Mouse Events
	// Mouse Enter Un-nessecary.
	void OnMouseEnter()
	{
//		if(_renderer != null)
//		{
//			if(_renderer.material != null)
//			{
//				if (GameManager.GetInstance().IsPlacingTrap())
//				{
//					_renderer.material.color = Color.red;
//
//					if (GameManager.GetInstance().IsTileValid(this))
//					{
//						if((parentRoom != null) && (parentRoom.CanHaveTraps))
//						{
//							_renderer.material.color = Color.green;
//						}
//						else if (parentRoom == null)
//						{
//							_renderer.material.color = Color.green;
//						}
//						
//					}
//				}
//			}
//		}
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
		}
	}

	void OnMouseOver()
	{
		if(_renderer != null)
		{
			if(_renderer.material != null)
			{
				if (GameManager.GetInstance().IsPlacingTrap())
				{
					_renderer.material.color = Color.red;
					
					if (GameManager.GetInstance().IsTileValid(this))
					{
						if((parentRoom != null) && (parentRoom.CanHaveTraps))
						{
							_renderer.material.color = Color.green;
						}
						else if (parentRoom == null)
						{
							_renderer.material.color = Color.green;
						}
						
					}
				}
			}
		}
	}

	void OnMouseExit()
	{
		if(_renderer != null)
		{
			if(_renderer.material != null)
			{
				_renderer.material.color = Color.white;
			}
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