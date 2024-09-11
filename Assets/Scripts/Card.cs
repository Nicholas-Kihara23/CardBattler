using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{

    public CardScriptableObject cardSO;
    public int currentHealth, attackPower, manaCost;

    //to reference the mana,health and attack power text
    public TMP_Text healthText, attackText, manaText, nameText, actionDescriptionText, loreText;

    public Image characterArt, bgArt;

    private Vector3 targetPoint;
    private Quaternion targetRotation;
    public float moveSpeed = 5f;
    public float rotateSpeed = 540f;

    public bool inHand;
    public int handPosition;

    private HandController theHC;

    private bool isSelected;
    private Collider theCol;

    public LayerMask whatIsDesktop, whatIsPlacement;
    private bool justPressed;

    public CardPlacePoint assignedPlace;

    // Start is called before the first frame update
    void Start()
    {
        SetupCard();
        theHC = FindObjectOfType<HandController>();
        theCol = GetComponent<Collider>();
    }
    public void SetupCard()
    {
        currentHealth = cardSO.currentHealth;
        attackPower = cardSO.attackPower;
        manaCost = cardSO.manaCost;

        healthText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
        manaText.text = manaCost.ToString();

        nameText.text = cardSO.cardName;
        actionDescriptionText.text = cardSO.actionDescription;
        loreText.text = cardSO.cardLore;

        characterArt.sprite = cardSO.characterSprite;
        bgArt.sprite = cardSO.backgroundSprite;
    }


    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        if (isSelected) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, whatIsDesktop )) 
            {
                MoveToPoint(hit.point , Quaternion.identity);
            
            }
            if (Input.GetMouseButtonUp(1)) 
            {
                
                ReturnToHand();
            
            }

            if (Input.GetMouseButtonDown(0) && justPressed == false) 
            {
                if (Physics.Raycast(ray, out hit, 100f, whatIsPlacement))
                {
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();
                    if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint)
                    {
                        selectedPoint.activeCard = this;
                        assignedPlace =selectedPoint;

                        MoveToPoint(selectedPoint.transform.position, Quaternion.identity);
                        inHand = false;
                        isSelected = false;

                        theHC.RemoveCardFromHand(this);

                    }
                    else 
                    {
                        ReturnToHand() ;
                    }

                }
                else
                {
                    ReturnToHand() ;
                
                }
                
            
            
            }
        
        }

        justPressed = false;
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRotation = rotToMatch;


    }

    private void OnMouseOver()
    {
        if (inHand)
        {
            MoveToPoint(theHC.cardPositions[handPosition] + new Vector3(0f, 0f, 0.5f), Quaternion.identity);

        }
    }

    private void OnMouseExit()
    {
        if (inHand)
        {
            MoveToPoint(theHC.cardPositions[handPosition], theHC.minPos.rotation);
        }
    }

    private void OnMouseDown()
    {
        if (inHand)
        {
            isSelected = true;
            theCol.enabled = false;
            justPressed = true;

        }
    }

    public void ReturnToHand()
    {
        isSelected = false;
        theCol.enabled = true;

        MoveToPoint(theHC.cardPositions[handPosition], theHC.minPos.rotation);

    }
}
