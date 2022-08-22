using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PeekMee.Friends.Character
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController _controller;
        [SerializeField] private float _runSpeed;
        [SerializeField] private float _climbSpeed;
        private bool _jump;
        private bool _crouch;
        private float _horizontalMove;
        private float _verticalMove;

        void Start()
        {
            _jump = false;
            _crouch = false;
            _horizontalMove = 0f;
            _verticalMove = 0f;
        }

        private void Update()
        {
            _horizontalMove = Input.GetAxisRaw("Horizontal") * _runSpeed;
            _verticalMove = Input.GetAxisRaw("Vertical") * _climbSpeed;

            if (Input.GetKeyDown(KeyCode.Space)) _jump = true;
            if (Input.GetKeyDown(KeyCode.RightShift) ||
                Input.GetKeyDown(KeyCode.LeftShift)) _crouch = true;
            else if (Input.GetKeyUp(KeyCode.RightShift) ||
                Input.GetKeyUp(KeyCode.LeftShift)) _crouch = false; 

        }

        private void FixedUpdate()
        {
            _controller.Move(_horizontalMove * Time.fixedDeltaTime, 
                _verticalMove * Time.fixedDeltaTime, _crouch, _jump);
            _jump = false;
        }
    }
}

