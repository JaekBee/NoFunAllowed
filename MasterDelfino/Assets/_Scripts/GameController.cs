using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Text countText;
	public Text winText;

	private int count;

	void Start () {

		count = 0;
		setCountText ();
		countText.text = "Count: " + count.ToString ();
		winText.text = "";

	}

	void OnTriggerEnter(Collider other) {

		if (other.gameObject.CompareTag ("Pickup")) {

			other.gameObject.SetActive (false);
			count++;
			setCountText ();

		}

	}

	void setCountText() {

		countText.text = "Count: " + count.ToString ();
		if (count >= 4) {

			winText.text = "You win!";

		}

	}
}
