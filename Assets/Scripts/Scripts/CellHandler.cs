using UnityEngine;
using System.Collections;

public class CellHandler : MonoBehaviour {

	public void OnCellClicked()
	{
		GameManager.Instance.OnCellClicked (gameObject.name);
	}
}
