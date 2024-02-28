using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuNvxScript : MonoBehaviour
{
	public GameObject joueur;

	private Image _spriteJoueur;
	private bool _autorisationDroite;
	private bool _autorisationGauche;
	private float _distanceBtn;
	private int _nbDeplacements;
	private int _nbNvx;

	// Start is called before the first frame update
	void Start()
	{
		_nbNvx = 0;
		foreach (GameObject btn in GameObject.FindGameObjectsWithTag("BtnNiveau"))
		{
			//Pour chaque boutton (donc chaque niveau) on récupère le text (donc le numéro du niveau)
			string nvx = btn.GetComponent<Button>().GetComponentInChildren<Text>().text;
			//Pour chaque bouton on crée un évènement déclenché lors d'un clic, et on passe en paramètre le niveau grace à un délégué
			btn.GetComponent<Button>().onClick.AddListener(delegate { LoadScene(nvx); });
			_nbNvx++;
		}
		//On initialise les variables
		_autorisationGauche = true;
		_autorisationDroite = true;
		_spriteJoueur = joueur.GetComponent<Image>();
	}

	// Update is called once per frame
	void Update()
	{
		//On regarde le nombre de déplacements effectuer pour savoir si le joueur peut aller a droite ou a gauche
		if (_nbDeplacements == _nbNvx - 1)
		{
			_autorisationDroite = false;
		}
		if (_nbDeplacements == 0)
		{
			_autorisationGauche = false;
		}

		//Si le joueur veut aller dans une direction et qu'il y est autoriser on le déplace
		if (Input.GetAxis("Horizontal") == 1 && _autorisationDroite)
		{
			_nbDeplacements += 1;
			//On met l'autorisation a false car sinon le programme rerentre dans la condition hors on veut que le joueur ne se déplace qu'une fois
			_autorisationDroite = false;

			//On trouve la distance entre 2 boutons pour savoir de combien il fait déplacer le joueur
			_distanceBtn = Vector2.Distance(GameObject.Find("BtnNiveau").transform.position, GameObject.Find("BtnNiveau (1)").transform.position);
			_spriteJoueur.transform.position = new Vector3(_spriteJoueur.transform.position.x + _distanceBtn, _spriteJoueur.transform.position.y, _spriteJoueur.transform.position.z);
		}
		else if (Input.GetAxis("Horizontal") == -1 && _autorisationGauche)
		{
			_nbDeplacements -= 1;
			_autorisationGauche = false;

			//On trouve la distance entre 2 boutons pour savoir de combien il fait déplacer le joueur
			_distanceBtn = Vector2.Distance(GameObject.Find("BtnNiveau").transform.position, GameObject.Find("BtnNiveau (1)").transform.position);
			_spriteJoueur.transform.position = new Vector3(_spriteJoueur.transform.position.x - _distanceBtn, _spriteJoueur.transform.position.y, _spriteJoueur.transform.position.z);
		}
		else if (Input.GetAxis("Horizontal") == 0)
		{
			_autorisationDroite = true;
			_autorisationGauche = true;
		}
	}

	/// <summary>
	/// Fonction qui sert à charger la scène du niveau souhaité
	/// </summary>
	/// <param name="str"> Numéro du niveau à charger </param>
	public void LoadScene(string str)
	{
		string nomScene = "SceneNiveau_" + str;
		Debug.Log(nomScene);
		SceneManager.LoadScene(nomScene, LoadSceneMode.Single);
	}
}
