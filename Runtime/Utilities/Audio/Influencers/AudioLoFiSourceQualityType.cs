﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Utilities.Audio.Influencers
{
    /// <summary>
    /// Source quality options, used by the AudioLoFiEffect, that match common telephony and
    /// radio broadcast options.
    /// </summary>
    public enum AudioLoFiSourceQualityType
    {
        /// <summary>
        /// Narrow frequency range telephony.
        /// </summary>
        NarrowBandTelephony = 0,

        /// <summary>
        /// Wide frequency range telephony.
        /// </summary>
        WideBandTelephony,

        /// <summary>
        /// AM radio.
        /// </summary>
        AmRadio,

        /// <summary>
        /// FM radio.
        /// </summary>
        /// <remarks>
        /// The FM radio frequency is quite wide as it relates to human hearing. While it is
        /// a lower fidelity than FullRange, some users may not hear a difference.
        /// </remarks>
        FmRadio,

        /// <summary>
        /// Full range of human hearing.
        /// </summary>
        /// <remarks>
        /// The frequency range used is a bit wider than that of human
        /// hearing. It closely resembles the range used for audio CDs.
        /// </remarks>
        FullRange
    }
}
