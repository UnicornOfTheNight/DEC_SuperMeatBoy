using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControleurScript : MonoBehaviour
{
	public Text TxtMinuterie;
	public GameObject menuPauseUI;

	private bool _estEnPause = false;
	
	static public string personnageChoisi = "Super Meat Boy";
	// Start is called before the first frame update
	void Start()
    {
		if (SceneManager.GetActiveScene().name == "SceneChoixPersonnage")
		{
			//On va chercher dans les fichiers d'etat pour savoir le nombre de moustaches récoltés
			string chemin = "Assets/Ressources/";
			string[] fichiers = Directory.GetFiles(chemin, "EtatMoustaches*.txt");
			int nbMoustaches = 0;
			foreach(string fichier in fichiers)
			{
				//On lit le fichier
				StreamReader reader = new StreamReader(fichier);
				string ligne;
				while ((ligne = reader.ReadLine()) != null)
				{
					if(ligne == "true")
					{
						nbMoustaches++;
					}
				}
			}

			//Selon le nombre de moustache on active ou non le personnage mystere
			if(nbMoustaches >= 2)
			{
				GameObject.Find("BtnPersoExtra").SetActive(true);
				GameObject.Find("PanelMystere").SetActive(false);
			}
			else
			{
				GameObject.Find("BtnPersoExtra").SetActive(false);
				GameObject.Find("PanelMystere").SetActive(true);
			}
		}
	}

	// Update is called once per frame
	void Update()
    {
		//Si la scène active est une scene de niveau alors on affiche la minuterie et on permet d'afficher le menu de pause	
		if(SceneManager.GetActiveScene().name != "SceneMenu" && SceneManager.GetActiveScene().name !=  "SceneChoixPersonnage" && SceneManager.GetActiveScene().name != "SceneFin")
		{
			TxtMinuterie.text = "Temps : " + Time.timeSinceLevelLoad.ToString("00:00.0");

			if (Input.GetKeyDown("joystick button 7") && _estEnPause)
			{
				reprendre();
			}
			else if (Input.GetKeyDown("joystick button 7") && _estEnPause == false)
			{
				pause();
			}
		}
		
	}

	/// <summary>
	/// Méthode qui sert à charger une scène
	/// </summary>
	/// <param name="nomScene">Nom de la scène à charger</param>
	public void menu(string nomScene)
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(nomScene, LoadSceneMode.Single);
	}

	/// <summary>
	/// Méthode pour quitter le jeu
	/// </summary>
	public void quitter()
	{
		Application.Quit();
	}

	/// <summary>
	/// Méthode pour afficher le menu de pause et mettre le jeu en pause
	/// </summary>
	private void pause()
	{
		menuPauseUI.SetActive(true);
		Time.timeScale = 0f;
		_estEnPause = true;
	}

	/// <summary>
	/// Méthode pour reprendre le jeu à partir du menu de pause
	/// </summary>
	public void reprendre()
	{
		menuPauseUI.SetActive(false);
		Time.timeScale = 1f;
		_estEnPause = false;
	}

	/// <summary>
	/// Méthode qui permet de donner une valeur à la variable statique
	/// </summary>
	/// <param name="nom">Nom du personnage choisi</param>
	public void PersonnageChoisi(string nom)
	{
		personnageChoisi = nom;
		SceneManager.LoadScene("SceneChoixNiveaux");
	}
}
