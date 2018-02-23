using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Model {
	public Action <int,int> onCellAddedToAdjacent;
	public Action <int,int> onCellRemovedFromAdjacent;
	public Action <bool> onGameOver;

	/// <summary>
	/// The cell grid.
	/// </summary>
	private int [,] cellGrid;

	public int [,] CellGrid {
		get {
			return cellGrid;
		}
	}

	private int [][] offset  = new int[][]{
		new int[]{ -1, 0},
		new int[]{ 0, -1},
		new int[]{ 0, 1},
		new int[]{ 1, 0}
	};

	private IList<Coords> listOfAdjacentCells;


	/// <summary>
	/// Initializes a new instance of the <see cref="Model"/> class.
	/// </summary>
	public Model (){
		cellGrid = new int[Constants.ROWS,Constants.COLS];
		listOfAdjacentCells = new List<Coords> ();
	}

	/// <summary>
	/// Generates the intial grid.
	/// </summary>
	public void GenerateIntialGrid ()
	{
		for (int i = 0; i < Constants.ROWS; i++) {
			for (int j = 0; j < Constants.COLS; j++) {
				cellGrid [i,j] = UnityEngine.Random.Range (1, 5);
			}
		}
		// In case if initial generated random grid can not proceed further
		if ( !PossibleToProceed ()) {
			GenerateIntialGrid ();
		}
	}

	/// <summary>
	/// Determines whether this instance is cell valid the specified r c.
	/// </summary>
	/// <returns><c>true</c> if this instance is cell valid the specified r c; otherwise, <c>false</c>.</returns>
	/// <param name="r">Row</param>
	/// <param name="c">Column</param>
	private bool IsCellValid (int r, int c)
	{

		if (r < 0 || c < 0 || r >= Constants.ROWS || c >= Constants.COLS) {
			return false;
		}
		return true;
	}


	public void ProcessCell (int r, int c)
	{
		// Remove all the adjacents cells from list
		RemoveAdjacentCells ();

		// add current cell
		listOfAdjacentCells.Add (new Coords (r, c));
		onCellAddedToAdjacent (r,c);

		// add all other cells
		CheckAdjacentCell (r, c);
	}

	/// <summary>
	/// A recursive method to add all the adjacent cells to list which has same value
	/// </summary>
	/// <param name="r">Row</param>
	/// <param name="c">Column</param>
	private void CheckAdjacentCell (int r, int c) 
	{
		int cellValue = CellGrid [r, c];
		foreach (int[] item in offset) {
			int offsetR = r + item[0];
			int offsetC = c + item[1];
			if (IsCellValid (r + item[0], c + item[1]) && cellValue == CellGrid [offsetR, offsetC]) {
				Coords coords = new Coords ( offsetR, offsetC);
				if(!listOfAdjacentCells.Contains (coords)){
					listOfAdjacentCells.Add (coords);
					onCellAddedToAdjacent(coords.r, coords.c);
					CheckAdjacentCell (offsetR, offsetC);
				}
			}
		}

		if (IsCompleted (r, c) && HasNoAdjacentCells()) {
			RemoveAdjacentCells ();
		}
	}

	/// <summary>
	/// Removes the adjacent cells.
	/// </summary>
	private void RemoveAdjacentCells ()
	{
		for (int i = 0; i < listOfAdjacentCells.Count; i++) {
			Coords coords = listOfAdjacentCells[i];
			onCellRemovedFromAdjacent(coords.r,coords.c);
		}
		listOfAdjacentCells.Clear ();
	}

	/// <summary>
	/// Determines whether this instance is completed the specified r c.
	/// </summary>
	/// <returns><c>true</c> if this instance is completed the specified r c; otherwise, <c>false</c>.</returns>
	/// <param name="r">Row</param>
	/// <param name="c">Column</param>
	private bool IsCompleted(int r, int c)
	{
		if (new Coords (r, c).Equals(listOfAdjacentCells [0])) {
			return true;
		}
		return false;
	}
	/// <summary>
	/// Determines whether this instance has no adjacent cells.
	/// </summary>
	/// <returns><c>true</c> if this instance has no adjacent cells; otherwise, <c>false</c>.</returns>
	private bool HasNoAdjacentCells ()
	{
		if (listOfAdjacentCells.Count == 1) {
			return true;
		}
		return false;
	}

	/// <summary>
	/// Determines whether this instance is cell already selected the specified r c.
	/// </summary>
	/// <returns><c>true</c> if this instance is cell already selected the specified r c; otherwise, <c>false</c>.</returns>
	/// <param name="r">Row</param>
	/// <param name="c">Column</param>
	public bool IsCellAlreadySelected (int r, int c)
	{
		return listOfAdjacentCells.Contains (new Coords(r,c));
	}

	/// <summary>
	/// Merges the cells.
	/// </summary>
	/// <returns>Score gained from merging</returns>
	/// <param name="r">Row</param>
	/// <param name="c">Column</param>
	public int MergeCells (int r, int c) 
	{
		// for scoring purposes only
		int listCount = listOfAdjacentCells.Count;

		// increment the clicked cell 
		cellGrid [r, c]++;

		// check if we reached WIN_SCORE which 10
		if (cellGrid [r, c] == Constants.WIN_SCORE) {
			onGameOver (true);
		}

		// removes current cell
		listOfAdjacentCells.Remove (new Coords (r, c));

		// sort the cell according to rows
		listOfAdjacentCells = listOfAdjacentCells.OrderBy(x => x.r).ToList();


		for (int i = 0; i < listOfAdjacentCells.Count; i++) {
			RearrangeCell (listOfAdjacentCells[i].r, listOfAdjacentCells[i].c);
		}


		RemoveAdjacentCells ();


		return cellGrid [r, c] * listCount; 
	}

	/// <summary>
	/// Rearranges the cell.
	/// </summary>
	/// <param name="r">Row</param>
	/// <param name="c">Column</param>
	private void RearrangeCell (int r,int c){
		if (IsCellValid (r - 1, c)) {
			cellGrid [ r, c ] = cellGrid [r - 1, c];
			RearrangeCell (r - 1, c);
		} else {
			cellGrid [ r, c ] = UnityEngine.Random.Range (1, 5);
		}
	}

	/// <summary>
	/// Possibles to proceed.
	/// </summary>
	/// <returns><c>true</c>, if to proceed was possibled, <c>false</c> otherwise.</returns>
	public bool PossibleToProceed (){
		for (int i = 0; i < Constants.ROWS; i++) {
			for (int j = 0; j < Constants.COLS; j++) {
				if( HasAdjacentCell( i,j ) ){
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Determines whether this instance has adjacent cell the specified r c.
	/// </summary>
	/// <returns><c>true</c> if this instance has adjacent cell the specified r c; otherwise, <c>false</c>.</returns>
	/// <param name="r">Row</param>
	/// <param name="c">Column</param>
	private bool HasAdjacentCell (int r, int c)
	{
		int cellValue = CellGrid [r, c];
		foreach (int[] item in offset) {
			int offsetR = r + item[0];
			int offsetC = c + item[1];
			if (IsCellValid (r + item[0], c + item[1]) && cellValue == CellGrid [offsetR, offsetC]) {
				return true;
			}
		}
		return false;
	}


	private struct Coords
	{
		public int r;
		public int c;

		public Coords (int r, int c) {
			this.r = r;
			this.c = c;
		}
		public override string ToString ()
		{
			return string.Format ("( "+r+","+c+" )");
		}
	}


}
