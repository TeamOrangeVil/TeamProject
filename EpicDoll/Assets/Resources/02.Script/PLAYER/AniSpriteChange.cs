using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spine.Unity.Modules;

public class AniSpriteChange : MonoBehaviour
{

    private static AniSpriteChange gInstance = null;

    #region Inspector
    //스파인 애니에 변경하여 적용 될 스킨
    [Header("Skin For Swap")]
    [SpineSkin]
    public string Thema01;
    [SpineSkin]
    public string Thema02;
    #endregion

    //스파인 이미지 변경을 위해 선언
    SkeletonAnimation skeletonAnimation;//에니메이션 셋팅
    Spine.Skin mySkin;//스킨 정보
    public string currentName;
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
        Spine.SkeletonData data = skeletonAnimation.skeleton.Data;
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
        //2. 불러온 스킨데이터를 애니메이션에 적용시킨다.
        skeletonAnimation.skeleton.SetSkin(AttachmentSource);
        skeletonAnimation.skeleton.SetSlotsToSetupPose();
        Spine.TrackEntry currentTrack = skeletonAnimation.state.GetCurrent(0);
        if (currentTrack != null)
        {
            currentTrack.Animation.Apply(skeletonAnimation.skeleton, 0f, currentTrack.Time, currentTrack.Loop, null);
        }
    }
}