using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class FirstPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;//Aim Camera
    [SerializeField] private float normalSensitivity;//Normal Move Sensitivity Value
    [SerializeField] private float aimSensitivity;//Aim Sensitivity Value
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();//aimColliderMask
    [SerializeField] private LayerMask enemyMask;//Enemy Layer Mask
    [SerializeField] private Transform debugTransform;//
    [SerializeField] private Transform pfBulletProjectile;//Bullet 
    [SerializeField] private Transform spawnBulletPosition;//Bullet Spawn Point
    [SerializeField] private float shootTime;//Set Time for every shot
    private float timeBtwShots;//Time between each actual Shot
    public Vector3 weaponOriginalPos;//Weapon Pos;
    public Vector3 forwardOffset;//Recoil for Weapon
    public float recoilOffSetValue = .5f;
    [SerializeField] private GameObject[] itemGameObjects;//Inventory Items for Player
    [SerializeField] private GameObject activeItemGameObject;//Active Invetory Item On Player
    [SerializeField] private GameObject Weapon;//Player's Weapon Game Object
    [SerializeField] private GameObject Locator;//Player's Compass/Item Locator GameObject
    public GameObject radioActiveGO;//Radio Active Energy Source GameObject
    GameObject crossHair;//Cross Hair for Aiming
    GameObject hasRadioActiveGOIndicator;//Indicator if player has A Radioactive Energy Source

    public LayerMask whatIsEnemy;//Check EnemyAI layer Mask
    public float enemyInRange;//Range for which to check for Close EnemyAI to alert Player

    private PlayerController playerController;//Main Controller Script
    private PlayerStats playerStats;//Player Stats Script
    private PlayerInput playerInput;//Player Input Script
    private Animator animator;//Animator Component

    Transform hitTransform;//Raycast Hit point Transfrom Checker
    Vector3 mouseWorldPosition;//Ray cast World Vector Point
    public bool weaponEquiped;//WeaponActive Checker
    public bool locatorEquiped;//Locator or Compass Active Checker

    private void Awake()
    {
        //Confirm All Components are available and referenced Properly
        playerController = GetComponent<PlayerController>();
        playerStats = GetComponent<PlayerStats>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        crossHair = GameObject.Find("Crosshair");
        weaponOriginalPos = Weapon.transform.localPosition;
        forwardOffset = new Vector3(weaponOriginalPos.x, weaponOriginalPos.y, weaponOriginalPos.z - recoilOffSetValue);
        hasRadioActiveGOIndicator = GameObject.Find("RadioActiveGOIndicator");
    }
    
    private void Update()
    {
        if (playerController._isInteracting)//If player is interacting with UI, don't do anything Gameplay wise
            return;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);//Get Screen Dimension and Cast a ray for aiming
        Ray ray = new Ray(Camera.main.transform.position + Camera.main.transform.forward * 4, Camera.main.transform.forward);
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;//Ray point in world
            mouseWorldPosition = raycastHit.point;//set MouseWorld Point to that position
            hitTransform = raycastHit.transform;//same with hitTransform
        }

        if (Physics.Raycast(ray, out RaycastHit _raycastHit, 999f, enemyMask))
        {
            crossHair.GetComponent<Image>().color = Color.red;
        }

        if (playerInput.aim && weaponEquiped)//Get aim input and...
        {
            aimVirtualCamera.gameObject.SetActive(true);//Set aimCamera on
            playerController.SetSensitivity(aimSensitivity);//set aim sensitivity
        }
        else//Reset Aim to normal
        {
            aimVirtualCamera.gameObject.SetActive(false);
            playerController.SetSensitivity(normalSensitivity);
        }

        CheckForInventoryItem();//Check if Inventory Item is Close for Pickup

        //Take Invetory Item Input, Select and Use Item if Item is Available (i.e Dictionary value != 0)
        if (playerInput.useItemInputAmmo && Inventory.Instance.numberOfItems[Item.Ammo] != 0) SelectItem(Item.Ammo);
        if (playerInput.useItemInputFood && Inventory.Instance.numberOfItems[Item.Food] != 0) SelectItem(Item.Food);

        crossHair.SetActive(weaponEquiped);//See crosshair if weapon is equipped

        if (playerInput.toggleGun)//On Toggle Gun Input, set weaponEquip active or Inactive
        {
            if (!weaponEquiped)//Active and play Sound
            {
                weaponEquiped = true;
                playerController.WeaponEquipedSound();
            }
            else//not active and play sound
            {
                weaponEquiped = false;
                playerController.WeaponEquipedSound();
            }
            //Reset other items and variables
            locatorEquiped = false;
            radioActiveGO.SetActive(false);
            activeItemGameObject = null;
            playerInput.toggleGun = false;
            NotHolding();
        }

        if (playerInput.toggleLocator)//On Toggle Locator Input, set locatorEquipped active or Inactive
        {
            if (!locatorEquiped)//Active and play Sound
            {
                locatorEquiped = true;
                playerController.WeaponEquipedSound();
            }
            else//Not active and play sound
            {
                locatorEquiped = false;
                playerController.WeaponEquipedSound();
            }
            //Reset other items and variables
            weaponEquiped = false;
            radioActiveGO.SetActive(false);
            activeItemGameObject = null;
            playerInput.toggleLocator = false;
            NotHolding();
        }

        if (playerInput.useItemInputRadioActiveGO)//On Energy Source Input
        {
            if (radioActiveGO != null && Inventory.Instance.hasRadioActiveGO)//Check if player has item in inventory
            {
                //Activate if not active, deactivate when active... 
                if (!radioActiveGO.activeSelf)
                {
                    if (activeItemGameObject != null)
                        activeItemGameObject.SetActive(false);
                    radioActiveGO.SetActive(true);
                    activeItemGameObject = radioActiveGO;//Set as Active Inventory Item
                }
                else
                {
                    radioActiveGO.SetActive(false);
                    Inventory.Instance.indicator.SetActive(false);
                    activeItemGameObject = null;//Reset to null
                }
            }
            //Reset other items and variables
            locatorEquiped = false;
            weaponEquiped = false;
            playerInput.useItemInputRadioActiveGO = false;//Set back to false
        }

        if (playerInput.toggleInventory)//On Toggle Inventory Input, set Inventory GameObject active or Inactive
        {
            Inventory.Instance.ShowInventoryItem();
            Time.timeScale = 0.5f;//Slow down Time when Checking Inventory Items Value
        }
        else
        {
            Inventory.Instance.HideInventoryItem();//Hide and ...
            Time.timeScale = 1f;//Reset time back to play mode
        }

        Weapon.SetActive(weaponEquiped);//Set Weapon Gameobject Active/Inactive
        Locator.SetActive(locatorEquiped);//Set Locator Gameobject Active/Inactive
        hasRadioActiveGOIndicator.SetActive(Inventory.Instance.hasRadioActiveGO);//Set RadioActive Energy Indicator Active/Inactive

        if (playerInput.shoot && weaponEquiped && !playerController._isInteracting && playerStats.ammo > 0)
        {
            ShootWeapon();// Projectile Shoot
            crossHair.transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 2f, 5f * Time.deltaTime);
        }
        else
        {
            timeBtwShots = 0;//Reset Timebtw Shots
            Weapon.transform.localPosition = weaponOriginalPos;
            crossHair.transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 5f * Time.deltaTime);
        }
    }

    void ShootWeapon()//Call Shoot Method
    {
        if (timeBtwShots <= 0)
        {
            playerStats.ammo--;//Update Ammo Value
            Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            //pushBack Recoil...
            Weapon.transform.localPosition = forwardOffset;
            Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            timeBtwShots = shootTime;
            playerController.AnimShoot();
            //soundFX.PlayOneShot(gunSoundFX);
            
        }
        else
        {
            //Return From Recoils
            Weapon.transform.localPosition = weaponOriginalPos;
            timeBtwShots -= Time.deltaTime;//Reduce Shoot Time
        }
    }



    void CheckForInventoryItem()//Check if Inventory Item can be picked Up
    {
        if (Inventory.Instance.canReceiveInput && playerInput.pickUp)//Get Input
        {
            playerController.AnimPickUp();//Play Pick Up Anim
            Inventory.Instance.AddItem();//Add Item to Inventory List
        }
    }

    void SelectItem(Item itemType)//Select and Inventory Item
    {
        if (activeItemGameObject != null && itemType == Inventory.Instance.activeUsableItem)
        {
            Inventory.Instance.activeUsableItem = Item.Null;//Set to Null if Item is already active...
        }
        else
        {
            Inventory.Instance.SelectActiveItem(itemType);//If not active, then select and use item
            SelectActiveGameObject(itemType);
        }
        ResetInput();
    }

    void SelectActiveGameObject(Item itemType)//Assign the GameObject to The Selected Inventory Item 
    {
        int itemValue = ((int)itemType) - 1;

        for (int i = 0; i < itemGameObjects.Length; i++)
        {
            if (i == itemValue && itemType != Item.Null && Inventory.Instance.numberOfItems[itemType] > 0)
            {
                activeItemGameObject = itemGameObjects[i];//Set that to the active Item GO
            }
        }

        if (activeItemGameObject != null) UseEquipedItem();//Use Item On Selection
    }

    void NotHolding()
    {
        activeItemGameObject = null;
        if (activeItemGameObject != null)
            activeItemGameObject.SetActive(false);
    }

    void UseEquipedItem()// Check if Item is Usuable, not null and the Use Inventory Item
    {
        if (Inventory.Instance.activeUsableItem != Item.Null && Inventory.Instance.activeUsableItem != Item.RadioActiveGO)
        {
            if (Inventory.Instance.numberOfItems[Inventory.Instance.activeUsableItem] > 0)
            {
                Inventory.Instance.sfx.PlayOneShot(Inventory.Instance.useSfx);
                activeItemGameObject.GetComponent<I_InventoryItem>().Use();
            }
        }
        if (activeItemGameObject != null && activeItemGameObject.GetComponent<RadioActiveGO>() != null)
            activeItemGameObject.GetComponent<I_InventoryItem>().Use();
    }

    public void OnUsedEquiped()//Rest On UsedEquipped Item
    {
        SelectActiveGameObject(Item.Null);
    }

    void ResetInput()//Reset All Inventory Input
    {
        playerInput.useItemInputAmmo = false;
        playerInput.useItemInputFood = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyInRange);
    }
}
