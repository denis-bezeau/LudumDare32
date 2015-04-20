using UnityEngine;
using System.Collections;

public class TrapDecal : MonoBehaviour 
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	// Sprites

	[SerializeField]
	private Sprite _plantSptire;

	[SerializeField]
	private Sprite _doorSprite;

	[SerializeField]
	private Sprite _marbleSprite;

	[SerializeField]
	private Sprite _nullSprite;

	public float ActiveAlpha = 0.7f;

	void Update()
	{
		Vector3 mousePos = Input.mousePosition;

		if (GameManager.GetInstance().IsPlacingTrap())
		{
			_spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, ActiveAlpha);

			if(GameManager.GetInstance().CurrentSelectedTrap == Trap.TrapType.Door)
			{
				_spriteRenderer.sprite = _doorSprite;
			}
			else if (GameManager.GetInstance().CurrentSelectedTrap == Trap.TrapType.Plant)
			{
				_spriteRenderer.sprite = _plantSptire;
			}
			else if (GameManager.GetInstance().CurrentSelectedTrap == Trap.TrapType.Marble)
			{
				_spriteRenderer.sprite = _marbleSprite;
			}
			else
			{
				_spriteRenderer.sprite = _nullSprite;
			}
		
			RaycastHit hit;
			Ray mousePosRay = Camera.main.ScreenPointToRay(mousePos);
			
			if(Physics.Raycast(mousePosRay, out hit))
			{
				GameTile tileHit = hit.collider.gameObject.GetComponent<GameTile>();
				if(tileHit != null)
				{
					_spriteRenderer.material.color = Color.red;
					this.transform.position = tileHit.transform.position;
					if (GameManager.GetInstance().IsTileValid(tileHit))
					{
						if((tileHit.parentRoom != null) && (tileHit.parentRoom.CanHaveTraps))
						{
							_spriteRenderer.material.color = Color.green;
						}
						else if (tileHit.parentRoom == null)
						{
							_spriteRenderer.material.color = Color.green;
						}
					}
				}
			}
		}
		else
		{
			_spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0.0f);
		}
	}
}
