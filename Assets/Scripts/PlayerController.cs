using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Takes and handles input and movement for a player character
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;
    Vector2 movementInput;
    Rigidbody2D rigidbody;
    Animator animator;
    SpriteRenderer spriteRenderer;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        if (!canMove) return;

        // If movement input is not 0 try to move
        if (movementInput != Vector2.zero) {
            // TODO: Check if following lines can be refactored
            // Check for potential collisions
            bool success = TryMove(movementInput); // Try Moving everywhere
            if (!success) success = TryMove(new Vector2(movementInput.x, 0)); // Try Moving in x-axis
            if (!success) success = TryMove(new Vector2(0, movementInput.y)); // Try Moving in y-axis

            animator.SetBool("isMoving", success);
        } else animator.SetBool("isMoving", false);

        // Set the direction of sprite to movement direction
        if (movementInput.x < 0) spriteRenderer.flipX = true;
        else if (movementInput.x > 0) spriteRenderer.flipX = false;
    }

    private bool TryMove(Vector2 direction) {
        if (direction == Vector2.zero) return false; // Can't move if there's no direction to move in

        int count = rigidbody.Cast(
            direction, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
            movementFilter, // The settings that determine where a collision can occur on such as layers can collide with
            castCollisions, // List of collisions to store the found collisions into after the Cast is finished
            moveSpeed * Time.fixedDeltaTime + collisionOffset // The amount to cast equal to the movement plus an offset
        );

        if (count != 0) return false;
        else {
            rigidbody.MovePosition(rigidbody.position + direction * moveSpeed * Time.fixedDeltaTime);
            return true;
        }
    }

    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire() {
        animator.SetTrigger("swordAttack");
    }

    public void SwordAttack() {
        LockMovement();
        
        if (spriteRenderer.flipX) swordAttack.AttackLeft();
        else swordAttack.AttackRight();
    }

    public void EndSwordAttack() {
        UnlockMovement();

        swordAttack.StopAttack();
    }

    public void LockMovement() {
        canMove = false;
    }

    public void UnlockMovement() {
        canMove = true;
    }
}
