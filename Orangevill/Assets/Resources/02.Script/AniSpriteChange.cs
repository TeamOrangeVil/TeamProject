using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spine.Unity.Modules;

public class AniSpriteChange : MonoBehaviour
{

    private static AniSpriteChange gInstance = null;
    //private string curAnimation = "";//현재 실행중인 애니메이션
    Spine.TrackEntry trackEntry;
    public string currentName;

    #region Inspector
    [Header("Swap Slot")]
    //스파인 애니에 적용 될 이미지 슬롯
    [SpineSlot]
    public string spineSlot01;
    [SpineSlot]
    public string spineSlot02;
    [SpineSlot]
    public string spineSlot03;
    [SpineSlot]
    public string spineSlot04;
    [SpineSlot]
    public string spineSlot05;
    [SpineSlot]
    public string spineSlot06;
    [SpineSlot]
    public string spineSlot07;
    [SpineSlot]
    public string spineSlot08;
    [SpineSlot]
    public string spineSlot09;
    //스파인 애니에 변경하여 적용 될 스킨
    [Header("Skin For Swap")]
    [SpineSkin]
    public string Thema01;
    [SpineSkin]
    public string Thema02;
    [SpineSkin]
    public string Thema03;
    [SpineSkin]
    public string Thema04;
    #endregion

    //스파인 이미지 변경을 위해 선언
    //SkeletonRenderer skeletonRenderer;//렌더러정보
    SkeletonAnimation skeletonAnimation;//에니메이션 셋팅
    Spine.Skin mySkin;//스킨 정보
    Spine.Bone myBone;
    float aniTime;
    public int skinNum;
    // Use this for initialization

    public static AniSpriteChange Instance
    {
        get
        {
            if (gInstance == null) { }
            return gInstance;
        }
    }
    void Awake()
    {
        gInstance = this;
    }
    void Start()
    {
        currentName = Thema01;
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (mySkin == null) { this.mySkin = new Spine.Skin("Default"); }
    }
    public void SpriteChange(int themaNum)
    {
        //1. 어느 스킨의 데이터를 불러올것인지 찾는다.
        Spine.SkeletonData data = skeletonAnimation.skeleton.data;
        switch (themaNum)
        {
            case 0:
                currentName = Thema01;
                break;
            case 1:
                currentName = Thema02;
                break;
        }
        skinNum = themaNum;
        Spine.Skin AttachmentSource = data.FindSkin(currentName);

        //2. 불러온 스킨의 데이터를 슬롯에 저장한다.
        if (currentName == Thema02)
        {
            int temp = data.FindSlotIndex("uniform/skin_hat");//유니폼 입었을때 모자 처리
            mySkin.AddAttachment(temp, "uniform/skin_hat", AttachmentSource.GetAttachment(temp, "uniform/skin_hat"));
        }
        else
        {
            skeletonAnimation.skeleton.SetAttachment("uniform/skin_hat", null);
        }
        int SlotIndex = data.FindSlotIndex(spineSlot01);
        mySkin.AddAttachment(SlotIndex, "doll_left_hand", AttachmentSource.GetAttachment(SlotIndex, "doll_left_hand"));
        SlotIndex = data.FindSlotIndex(spineSlot02);
        mySkin.AddAttachment(SlotIndex, "doll_mouth", AttachmentSource.GetAttachment(SlotIndex, "doll_mouth"));
        SlotIndex = data.FindSlotIndex(spineSlot03);
        mySkin.AddAttachment(SlotIndex, "doll_eye", AttachmentSource.GetAttachment(SlotIndex, "doll_eye"));
        SlotIndex = data.FindSlotIndex(spineSlot04);
        mySkin.AddAttachment(SlotIndex, "doll_head", AttachmentSource.GetAttachment(SlotIndex, "doll_head"));
        SlotIndex = data.FindSlotIndex(spineSlot05);
        mySkin.AddAttachment(SlotIndex, "left_leg", AttachmentSource.GetAttachment(SlotIndex, "left_leg"));
        SlotIndex = data.FindSlotIndex(spineSlot06);
        mySkin.AddAttachment(SlotIndex, "right_leg", AttachmentSource.GetAttachment(SlotIndex, "right_leg"));
        SlotIndex = data.FindSlotIndex(spineSlot07);
        mySkin.AddAttachment(SlotIndex, "doll_body", AttachmentSource.GetAttachment(SlotIndex, "doll_body"));
        SlotIndex = data.FindSlotIndex(spineSlot08);
        mySkin.AddAttachment(SlotIndex, "doll_right_hand", AttachmentSource.GetAttachment(SlotIndex, "doll_right_hand"));
        SlotIndex = data.FindSlotIndex(spineSlot09);
        mySkin.AddAttachment(SlotIndex, "hip", AttachmentSource.GetAttachment(SlotIndex, "hip"));


        //3. 슬롯에 저장된 스킨데이터들을 애니메이션에 적용시킨다.
        skeletonAnimation.skeleton.SetSkin(mySkin);
        skeletonAnimation.skeleton.SetSlotsToSetupPose();
        Spine.TrackEntry currentTrack = skeletonAnimation.state.GetCurrent(0);
        if (currentTrack != null)
        {
            currentTrack.Animation.Apply(skeletonAnimation.skeleton, 0f, currentTrack.time, currentTrack.loop, null);
        }
    }
}