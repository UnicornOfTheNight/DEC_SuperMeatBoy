using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoueScript : MonoBehaviour
{
	private float _zAngle;
	public Sprite roue0;
	public Sprite roue1;
	public Sprite roue2;
	public Sprite roue3;
	
    // Start is called before the first frame update
    void Start()
    {
		_zAngle = 5f;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		//Selon l'état de la roue on change le sprite lorsque le joueur la touche
		if(collision.gameObject.name== "Joueur")
		{
			if (gameObject.GetComponent<SpriteRenderer>().sprite == roue0)
				gameObject.GetComponent<SpriteRenderer>().sprite = roue1;
			else if (gameObject.GetComponent<SpriteRenderer>().sprite == roue1)
				gameObject.GetComponent<SpriteRenderer>().sprite = roue2;
			else if (gameObject.GetComponent<SpriteRenderer>().sprite == roue2)
				gameObject.GetComponent<SpriteRenderer>().sprite = roue3;
		}
	}

	// Update is called once per frame
	void Update()
    {
		//On fait tourner la roue avec l'angle définit dans le start
		gameObject.transform.Rotate(0, 0, _zAngle);
	}
}
