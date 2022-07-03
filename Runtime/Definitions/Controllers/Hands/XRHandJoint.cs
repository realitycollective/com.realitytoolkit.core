// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Definitions.Controllers.Hands
{
    /// <summary>
    /// The supported tracked hand joints for hand tracking as defined by the OpenXR standard.
    /// </summary>
    /// <remarks>See https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XrHandJointEXT for more information.</remarks>
    public enum XRHandJoint
    {
        Palm = 0,
        Wrist,
        ThumbMetacarpal,
        ThumbProximal,
        ThumbDistal,
        ThumbTip,
        IndexMetacarpal,
        IndexProximal,
        IndexIntermediate,
        IndexDistal,
        IndexTip,
        MiddleMetacarpal,
        MiddleProximal,
        MiddleIntermediate,
        MiddleDistal,
        MiddleTip,
        RingMetacarpal,
        RingProximal,
        RingIntermediate,
        RingDistal,
        RingTip,
        LittleMetacarpal,
        LittleProximal,
        LittleIntermediate,
        LittleDistal,
        LittleTip,
        Unknown = int.MaxValue
    }
}