﻿using System.Collections.Generic;
using System.Linq;
using AVFoundation;
using Foundation;
using MediaManager.Library;
using MediaManager.Media;

namespace MediaManager.Platforms.Apple.Media
{
    public static class MediaItemExtensions
    {
        public static Dictionary<string, string> RequestHeaders => CrossMediaManager.Current.RequestHeaders;

        public static NSUrl GetNSUrl(this IMediaItem mediaItem)
        {
            var isLocallyAvailable = mediaItem.MediaLocation.IsLocal();
            return isLocallyAvailable ? new NSUrl(mediaItem.MediaUri, false) : new NSUrl(mediaItem.MediaUri);
        }

        public static AVPlayerItem ToAVPlayerItem(this IMediaItem mediaItem)
        {
            AVAsset asset;

            if (mediaItem.MediaLocation.IsLocal())
            {
                if (mediaItem.MediaUri.StartsWith("file:///"))
                {
                    asset = AVAsset.FromUrl(NSUrl.FromString(mediaItem.MediaUri));
                }
                else
                {
                    asset = AVAsset.FromUrl(NSUrl.FromFilename(mediaItem.MediaUri));
                }
            }
            else if (RequestHeaders != null && RequestHeaders.Any())
            {
                asset = AVUrlAsset.Create(NSUrl.FromString(mediaItem.MediaUri), GetOptionsWithHeaders(RequestHeaders));
            }
            else
            {
                asset = AVAsset.FromUrl(NSUrl.FromString(mediaItem.MediaUri));
            }
            return AVPlayerItem.FromAsset(asset);
        }

        private static AVUrlAssetOptions GetOptionsWithHeaders(IDictionary<string, string> headers)
        {
            var nativeHeaders = new NSMutableDictionary();

            foreach (var header in headers)
            {
                nativeHeaders.Add((NSString)header.Key, (NSString)header.Value);
            }

            var nativeHeadersKey = (NSString)"AVURLAssetHTTPHeaderFieldsKey";

            var options = new AVUrlAssetOptions(NSDictionary.FromObjectAndKey(
                nativeHeaders,
                nativeHeadersKey
            ));

            return options;
        }
    }
}
