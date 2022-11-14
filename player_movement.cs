using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class player_movement : MonoBehaviour
{
    private Rigidbody2D body_;
    private BoxCollider2D collider_;


    [SerializeField] private float speed_;
    [SerializeField] private float jump_force_;
    [SerializeField] private float gravity_;
    [SerializeField] private float fall_gravity_multiplier_;
    [SerializeField] private float jump_gravity_multiplier_;
    [SerializeField] private LayerMask ground_layer_;

    [SerializeField] private float jump_cache_time_;
    private float jump_cache_timer_;

    [SerializeField] private float cayote_time_;
    private float grounded_timer_ = 0.0f;

    private bool is_grounded_ { get { return grounded_timer_ > .0f; } }


    private void Start()
    {
        body_ = GetComponent<Rigidbody2D>();
        collider_ = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        CheckAndSetGrounded();
        HandleHorizontalMove();
        HandleJump();
    }


    private void HandleHorizontalMove()
    {
        float horizontal_input = Input.GetAxis("Horizontal");
        transform.Translate(Vector2.right * speed_ * Time.deltaTime * horizontal_input);
    }

    private void HandleJump()
    {
        //This is to cache the pressed jump button so the player can press jump before landing and still jump
        if (Input.GetButtonDown("Jump")) jump_cache_timer_ = jump_cache_time_;
        jump_cache_timer_ -= Time.deltaTime;

        if (is_grounded_ && jump_cache_timer_ > .0f)
        {
            body_.velocity = Vector2.up * jump_force_;
            jump_cache_timer_ = .0f;
        }

        ApplyAirGravity();
    }

    private void ApplyAirGravity()
    {
        if (body_.velocity.y < 0.0f)
        {
            body_.velocity += Vector2.up * Physics2D.gravity.y * (fall_gravity_multiplier_ - 1) * Time.deltaTime;
        }
        else if (body_.velocity.y > 0.0f && !Input.GetButton("Jump")) //This part is to jump longer when the jump button is pressed longer.
        {
            body_.velocity += Vector2.up * Physics2D.gravity.y * (jump_gravity_multiplier_ - 1) * Time.deltaTime;
        }
    }

    private void CheckAndSetGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(collider_.bounds.center, collider_.size, .0f, Vector2.down, 0.1f, ground_layer_);

        if (hit.collider != null) grounded_timer_ = cayote_time_;

        grounded_timer_ -= Time.deltaTime;

        SetGravity();
    }

    private void SetGravity()
    {
        //Set gravity to 0 whilst the grounded_timer_ is greater than 0 to allow the cayote jump.
        if (grounded_timer_ > .0f) body_.gravityScale = 0.0f;
        else body_.gravityScale = gravity_;
    }
}
