using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject tile_Prefab;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGround;
    [SerializeField] private bool keyCollected = false;
    [SerializeField] private Transform legs;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float jumpForce;
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private float playerSpeedMultiplier;
    [SerializeField] private float playerMovementX;
    [SerializeField] private Rigidbody2D playerRigidBody2D;
    [SerializeField] private AudioSource player_AudioSource;
    [SerializeField] private AudioSource player_AudioSourceNoPitch;
    [SerializeField] private AudioClip tilePlacingAudioClip;
    [SerializeField] private AudioClip playerJump_AudioClip;
    [SerializeField] private AudioClip keyCollected_AudioClip;
    [SerializeField] private LevelManager thisLevelManager;
    [SerializeField] private ParticleSystem keyDestroyed_Effect;
    RaycastHit hitInfo;

    private void Start()
    {
        playerRigidBody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        playerMovementX = Input.GetAxisRaw("Horizontal");

        playerRigidBody2D.linearVelocity = new Vector2(playerMovementX * playerSpeedMultiplier, playerRigidBody2D.linearVelocityY);

        if (Input.GetMouseButtonDown(0) && LevelManager.levelStarted)
        {
            Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool canPlace = NonPlacebleTilePos(clickPos);
            Vector3Int cellPos = tileMap.WorldToCell(clickPos);
            Vector3 cellWorldPos = tileMap.GetCellCenterWorld(cellPos);

            if (tileMap.GetTile(cellPos) == null && LevelManager.Instance.AvailableTiles() > 0 && canPlace)
            {
                tileMap.SetTile(cellPos, floorTile);
                LevelManager.Instance.TilesAvailabilityUpdate();
                player_AudioSource.PlayOneShot(tilePlacingAudioClip);
                Instantiate(tile_Prefab, cellWorldPos - Vector3.right, Quaternion.identity);
                StartCoroutine(DisappearTile(cellPos));
            }
        }
        isGround = Physics2D.OverlapCircle(legs.position, groundCheckRadius, groundLayer);
        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            player_AudioSource.PlayOneShot(playerJump_AudioClip);
            playerRigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        playerRigidBody2D.gravityScale = isGround ? 1 : 2;
    }

    IEnumerator DisappearTile(Vector3Int cellPos)
    {
        yield return new WaitForSeconds(2);
        tileMap.SetTile(cellPos, null);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(legs.position, groundCheckRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Key"))
        {
            player_AudioSourceNoPitch.PlayOneShot(keyCollected_AudioClip);
            thisLevelManager.PlayerCollectedKey();
            keyDestroyed_Effect.Play();
            Destroy(collision.gameObject);
        }
    }

    private bool NonPlacebleTilePos(Vector2 clickPos)
    {
        if(clickPos.y < transform.position.y + 1.1 && clickPos.y > transform.position.y - 1.1 && clickPos.x < transform.position.x + 1f && clickPos.x > transform.position.x - 1f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool KeyStatus()
    {
        return keyCollected;
    }
}
