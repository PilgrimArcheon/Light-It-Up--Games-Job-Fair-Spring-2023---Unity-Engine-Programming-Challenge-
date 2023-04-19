using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassBar : MonoBehaviour
{
	public GameObject iconPrefab;//Get a reference to the Icon Indicator
	List<ItemMarker> itemMakers = new List<ItemMarker>();//List of all Inventory Item Marker
	public RawImage compassImage;//Compass Image Holder... 
	public Transform player;//The Player Transform
	public TextMesh distToFinalPoint;// Distance to Current Mission Final Pos 

	public float maxDistance;//maxDistance the player has to be from the itemMarker to veiw it on compassBar
	float compassUnit;//Compass Holder RectWidth

	public ItemMarker finalGoal;// Current Mission ItemMarker Component 

	void Start() 
	{
		//Set all References and variables...
		player = GameObject.FindGameObjectWithTag("Player").transform;
		compassUnit = compassImage.rectTransform.rect.width / 360f;
		UpdateItemMarkers();//Get all and update the ItemMarkers for Navigation...
	}

	void Update() 
	{
		compassImage.uvRect = new Rect(player.localEulerAngles.y / 360f, 0f, 1f, 1f);
	
		foreach (ItemMarker _itemMarker in itemMakers)//For Every ItemMarker on the ItemMarkers List Update all info... (Position, Distance and Opacity)
		{
			_itemMarker.image.rectTransform.anchoredPosition = GetPosOnCompass(_itemMarker);

			float dst = Vector2.Distance (new Vector2(player.transform.position.x, player.transform.position.z), _itemMarker.position);
			
			if(_itemMarker.gameObject.tag != "FinalPoint")
			//Update Scale and opacity depending on the player's distance from ItemMarker Transform
			{
				float scale = 0f;
				if(dst < maxDistance)
				scale = 1f - ((dst - 10) / (maxDistance));

				_itemMarker.image.rectTransform.localScale = Vector3.one * scale;
				_itemMarker.image.color = Color.green;
				_itemMarker.image.color = new Color(_itemMarker.image.color.r, _itemMarker.image.color.g, _itemMarker.image.color.b, 1 * scale);
			}
			else 
			{
				if(dst >= 0) distToFinalPoint.text = "~. " + (dst - 5).ToString("00") + "M .~";//Show distance to Current Mission Final Point
			}
		}
	}

	public void UpdateItemMarkers()//Call the Update Update ItemMarkers...
	{
		Transform compassTransform = transform.GetChild(0);
		itemMakers.Clear();//Clear the itemsMarkers List
		foreach (Transform child in compassTransform)
		{
			Destroy(child.gameObject);//Destroy all the instantiated GameObjects of any Exists...
		}
		
		ItemMarker[] _itemMarkers = FindObjectsOfType<ItemMarker>();//Find All the Active Item Makers on the Scene
		foreach (ItemMarker _itemMarker in _itemMarkers)
		{
			AddItemMarker(_itemMarker);//Add them to the Makers' List
		}
	}

	public void AddItemMarker (ItemMarker marker)//Get the Item Marker image and then add to ItemMarkers' List 
	{
		GameObject newMarker = Instantiate(iconPrefab, compassImage.transform, false);
		marker.image = newMarker.GetComponent<Image>();
		newMarker.name = marker.gameObject.name;
		marker.image.sprite = marker.icon;

		itemMakers.Add(marker);
	}

	Vector2 GetPosOnCompass (ItemMarker marker)//Get the Position of the Item Marker on the Compass Canvas GameObject (RectTrans)
	{
		Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
		Vector2 playerFwd = new Vector2(player.transform.forward.x, player.transform.forward.z);

		float angle = Vector2.SignedAngle(marker.position - playerPos, playerFwd);

		return new Vector2(compassUnit * angle, 0f);
	} 
}
