using Godot;
using System;

public class Player : KinematicBody2D
{
    // Emit signals to use them on SFX later
    [Signal] delegate void PlayerPressing();
    [Signal] delegate void PlayerJumped();
    [Signal] delegate void PlayerLanded(float landingSpeed);
    [Signal] delegate void PlayerAdvanced(int score);
    [Signal] delegate void PlayerDied();

    [Export] public float jumpPower = 1200f;
    [Export] public float gravity = 800f;
    public int score {get; private set;} = 0;

    private float timePressed = 0;
    private bool isJumping = false;
    private Node2D lastPlatform = null;
    private Node2D rootNode = null;
    private Vector2 velocity = Vector2.Zero;
    private Vector2 prevPos;

    public override void _Ready()
    {
        rootNode = (Node2D)GetOwner();
        lastPlatform = (Node2D)GetParent();
    }

    public override void _Process(float delta)
    {
        if (isJumping)
        {
            velocity.y += gravity * delta;
            KinematicCollision2D collisionData = MoveAndCollide(velocity * delta);

            // If the player is dead
            if (GlobalPosition.y > 1100)
                EmitSignal("PlayerDied", score);

            // If the player is landed
            if (collisionData != null)
            {
                Node2D collidedPlatform = (Node2D)collisionData.Collider;
                ChangeParent(this, collidedPlatform);
                isJumping = false;
                if (lastPlatform != collidedPlatform)
                {
                    score++;
                    float distance = prevPos.y - GetPosition().y;
                    EmitSignal("PlayerAdvanced", score);
                }
                lastPlatform = collidedPlatform;
                Input.ActionRelease("jump");
                EmitSignal("PlayerLanded", velocity.y);
            }
        }
        else
        {
            if (Input.IsActionJustPressed("jump"))
                EmitSignal("PlayerPressing");

            if (Input.IsActionPressed("jump"))
            {
                timePressed += delta;
                if (timePressed > 2)
                    Input.ActionRelease("jump");
            }

            if (Input.IsActionJustReleased("jump"))
            {
                ChangeParent(this, rootNode);
                velocity.y = Mathf.Sqrt(timePressed) * -jumpPower;
                timePressed = 0;
                isJumping = true;
                EmitSignal("PlayerJumped");
                prevPos = GetPosition();
            }
        }
    }

    private void ChangeParent(Node2D child, Node2D newParent)
    {
        Vector2 prevPos = child.GlobalPosition;
        Vector2 prevScale = GetGlobalScale();
        child.GetParent().RemoveChild(child);
        newParent.AddChild(child);
        child.SetGlobalPosition(prevPos);
        child.SetGlobalScale(prevScale);
    }
}
