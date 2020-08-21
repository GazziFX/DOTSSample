#if UNITY_EDITOR

using Unity.Animation;
using Unity.Entities;
using UnityEngine;
using UnityEditor.Animations;
using Unity.Animation.Hybrid;

public class AnimSourceDamageAuthoring : AnimSourceAuthoring, IConvertGameObjectToEntity
{
    [Range(0f, 1f)]
    public float Blend;
    public BlendTree BlendTreeAsset;
    public AnimationClip AdditiveRefPose;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AnimSource.Data());

        var settings = new AnimSourceDamage.Settings
        {
            Blend = Blend,
            AdditiveRefPose = AdditiveRefPose.ToDenseClip(),
        };

        BlendTreeEntityStoreHelper.AddBlendTree1DComponents(dstManager,entity, BlendTreeAsset);

        dstManager.AddComponentData(entity, settings);
    }
}

#endif
