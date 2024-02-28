using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FonteMurScript : MonoBehaviour
{
	private bool _fond;
	// Start is called before the first frame update
	void Start()
    {
		//De base les murs ne sont pas en train de disparaitre
		_fond = false;
	}

	/// <summary>
	/// Permet de changer la variable _fond si le joueur touche le mur afin de le faire disparaitre dans le update
	/// </summary>
	/// <param name="collision">Object qui est entré en collision avec le mur</param>
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.name == "Joueur")
		{
			_fond = true;
		}			
	}

	// Update is called once per frame
	void Update()
    {
		//Si le joueur entre en collision avec un mur qui disparait alors on fait disparaitre le mur
		if (_fond)
		{
			//On joue sur la transparence pour le rendre invisible
			Color alphaColor = gameObject.GetComponent<SpriteRenderer>().color;
			alphaColor.a = 0;
			gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(gameObject.GetComponent<SpriteRenderer>().color, alphaColor, 1.0f * Time.deltaTime);
		}

		if(gameObject.GetComponent<SpriteRenderer>().color.a < 0.18)
		{
			//Quand le mur à attaint une certaine transparence on le désactive
			gameObject.SetActive(false);
		}
	}
}
