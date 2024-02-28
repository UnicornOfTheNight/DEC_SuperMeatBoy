using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
	public GameObject personnage;
	public GameObject limiteGauche;
	public GameObject limiteDroite;
	public GameObject limiteHaut;
	public GameObject Ennemi;

	public Vector3 frontieres;
	private Vector3 _posDepartCamera;
	private Vector3 _posDepartJoueur;
	
	private float _moveX = 0f;
	private float _moveY = 0f;	

	//Start is called before the first frame update
	void Start()
    {
		//On enregistre les positions de la caméra et du joueur au début du jeu
		_posDepartCamera = gameObject.transform.position;
		_posDepartJoueur = personnage.transform.position;
	}

	// Update is called once per frame
	void Update()
    {
		//Calcul des limites de la caméra
		float dist = (transform.position - Camera.main.transform.position).z;
		float maxX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
		float maxY = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, dist)).y;
		frontieres = new Vector3(maxX, maxY, dist);

		//Si le joueur est à plus de la moitié de la caméra et que la caméra n'a pas atteint ses limites alors elle suit le joueur
		if (personnage.transform.position.x > 0 && personnage.transform.position.x > limiteGauche.transform.position.x && personnage.transform.position.x < limiteDroite.transform.position.x)
		{
			_moveX = personnage.transform.position.x;
		}
		if (personnage.transform.position.y > 0 && personnage.transform.position.y < limiteHaut.transform.position.y)
		{
			_moveY = personnage.transform.position.y;
		}

		//Si le joueur est mort et qu'il est retourné au point de départ la caméra retrouve elle aussi sa position de départ
		if (personnage.transform.position == _posDepartJoueur || personnage.GetComponent<JoueurScript>().calculSiDehors())
		{
			transform.position = _posDepartCamera;
			_moveX = 0f;
			_moveY = 0f;
		}
		else{
			transform.position = new Vector3(_moveX, _moveY, transform.position.z);
		}
	}
}
