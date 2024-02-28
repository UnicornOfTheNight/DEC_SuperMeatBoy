using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnnemiScript : MonoBehaviour
{
	public Animator animateur;

	private float _vitesse = 8.0f;
	private Vector3 _direction;
	private Vector3 _posDepartJoueur;
	private Vector3 _posDepartEnnemi;

	// Start is called before the first frame update
	void Start()
    {
		//On retient les positions de départ
		_posDepartEnnemi = transform.position;
		_posDepartJoueur = GameObject.Find("Joueur").transform.position;
	}

    // Update is called once per frame
    void Update()
    {
		//On crée un raycasthit2d pour que l'ennemi repère le joueur lorsque celui ci est proche, on regarde d'abord à la gauche de l'ennemi
		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.left), 8.0f, 1 << LayerMask.NameToLayer("Joueur"));

		//On regarde si l'ennemi repère le joueur à sa gauche
		if (hit.collider != null)
		{
			//Si le joueur est repéré l'ennemi se lance à sa poursuite
			_direction = hit.collider.gameObject.transform.position - transform.position;
			_direction.Normalize();
			transform.position += _direction * Time.deltaTime * _vitesse;
			//on change également le booléen de l'animation pour changer d'animation
			animateur.SetBool("Deplacement", true);
			//Si le joueur est sur la gauche il faut inverser le sprite
			gameObject.GetComponent<SpriteRenderer>().flipX = true;
		}
		else
		{
			//Si le joueur n'est pas repéré à gauche de l'ennemi on regarde à droite de l'ennemi
			hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), 8.0f, 1 << LayerMask.NameToLayer("Joueur"));

			if (hit.collider != null)
			{
				_direction = hit.collider.gameObject.transform.position - transform.position;
				_direction.Normalize();
				transform.position += _direction * Time.deltaTime * _vitesse;
				animateur.SetBool("Deplacement", true);
				gameObject.GetComponent<SpriteRenderer>().flipX = false;
			}
			else
			{
				//Si le joueur n'est ni sur la gauche ni sur la droite il reprend son animation de départ
				animateur.SetBool("Deplacement", false);
			}
		}
	}


	/// <summary>
	/// Gère la collision entre l'ennemi et les autres objets
	/// </summary>
	/// <param name="collision"></param>
	private void OnCollisionEnter2D(Collision2D collision)
	{
		//Si l'ennemi arrive a toucher le joueur alors le joueur doit recommencer le niveau
		if (collision.gameObject.name == "Joueur")
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
		}
		else if(collision.gameObject.tag == "Obstacle")
		{
			//Si l'ennemi touche un obstacle alors il meurt
			Destroy(gameObject);
		}
	}

}
