using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UIManager : MonoBehaviour {



	private Color[] colors;


	[SerializeField]
	private GameObject cellToInstantiate;
	[SerializeField]
	private Transform gridParent;
	[SerializeField]
	private GameObject messageBox;
	[SerializeField]
	private Image messageBoxBackgroundImage;
	[SerializeField]
	private Text messageBoxText;
	[SerializeField]
	private Text score;


	private GameObject[,] cellGrid;




	// Use this for initialization
	void Start () {

		colors = new Color[Constants.COLORS_LENGTH];
		for (int i = 0; i < colors.Length; i++) {
			float lowerRange = i / 10f;
			float upperRange = Mathf.Min (lowerRange + 0.5f, 0.9f );
			colors [i] = new Color (Random.Range (lowerRange, upperRange),Random.Range (lowerRange, upperRange),Random.Range (lowerRange, upperRange),Constants.ALPHA_VALUE);
		}


		// Set MessageBox off
		messageBox.SetActive (false);

		IntializeCellGrid ();
	}

	/// <summary>
	/// Intializes the cell grid.
	/// </summary>
	private void IntializeCellGrid (){
		cellGrid = new GameObject[Constants.ROWS, Constants.COLS];
		for (int i = 0; i < Constants.ROWS; i++) {
			for (int j = 0; j < Constants.COLS; j++) {
				GameObject newCell = Instantiate (cellToInstantiate);
				newCell.name = i+""+j;
				newCell.transform.SetParent (gridParent);
				newCell.transform.localScale = Vector3.one;
				cellGrid[i,j] = newCell;
			}
		}
	}


	/// <summary>
	/// Updates the grid.
	/// </summary>
	/// <param name="grid">Grid.</param>
	public void UpdateGrid (int [,] grid)
	{
		for (int i = 0; i < Constants.ROWS; i++) {
			for (int j = 0; j < Constants.COLS; j++) {
				Text cellText = cellGrid[i,j].transform.GetChild (0).GetComponent<Text>();
				cellText.text = grid [i,j].ToString();
				cellText.color = Color.black;
				if(grid[i,j] != 0)
					cellGrid[i,j].GetComponent <Image>().color = colors[grid[i,j] - 1];
			}
		}
	}

	/// <summary>
	/// Raises the cell clicked event.
	/// </summary>
	/// <param name="gameObject">Game object.</param>
	public void OnCellClicked(GameObject clickedCell) {
		Debug.Log ("UIManager "+clickedCell.name);
	}

	/// <summary>
	/// Changes the color of the cell.
	/// </summary>
	/// <param name="r">The red component.</param>
	/// <param name="c">C.</param>
	/// <param name="color">Color.</param>
	public void ChangeCellColor (int r, int c, Color color) {
		Text cellText = cellGrid[r,c].transform.GetChild (0).GetComponent<Text>();
		cellText.color = Color.white;
		cellGrid [r, c].GetComponent <Image> ().color = color;	
	}

	/// <summary>
	/// Changes the color of the cell.
	/// </summary>
	/// <param name="r">The red component.</param>
	/// <param name="c">C.</param>
	public void ChangeCellColor (int r, int c) {
		Text cellText = cellGrid[r,c].transform.GetChild (0).GetComponent<Text>();
		cellText.color = Color.black;
		int colorIndex = int.Parse (cellText.text);
		cellGrid [r, c].GetComponent <Image> ().color = colors[colorIndex - 1];	
	}

	/// <summary>
	/// Shows the message box.
	/// </summary>
	/// <param name="isWon">If set to <c>true</c> is won.</param>
	public void ShowMessageBox (bool isWon){
		if (isWon) {
			messageBoxText.text = Constants.YOU_WON;
			messageBoxBackgroundImage.color = Color.green;
		} else {
			messageBoxText.text = Constants.YOU_LOSE;
			messageBoxBackgroundImage.color = Color.red;
		}
		messageBox.SetActive (true);
	}

	/// <summary>
	/// Raises the restart clicked event.
	/// </summary>
	public void OnRestartClicked ()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	/// <summary>
	/// Updates the score.
	/// </summary>
	/// <param name="s">S.</param>
	public void UpdateScore (string s)
	{
		score.text = s;
	}
}
