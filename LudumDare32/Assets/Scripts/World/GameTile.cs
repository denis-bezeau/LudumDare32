using UnityEngine;
using System.Collections;

public class GameTile : MonoBehaviour 
{
	private MeshCollider _col;
	private Renderer _renderer;
		
	void Awake()
	{
		_col = this.gameObject.GetComponent<MeshCollider>();
		_renderer = this.gameObject.GetComponent<Renderer>();
	}

	// Mouse Events
	void OnMouseEnter()
	{
		if(_renderer != null)
		{
			if(_renderer.material != null)
			{
				_renderer.material.color = Color.red;
			}
		}
	}

	void OnMouseOver()
	{

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

	void Update() 
	{
		if(Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray mousePosRay = Camera.main.ScreenPointToRay(Input.mousePosition);

			if(Physics.Raycast(mousePosRay, out hit))
			{
				if(hit.collider == _col)
				{
					Debug.Log ("Click is on an object: " + this.gameObject.name);
				}
			}
		}
		if(Input.GetMouseButtonDown(1))
		{
			// Do something else.
		}
		if(Input.GetMouseButtonDown(2))
		{
			// yeah.
		}
	}

	public void SetTexture(string texPath)
	{
		Material tex = Resources.Load <Material>(texPath);
		this.gameObject.GetComponent<Renderer>().material = tex;
	}
}