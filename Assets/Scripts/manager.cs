using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class manager : MonoBehaviour
{
    [SerializeField] float growSpeed = 0.05f;
    public Image ting;
    public IEnumerator AnimateMailbox() //typewriter effect
    {
        ting.gameObject.SetActive(true);
        for (int i = 0; i < (100); i++)
        {
            ting.gameObject.transform.localScale += new Vector3(.01f,.01f,0f);

            yield return new WaitForSeconds(growSpeed);
        }
    }

    private void Start()
    {
        ting.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click");
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.gameObject.CompareTag("Mailbox"))
                {
                    StartCoroutine(AnimateMailbox());
                }
            }
        }
    }
}
