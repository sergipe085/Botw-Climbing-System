using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeClimbAnimHook : MonoBehaviour
{
    private Animator anim;
    private IKSnapshot ikBase;
    private IKSnapshot current = new IKSnapshot();
    private IKSnapshot next = new IKSnapshot();

    public float w_rh, w_lh, w_rf, w_lf;

    private Vector3 rh, lh, rf, lf;
    Transform helper;

    public void Init(FreeClimb c, Transform _helper) {
        anim = c.anim;
        ikBase = c.baseIkSnapshot;
        helper = _helper;
    }

    public void CreatePositions(Vector3 origin) {
        IKSnapshot ik = CreateSnapshot(origin);
        CopySnapshot(ik, ref current);

        UpdateIKPosition(AvatarIKGoal.LeftFoot, current.lf);
        UpdateIKPosition(AvatarIKGoal.RightFoot, current.rf);
        UpdateIKPosition(AvatarIKGoal.LeftHand, current.lh);
        UpdateIKPosition(AvatarIKGoal.RightHand, current.rh);

        UpdateIKWeight(AvatarIKGoal.LeftFoot, 1);
        UpdateIKWeight(AvatarIKGoal.RightFoot, 1);
        UpdateIKWeight(AvatarIKGoal.LeftHand, 1);
        UpdateIKWeight(AvatarIKGoal.RightHand, 1);
    }

    public IKSnapshot CreateSnapshot(Vector3 origin) {
        IKSnapshot r = new IKSnapshot();
        r.lh = LocalToWorld(ikBase.lh);
        r.rh = LocalToWorld(ikBase.rh);
        r.lf = LocalToWorld(ikBase.lf);
        r.rf = LocalToWorld(ikBase.rf);
        return r;
    }

    public void CopySnapshot(IKSnapshot from, ref IKSnapshot to) {
        to.rh = from.rh;
        to.lh = from.lh;
        to.rf = from.rf;
        to.lf = from.lf;
    }

    public void UpdateIKPosition(AvatarIKGoal goal, Vector3 pos) {
        switch(goal) {
            case AvatarIKGoal.LeftFoot:
                print("a");
                lf = pos;
                break;
            case AvatarIKGoal.RightFoot:
                rf = pos;
                break;
            case AvatarIKGoal.RightHand:
                rh = pos;
                break;
            case AvatarIKGoal.LeftHand:
                lh = pos;
                break;
        }
    }

    public void UpdateIKWeight(AvatarIKGoal goal, float w)
    {
        switch (goal)
        {
            case AvatarIKGoal.LeftFoot:
                w_lf = w;
                break;
            case AvatarIKGoal.RightFoot:
                w_rf = w;
                break;
            case AvatarIKGoal.RightHand:
                w_rh = w;
                break;
            case AvatarIKGoal.LeftHand:
                w_lh = w;
                break;
        }
    }

    private Vector3 LocalToWorld(Vector3 p) {
        Vector3 r = helper.position;
        r += helper.right * p.x;
        r += helper.forward * p.z;
        r += helper.up * p.y;
        return r;
    }

    private void OnAnimatorIK() {
        SetIKPos(AvatarIKGoal.LeftHand, lh, w_lh);
        SetIKPos(AvatarIKGoal.RightHand, rh, w_rh);
        SetIKPos(AvatarIKGoal.LeftFoot, lf, w_lf);
        SetIKPos(AvatarIKGoal.RightFoot, rf, w_rf);
    }

    private void SetIKPos(AvatarIKGoal goal, Vector3 pos, float w) {
        anim.SetIKPositionWeight(goal, w);
        anim.SetIKPosition(goal, pos);
    }
}
