using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Camera playerCamera;

    void Start()
    {
        if(playerCamera == null)
        {
            Debug.LogWarning($"Player camera needs to be assinged in {gameObject.name}");
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100))
            {
                EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();

                if (enemy != null)
                    enemy.Kill();
            }
        }
    }
}
