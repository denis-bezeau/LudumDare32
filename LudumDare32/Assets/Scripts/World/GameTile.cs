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
				_renderer.material.color = Color.green;
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

	public void SetTexture(string texPath)
	{
		Material tex = Resources.Load <Material>(texPath);
		this.gameObject.GetComponent<Renderer>().material = tex;
	}
}