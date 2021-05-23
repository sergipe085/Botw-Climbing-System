using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeClimb : MonoBehaviour
{
    public Animator anim;
    public bool isClimbing = false;

    private bool inPosition;
    private bool isLerping;
    private float t;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Quaternion startRot;
    private Quaternion targetRot;
    public float positionOffset;
    public float offsetFromWall = 0.3f;
    public float speed = 0.2f;

    private Transform helper;
    private float delta;

    private void Start() {
        Init();
    }

    public void Init() {
        helper = new GameObject().transform;
        CheckForClimb();
    }

    private void Update() {
        delta += Time.deltaTime;

        Tick(delta);
    }

    public void Tick(float delta) {
        if (!inPosition) {
            GetInPosition();
            return;
        }
    }

    private void CheckForClimb() {
        Vector3 origin = transform.position;
        origin.y += 1.4f;
        Vector3 dir = transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, 5)) {
            InitClimb(hit);
        }
    }

    private void InitClimb(RaycastHit hit) {
        isClimbing = true;
        helper.rotation = Quaternion.LookRotation(-hit.normal);
        startPos = transform.position;
        targetPos = hit.point + (hit.normal * offsetFromWall);
        t = 0;
        inPosition = false;
        anim.CrossFade("climb_idle", 2);
    }

    private void GetInPosition() {
        t += delta;
        if (t > 1) {
            t = 1;
            inPosition = true;
        }

        Vector3 targetPosition = Vector3.Lerp(startPos, targetPos, t);
        transform.position = targetPosition;
    }

    private Vector3 PosWithOffset(Vector3 origin, Vector3 target) {
        Vector3 direction = origin - target;
        direction.Normalize();
        Vector3 offset = direction * offsetFromWall;
        return target + offset;
    }
}
