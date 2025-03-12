using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

	public CameraFollow cameraFollow;
	int currentBirdIndex = 0;
	public SlingShot slingshot;
	[SerializeField] private bool overrideOtherGameManagers = false;
	[HideInInspector]
	public static GameState CurrentGameState = GameState.Start;
	private List<GameObject> Bricks;
	private List<GameObject> Birds;
	private List<GameObject> Pigs;

	private static GameManager instance;

	// Use this for initialization
	void Start()
	{
		CurrentGameState = GameState.Start;
		slingshot.enabled = false;
		//find all relevant game objects
		Bricks = new List<GameObject>(GameObject.FindGameObjectsWithTag("Brick"));
		Birds = new List<GameObject>(GameObject.FindGameObjectsWithTag("Bird"));
		Pigs = new List<GameObject>(GameObject.FindGameObjectsWithTag("Pig"));
		//unsubscribe and resubscribe from the event
		//this ensures that we subscribe only once
		slingshot.BirdThrown -= Slingshot_BirdThrown; slingshot.BirdThrown += Slingshot_BirdThrown;
	}


	// Update is called once per frame
	void Update()
	{
		switch (CurrentGameState)
		{
			case GameState.Start:
				//if player taps, begin animating the bird 
				//to the slingshot
				if (Input.GetMouseButtonUp(0))
				{
					AnimateBirdToSlingshot();
				}
				break;
			case GameState.BirdMovingToSlingshot:
				//do nothing
				break;
			case GameState.Playing:
				//if we have thrown a bird
				//and either everything has stopped moving
				//or there has been 50 seconds since we threw the bird
				//animate the camera to the start position
				if (slingshot.slingshotState == SlingshotState.BirdFlying &&
					(BricksBirdsPigsStoppedMoving() || Time.time - slingshot.TimeSinceThrown > 6.5f))
				{
					slingshot.enabled = false;
					AnimateCameraToStartPosition();
					CurrentGameState = GameState.BirdMovingToSlingshot;
				}
				break;
			//if we have won or lost, we will restart the level
			//in a normal game, we would show the "Won" screen 
			//and on tap the user would go to the next level
			case GameState.Won:
			case GameState.Lost:
				if (Input.GetMouseButtonUp(0))
				{
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				}
				break;
			default:
				break;
		}
	}


	/// <summary>
	/// A check whether all Pigs are null
	/// i.e. they have been destroyed
	/// </summary>
	/// <returns></returns>
	private bool AllPigsDestroyed()
	{
		return Pigs.All(x => x == null);
	}

	/// <summary>
	/// Animates the camera to the original location
	/// When it finishes, it checks if we have lost, won or we have other birds
	/// available to throw
	/// </summary>
	private void AnimateCameraToStartPosition()
	{
		bool lost = false;
		foreach (GameObject b in Birds)
		{
			if (b == null)
			{
				lost = true;
				continue;
			}
			else
			{
				if (b.GetComponent<Thrown>().thrown)
				{
					lost = true;
					continue;
				}
				else
				{
					lost = false;
					break;
				}

			}
		}
		float duration = Vector2.Distance(Camera.main.transform.position, cameraFollow.StartingPosition) / 10f;
		if (duration == 0.0f) duration = 0.1f;
		//animate the camera to start
		Camera.main.transform.DOMove(cameraFollow.StartingPosition, duration). //end position
			OnComplete(() =>
						{
							cameraFollow.IsFollowing = false;
							if (AllPigsDestroyed())
							{
								CurrentGameState = GameState.Won;
							}
							//animate the next bird, if available
							else if (lost)
							{
								//no more birds, go to finished
								CurrentGameState = GameState.Lost;
							}
							else
							{
								slingshot.slingshotState = SlingshotState.Idle;
								//bird to throw is the next on the list
								currentBirdIndex++;
								AnimateBirdToSlingshot();
							}
						});
	}

	/// <summary>
	/// Animates the bird from the waiting position to the slingshot
	/// </summary>
	void AnimateBirdToSlingshot()
	{
		CurrentGameState = GameState.BirdMovingToSlingshot;
		float closest = 100;
		foreach (GameObject b in Birds)
		{
			if (b == null) continue;
			else if (closest > Mathf.Abs(b.gameObject.transform.position.x - slingshot.BirdWaitPosition.transform.position.x))
			{
				closest = Mathf.Abs(b.gameObject.transform.position.x - slingshot.BirdWaitPosition.transform.position.x);
				currentBirdIndex = Birds.IndexOf(b);
			}
		}
		Birds[currentBirdIndex].GetComponent<Thrown>().thrown = true;
		Birds[currentBirdIndex].transform.DOMove
			(slingshot.BirdWaitPosition.transform.position, //final position
			Vector2.Distance(Birds[currentBirdIndex].transform.position / 10,
			slingshot.BirdWaitPosition.transform.position) / 10). //position
				OnComplete(() =>
						{
							CurrentGameState = GameState.Playing;
							slingshot.enabled = true; //enable slingshot
													  //current bird is the current in the list
							slingshot.BirdToThrow = Birds[currentBirdIndex];
						});
	}

	/// <summary>
	/// Event handler, when the bird is thrown, camera starts following it
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Slingshot_BirdThrown(object sender, System.EventArgs e)
	{
		cameraFollow.BirdToFollow.Add(Birds[currentBirdIndex].transform);
		cameraFollow.IsFollowing = true;
	}

	/// <summary>
	/// Check if all birds, pigs and bricks have stopped moving
	/// </summary>
	/// <returns></returns>
	bool BricksBirdsPigsStoppedMoving()
	{
		foreach (var item in Bricks.Union(Birds).Union(Pigs))
		{
			if (item != null && item.GetComponent<Rigidbody2D>().velocity.sqrMagnitude > Constants.MinVelocity)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Found here
	/// http://www.bensilvis.com/?p=500
	/// </summary>
	/// <param name="screenWidth"></param>
	/// <param name="screenHeight"></param>
	public static void AutoResize(int screenWidth, int screenHeight)
	{
		Vector2 resizeRatio = new Vector2((float)Screen.width / screenWidth, (float)Screen.height / screenHeight);
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(resizeRatio.x, resizeRatio.y, 1.0f));
	}

	/// <summary>
	/// Shows relevant GUI depending on the current game state
	/// </summary>
	void OnGUI()
	{
		AutoResize(800, 480);
		switch (CurrentGameState)
		{
			case GameState.Start:
				GUI.Label(new Rect(0, 150, 200, 100), "Tap the screen to start");
				break;
			case GameState.Won:
				GUI.Label(new Rect(0, 150, 200, 100), "You won! Tap the screen to restart");
				break;
			case GameState.Lost:
				GUI.Label(new Rect(0, 150, 200, 100), "You lost! Tap the screen to restart");
				break;
			default:
				break;
		}
	}

	public void AddBird(GameObject bird)
	{
		Birds.Add(bird);
	}

	public void RemoveBird(GameObject bird)
	{
		Birds.Remove(bird);
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			if (overrideOtherGameManagers)
			{
				Destroy(instance.gameObject);
				instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}

	public static GameManager Instance { get { return instance; } }

}
