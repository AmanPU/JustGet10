using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private static GameManager instance;
	
	public static GameManager Instance {
		get {
			return instance;
		}
	}


	private Model model;
	private UIManager uiManager;
	private int score;

	void Awake () 
	{
		instance = this;
	}
	// Use this for initialization
	void Start () {
		score = 0;

		// Initialize UIManager
		uiManager = GetComponent <UIManager> ();

		// Create Model
		model = new Model ();
		model.GenerateIntialGrid ();

		// add listeners to events
		model.onCellAddedToAdjacent = OnCellAddedToAdjacent;
		model.onCellRemovedFromAdjacent = OnCellRemovedFromAdjacent;
		model.onGameOver = OnGameOver;

		// shows data to UI
		uiManager.UpdateGrid (model.CellGrid);
	}





	/// <summary>
	/// Raises the cell clicked event.
	/// </summary>
	/// <param name="clickedCell">Clicked cell.</param>
	public void OnCellClicked(string cellName) {
		int r = int.Parse (cellName [0].ToString());
		int c = int.Parse (cellName [1].ToString());
		Debug.Log ("( "+r+" ,"+c+" )");
		if (model.IsCellAlreadySelected (r, c)) {
			score += model.MergeCells (r,c);
			uiManager.UpdateGrid(model.CellGrid);
			uiManager.UpdateScore (score.ToString());
			if(!model.PossibleToProceed ()){
				OnGameOver (false);
			}
		} else {
			model.ProcessCell (r, c);
		}
	}

	/// <summary>
	/// Raises the cell added to adjacent event.
	/// </summary>
	/// <param name="r">The red component.</param>
	/// <param name="c">C.</param>
	public void OnCellAddedToAdjacent (int r, int c){
		uiManager.ChangeCellColor (r,c,Color.gray);
	}

	/// <summary>
	/// Raises the cell removed from adjacent event.
	/// </summary>
	/// <param name="r">The red component.</param>
	/// <param name="c">C.</param>
	public void OnCellRemovedFromAdjacent (int r, int c){
		uiManager.ChangeCellColor (r,c);
	}

	/// <summary>
	/// Raises the game over event.
	/// </summary>
	/// <param name="isWon">If set to <c>true</c> is won.</param>
	private void OnGameOver (bool isWon) {
		uiManager.ShowMessageBox (isWon);
	}

}
