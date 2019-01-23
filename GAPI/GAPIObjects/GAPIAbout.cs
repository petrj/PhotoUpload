using System;
using System.Collections.Generic;

namespace GAPI
{
    public partial class GAPIAbout : GAPIBaseObject
    {
        // https://developers.google.com/drive/api/v2/reference/about/get

        public string kind { get; set; }
        public string etag { get; set; }

        public string selfLink { get; set; }
        public string name { get; set; }

        public GAPIUser user { get; set; } = new GAPIUser();

        public long quotaBytesTotal { get; set; }
        public long quotaBytesUsed { get; set; }
        public long quotaBytesUsedAggregate { get; set; }
        public long quotaBytesUsedInTrash { get; set; }
        public string quotaType { get; set; }

        public List<GAPIQuotaBytesByService> quotaBytesByService { get; set; } = new List<GAPIQuotaBytesByService>();

        public long largestChangeId { get; set; }
        public long remainingChangeIds { get; set; }
        public string rootFolderId { get; set; }
        public string domainSharingPolicy { get; set; }
        public string permissionId { get; set; }

        public List<GAPIFormats> importFormats { get; set; } = new List<GAPIFormats>();
        public List<GAPIFormats> exportFormats { get; set; } = new List<GAPIFormats>();

        public List<GAPIAdditionalRoleInfo> additionalRoleInfo { get; set; } = new List<GAPIAdditionalRoleInfo>();

        public List<GAPIFeature> features { get; set; } = new List<GAPIFeature>();

        public List<GAPIMaxUploadSize> maxUploadSizes { get; set; } = new List<GAPIMaxUploadSize>();

        public bool isCurrentAppInstalled { get; set; }
        public string languageCode { get; set; }
        public List<string> folderColorPalette { get; set; } = new List<string>();

        public List<GAPITeamDriveThemes> teamDriveThemes { get; set; } = new List<GAPITeamDriveThemes>();

        public bool canCreateTeamDrives { get; set; }
    }
}
