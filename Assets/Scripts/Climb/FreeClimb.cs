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
    public float climbSpeed = 3f;
    public float rotateSpeed = 5f;
    public float inAngleDis = 1;
    public float moveAmount;

    private Transform helper;
    private float delta;

    public float horizontal;
    public float vertical;

    private void Start() {
        Init();
    }

    public void Init() {
        helper = new GameObject().transform;
        CheckForClimb();
    }

    private void Update() {
        delta = Time.deltaTime;

        Tick(delta);
    }

    public void Tick(float delta) {
        if (!inPosition) {
            GetInPosition();
            return;
        }

        if (!isLerping) {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            //float moveAmount = Mathf.Abs(horizontal) + Mathf.Abs(vertical);

            Vector3 h = helper.right * horizontal;
            Vector3 v = helper.up * vertical;
            Vector3 moveDir = (h + v).normalized * moveAmount;

            bool canMove = CanMove(moveDir);
            if (!canMove || moveDir == Vector3.zero) {
                return;
            }

            t = 0;
            isLerping = true;
            startPos = transform.position;
            //Vector3 tp = helper.position - transform.position;
            targetPos = helper.position;
        }
        else {
            t += delta * climbSpeed;
            if (t > 1) {
                t = 1;
                isLerping = false;
            }

            Vector3 cp = Vector3.Lerp(startPos, targetPos, t);
            transform.position = cp;
            transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotateSpeed);
        }
    }

    bool CanMove(Vector3 moveDir) {
        Vector3 origin = transform.position;
        float dis = positionOffset;
        Vector3 dir = moveDir;
        Debug.DrawRay(origin, dir * dis, Color.blue);
        RaycastHit hit;

        if (Physics.Raycast(origin, dir, out hit, dis)) {
            return false;
        }

        origin += moveDir * dis;
        dir = helper.forward;
        float dis2 = inAngleDis;
        Debug.DrawRay(origin, dir * dis2, Color.red);

        if (Physics.Raycast(origin, dir, out hit, dis)) {
            helper.position = PosWithOffset(origin, hit.point);
            helper.rotation = Quaternion.LookRotation(-hit.normal);
            return true;
        }

        origin += dir * dis2;
        dir = -Vector3.up;

        Debug.DrawRay(origin, dir, Color.yellow);
        if (Physics.Raycast(origin, dir, out hit, dis2)) {
            float angle = Vector3.Angle(helper.up, hit.normal);
            if (angle < 40) {
                helper.position = PosWithOffset(origin, hit.point);
                helper.rotation = Quaternion.LookRotation(-hit.normal);
                return true;
            }
        }

        return false;
    }

    private void CheckForClimb() {
        Vector3 origin = transform.position;
        origin.y += 1.4f;
        Vector3 dir = transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, 5)) {
            helper.position = PosWithOffset(origin, hit.point);
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

            //enable ik
        }

        Vector3 targetPosition = Vector3.Lerp(startPos, targetPos, t);
        transform.position = targetPosition;
        transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotateSpeed);
    }

    private Vector3 PosWithOffset(Vector3 origin, Vector3 target) {
        Vector3 direction = origin - target;
        direction.Normalize();
        Vector3 offset = direction * offsetFromWall;
        return target + offset;
    }
}
