using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoueurScript : MonoBehaviour
{
	public Animator animateur;
	public Rigidbody2D rb;

	private float _vitesse = 10.0f;
	private float _maxSaut = 5f;
	private string _dernier;		
	private bool _estAuSol = true;
	private bool _estAuMurDroit = false;
	private bool _estAuMurGauche = false;
	private bool _saute = false;
	private Vector3 _posPers;
	private string[] _tabEtatMoustaches;
	private string[] _tabNomsMoustaches;

	// Start is called before the first frame update
	void Start()
	{		
		rb = gameObject.GetComponent<Rigidbody2D>();

		_tabEtatMoustaches = new string[GameObject.FindGameObjectsWithTag("Moustache").Length];
		_tabNomsMoustaches = new string[GameObject.FindGameObjectsWithTag("Moustache").Length];

		_posPers = transform.position;

		//Gestion des moustaches qui ont déjà été collectées
		int i = 0;
		string nomFichier = "NomMoustaches" + SceneManager.GetActiveScene().name.Split('_')[1] + ".txt";
		string chemin = "Assets/Ressources/";

		//On regarde si un fichier existe déjà (si il existe ca veut dire que le niveau à déjà été complété)
		if (File.Exists(chemin + nomFichier))
		{
			//Si le fichier existe on le lit et on remplit les tableaux
			StreamReader reader = new StreamReader(chemin + nomFichier);
			string ligne;
			
			//On remplit la tableau de noms avec les données du fichier
			while ((ligne = reader.ReadLine()) != null)
			{
				_tabNomsMoustaches[i] = ligne;
				i++;
			}
			i = 0;

			reader.Close();

			nomFichier = "EtatMoustaches" + SceneManager.GetActiveScene().name.Split('_')[1] + ".txt";
			reader = new StreamReader(chemin + nomFichier);

			while ((ligne = reader.ReadLine()) != null)
			{
				//On remplit le tableau d'états avec les données du fichier
				_tabEtatMoustaches[i] = ligne;
				i++;
			}
			i = 0;

			reader.Close();
		}
		else
		{
			//Si le fichier n'existe pas on remplit le tableau avec les valeurs par défaut
			foreach (GameObject o in GameObject.FindGameObjectsWithTag("Moustache"))
			{
				_tabNomsMoustaches[i] = o.name;
				//L'etat est sur false par défaut ce qui veut dire que la moustache n'a pas été ramassée
				_tabEtatMoustaches[i] = "false";
				i++;
			}
			i = 0;
		}

		//On cherche les moustaches qui ont déjà été ramassées pour les détruires
		for(int j = 0; j < _tabEtatMoustaches.Length; j++)
		{
			//Si la moustache à déjà été ramassée on la détruit pour qu'on ne puisse pas la ramasser de nouveau
			if(_tabEtatMoustaches[j].ToString() == "true")
			{
				Destroy(GameObject.Find(_tabNomsMoustaches[j]));
			}
		}

		//Gestion des animations en fonction du personnage choisi
		if (ControleurScript.personnageChoisi == "Super Meat Boy")
		{
			animateur.SetBool("Moustache", false);
		}
		else if (ControleurScript.personnageChoisi == "Super Super Meat Boy")
		{
			animateur.SetBool("Moustache", true);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		_saute = false;
		
		//Si le joueur touche le mur
		if(collision.gameObject.tag == "Mur")
		{
			//On regarde si le joueur est à droite ou à gauche du mur et on remplit les variables en fonction de ça
			if ((transform.position.x - collision.collider.transform.position.x) < 0)
			{
				_estAuMurDroit = true;
				_dernier = "MurDroit";
			}
			else if ((transform.position.x - collision.collider.transform.position.x) > 0)
			{
				_estAuMurGauche = true;
				_dernier = "MurGauche";
			}
		}
		else if (collision.gameObject.tag == "Sol")
		{
			_estAuSol = true;
			_dernier = "Sol";
		}
		else if(collision.gameObject.tag == "Obstacle")
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
			_dernier = "Obstacle";
		}
		else if(collision.gameObject.name == "Princesse")
		{
			//Lorsque le niveau se termine on écrit dans un fichier l'état des moustaches (collectée ou non) et dans un autre le nom des moustaches
			string nomScene = SceneManager.GetActiveScene().name;
			int numNvx = Convert.ToInt32(nomScene.Split('_')[1]) + 1;
			

			string nomFichier = "NomMoustaches" + (numNvx - 1) + ".txt";
			string chemin = "Assets/Ressources/";
			
			//on met le deuxieme paramètre  a false car il faut réécrire le fichier et non écrire à la suite
			StreamWriter writer = new StreamWriter(chemin + nomFichier, false);

			//On écrit dans le fichier pour stocker le nom des objets moustaches
			for (int i = 0; i < _tabNomsMoustaches.Length; i++)
			{
				writer.WriteLine(_tabNomsMoustaches[i]);
			}
			
			writer.Close();

			nomFichier = "EtatMoustaches" + (numNvx-1) + ".txt";
			writer = new StreamWriter(chemin + nomFichier, false);


			//On écrit dans un autre fichier pour stocker l'état des moustaches (ramassée ou non)
			for (int i = 0; i < _tabEtatMoustaches.Length; i++)
			{
				writer.WriteLine(_tabEtatMoustaches[i]);
			}

			writer.Close();

			//On change de scène pour aller à la suivante
			if (File.Exists("Assets/Scenes/SceneNiveau_" + numNvx + ".unity"))
			{
				SceneManager.LoadScene("SceneNiveau_" + numNvx, LoadSceneMode.Single);
			}
			else
			{
				//Si il n'y a pas de niveau suivant on charge la scène de fin
				SceneManager.LoadScene("SceneFin", LoadSceneMode.Single);
			}
		}
		else if (collision.gameObject.tag == "Moustache")
		{
			//Si une moustache est collectée on change son état dans le tableau;
			for(int i = 0; i < _tabEtatMoustaches.Length; i++)
			{
				if(collision.gameObject.name == _tabNomsMoustaches[i])
				{
					_tabEtatMoustaches[i] = "true";
				}
			}
			//on detruit la moustache collectée pour que le joueur ne puisse pas la reprendre
			Destroy(collision.gameObject);
			_dernier = "Moustache";
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		//Si le joueur n'est plus en collision avec le mur ou avec le sol on change l'état des variables
		if (collision.gameObject.tag == "Mur")
		{
			if ((transform.position.x - collision.collider.transform.position.x) < 0)
			{
				_estAuMurDroit = false;
			}
			else if ((transform.position.x - collision.collider.transform.position.x) > 0)
			{
				_estAuMurGauche = false;
			}

		}
		else if (collision.gameObject.tag == "Sol")
		{
			_estAuSol = false;
		}
	}	

	private void FixedUpdate()
	{
		float inputX = Input.GetAxis("Horizontal");
		
		// Gestion des variables pour les animations
		if(_estAuMurDroit || _estAuMurGauche)
		{
			animateur.SetBool("EstAuMur", true);
		}
		else if (_saute)
		{
			animateur.SetBool("EstAuMur", false);
			animateur.SetBool("Saute", true);
		}
		else if (inputX != 0 && _vitesse == 10f)
		{
			animateur.SetBool("EstAuMur", false);
			animateur.SetBool("Saute", false);
			animateur.SetFloat("VitesseJoueur", 1);
		}
		else if(inputX != 0 && _vitesse == 15f)
		{
			animateur.SetBool("EstAuMur", false);
			animateur.SetBool("Saute", false);
			animateur.SetFloat("VitesseJoueur", 3);
		}
		else
		{
			animateur.SetBool("EstAuMur", false);
			animateur.SetBool("Saute", false);
			animateur.SetFloat("VitesseJoueur", 0);
		}

		//Gestion du flip du personnage selon si le joueur va vers la gauche ou vers la droite ou est sur un mur
		if (!_estAuMurGauche && inputX == -1 || _estAuMurDroit)
		{
			gameObject.GetComponent<SpriteRenderer>().flipX = true;
		}
		else if (_estAuMurGauche || inputX == 1)
		{
			gameObject.GetComponent<SpriteRenderer>().flipX = false;
		}
		
		//Si le joueur _saute du mur et revient vers le mur on enlève la force
		if (_saute && ((_dernier == "MurGauche" && inputX == -1) || (_dernier == "MurDroit" && inputX == 1)))
		{
			rb.velocity = new Vector2(0, 0);
		}

		//Gestion des variables quand le joueur accelère
		if ((Input.GetKeyDown("joystick button 2") || Input.GetKeyDown("joystick button 3")))
		{
			_vitesse = 15f;
			_maxSaut = 7f;
			rb.gravityScale = 1.2f;
		}

		//Gestion des variable si le joueur n'accelère plus (on les remets par défaut)
		if ((Input.GetKeyUp("joystick button 2") || Input.GetKeyUp("joystick button 3")))
		{
			_vitesse = 10f;
			_maxSaut = 5f;
			rb.gravityScale = 0.8f;
		}

		//Vérification si le joueur appuie pour _sauter et si il n'est pas déjà en train de _sauter
		if ((Input.GetKeyDown("joystick button 0") || Input.GetKeyDown("joystick button 1")) && (_estAuSol || _estAuMurDroit || _estAuMurGauche))
		{
			
			_saute = true;

			rb.velocity = new Vector2(0, _maxSaut);
			
			//Si le joueur _saute depuis l'un des murs on lui applique une force opposée à la direction du mur pour le faire _sauter dans le sens inverse
			if (_estAuMurGauche || _estAuMurDroit)
			{
				//Si le joueur est au mur et _saute vers le mur
				if (_estAuMurGauche && Input.GetAxis("Horizontal") != -1)
				{
					rb.AddForce(new Vector2(200, 50));
				}
				else if (_estAuMurDroit && Input.GetAxis("Horizontal") != -1)
				{
					rb.AddForce(new Vector2(-200, 50));
				}
				//Si le joueur est au mur et _saute dans le sens opposé du mur
				else if (_estAuMurGauche)
				{
					rb.AddForce(new Vector2(800, -50));
				}
				else if (_estAuMurDroit)
				{
					rb.AddForce(new Vector2(-400, 50));
				}
			}
		}
			
		//Si le joueur est au sol et ne saute pas ou qu'il est au mur et ne saute pas on lui remet une force normale
		if ((_estAuSol || _estAuMurDroit || _estAuMurGauche) && !_saute)
		{
			rb.velocity = new Vector2(0, 0);
		}

		//Selon sa position on change sa masse et sa gravité
		if (_estAuSol)
		{
			rb.mass = 1f;
			rb.gravityScale = 0.8f;
		}	
		else if((_estAuMurDroit || _estAuMurGauche) && !_saute)
		{
			rb.mass = 10f;
			rb.gravityScale = 10f;
		}else if (_saute)
		{
			rb.mass = 1f;
			rb.gravityScale = 0.8f;
		}

		Vector3 direction = new Vector3(inputX, 0, 0);
		Vector3 pos = transform.position + direction * _vitesse * Time.deltaTime;

		if (calculSiDehors())
		{
			//Si le joueur est hors des limites de la caméra alors on lui fait recommencer le niveau
			SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
		}
		else if ((!_estAuMurGauche && !_estAuMurDroit) || _saute || _estAuSol)
		{
			//Si le joueur est dans les limites de la caméra alors on le fait bouger
			transform.position = pos;
		}
	}

	/// <summary>
	/// Fonction qui sert à savoir si le joueur est hors des limites de la caméra
	/// </summary>
	/// <returns>Retourne un booleen qui renvoie vrai si le joueur est hors des limites et faux si il est visible par la caméra</returns>
	public bool calculSiDehors()
	{
		Vector3 frontieres = GameObject.Find("Main Camera").GetComponent<CameraScript>().frontieres;
		bool dehors = false;
		if (gameObject.transform.position.x > frontieres.x)
		{
			dehors = true;
		}

		if (gameObject.transform.position.x < -frontieres.x)
		{
			dehors = true;
		}

		if (gameObject.transform.position.y > frontieres.y)
		{
			dehors = true;
		}

		if (gameObject.transform.position.y < -frontieres.y)
		{
			dehors = true;
		}

		return dehors;
	}

}
