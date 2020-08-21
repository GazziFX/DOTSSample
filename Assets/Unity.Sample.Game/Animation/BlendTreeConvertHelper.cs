using Unity.Animation;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Animation.Hybrid;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

// We currently do not support blobassets referencing other blobassets. This helper class can be used to store blendspace data on entity
// and create blendspace blobasset at entity initialization
public class BlendTreeEntityStoreHelper
{
    public struct BlendTree1DData : IComponentData
    {
        public StringHash BlendParameter;
    }

    public struct BlendTree2DData : IComponentData
    {
        public StringHash BlendParam;
        public StringHash BlendParamY;
    }




#if UNITY_EDITOR
    public static void AddBlendTree1DComponents(EntityManager entityManager, Entity entity, BlendTree blendTree)
    {
        entityManager.AddComponentData(entity, new BlendTree1DData
        {
            BlendParameter = new StringHash(blendTree.blendParameter)
        });

        var buffer = entityManager.AddBuffer<BlendTree1DMotionData>(entity);
        for(int i=0;i<blendTree.children.Length;i++)
        {
            buffer.Add(new BlendTree1DMotionData
            {
                MotionThreshold = blendTree.children[i].threshold,
                MotionSpeed = blendTree.children[i].timeScale,
                Motion = BlendTreeConvertHelper.Convert(blendTree.children[i].motion),
            });
        }
    }

    public static void AddBlendTree2DComponents(EntityManager entityManager, Entity entity, BlendTree blendTree)
    {
        var blendSpaceData = new BlendTree2DData
        {
            BlendParam = blendTree.blendParameter,
            BlendParamY = blendTree.blendParameterY,
        };
        entityManager.AddComponentData(entity, blendSpaceData);


        var blendSpaceEntries =  entityManager.AddBuffer<BlendTree2DMotionData>(entity);
        for (int i = 0; i < blendTree.children.Length; i++)
        {
            blendSpaceEntries.Add(new BlendTree2DMotionData
            {
                MotionPosition = blendTree.children[i].position,
                MotionSpeed = blendTree.children[i].timeScale,
                Motion = BlendTreeConvertHelper.Convert(blendTree.children[i].motion),
            });
        }
    }
#endif

    public static BlobAssetReference<BlendTree1D> CreateBlendTree1DFromComponents(EntityManager entityManager, Entity entity)
    {

        BlendTree1DMotionData[] blendTreeMotionData = entityManager.GetBuffer<BlendTree1DMotionData>(entity).ToNativeArray(Unity.Collections.Allocator.Temp).ToArray();
        return BlendTreeBuilder.CreateBlendTree(blendTreeMotionData);
    }

    public static BlobAssetReference<BlendTree2DSimpleDirectional> CreateBlendTree2DFromComponents(EntityManager entityManager,
        Entity entity)
    {
        BlendTree2DMotionData[] blendTree2DMotionData = entityManager.GetBuffer<BlendTree2DMotionData>(entity).ToNativeArray(Unity.Collections.Allocator.Temp).ToArray();
        return BlendTreeBuilder.CreateBlendTree2DSimpleDirectional(blendTree2DMotionData);
    }
}


#if UNITY_EDITOR
namespace UnityEditor.Animations
{
    public class BlendTreeConvertHelper
    {
        public static Unity.Animation.Motion Convert(UnityEngine.Motion motion)
        {
            var animationClip = motion as AnimationClip;
//            var blendTree = motion as BlendTree;
//
//            if (blendTree != null)
//                return Convert(blendTree);
//            else if( animationClip != null)
            if( animationClip != null)
            {
                var clip = animationClip.ToDenseClip();
                return new Unity.Animation.Motion { Clip = clip };
            }
            else
                throw new System.ArgumentException($"Selected Motion type is not supported.");
        }
    }
}
#endif
