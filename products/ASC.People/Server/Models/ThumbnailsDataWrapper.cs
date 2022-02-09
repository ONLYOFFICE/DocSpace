/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace ASC.Web.Api.Models
{
    public class ThumbnailsDataWrapper
    {
        public ThumbnailsDataWrapper(Guid userId, UserPhotoManager userPhotoManager)
        {
            Original = userPhotoManager.GetPhotoAbsoluteWebPath(userId);
            Retina = userPhotoManager.GetRetinaPhotoURL(userId);
            Max = userPhotoManager.GetMaxPhotoURL(userId);
            Big = userPhotoManager.GetBigPhotoURL(userId);
            Medium = userPhotoManager.GetMediumPhotoURL(userId);
            Small = userPhotoManager.GetSmallPhotoURL(userId);
        }

        private ThumbnailsDataWrapper()
        {
        }

        public string Original { get; set; }

        public string Retina { get; set; }

        public string Max { get; set; }

        public string Big { get; set; }

        public string Medium { get; set; }

        public string Small { get; set; }


        public static ThumbnailsDataWrapper GetSample()
        {
            return new ThumbnailsDataWrapper
            {
                Original = "default_user_photo_size_1280-1280.png",
                Retina = "default_user_photo_size_360-360.png",
                Max = "default_user_photo_size_200-200.png",
                Big = "default_user_photo_size_82-82.png",
                Medium = "default_user_photo_size_48-48.png",
                Small = "default_user_photo_size_32-32.png",
            };
        }
    }
}