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


using System;


using ASC.Common.Mapping;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Mapping;

using AutoMapper;

namespace ASC.CRM.ApiModels
{

    #region History Category
    public class HistoryCategoryBaseDto : ListItemDto
    {
        public HistoryCategoryBaseDto()
        {

        }

        public HistoryCategoryBaseDto(ListItem listItem)
            : base(listItem)
        {
        }

        public String ImagePath { get; set; }

        public static HistoryCategoryBaseDto GetSample()
        {
            return new HistoryCategoryBaseDto
            {
                Title = "Lunch",
                SortOrder = 10,
                Color = String.Empty,
                Description = "",
                ImagePath = "path to image"
            };
        }
    }

    public class HistoryCategoryDto : HistoryCategoryBaseDto
    {
        public HistoryCategoryDto()
        {
        }

        public HistoryCategoryDto(ListItem listItem)
            : base(listItem)
        {
        }


        public int RelativeItemsCount { get; set; }

        public new static HistoryCategoryDto GetSample()
        {
            return new HistoryCategoryDto
            {
                Title = "Lunch",
                SortOrder = 10,
                Color = String.Empty,
                Description = "",
                ImagePath = "path to image",
                RelativeItemsCount = 1
            };
        }
    }

    #endregion

    #region Deal Milestone

    public class DealMilestoneBaseDto : ListItemDto
    {
        public DealMilestoneBaseDto()
        {
        }

        public DealMilestoneBaseDto(DealMilestone dealMilestone)
        {
            SuccessProbability = dealMilestone.Probability;
            StageType = dealMilestone.Status;
            Color = dealMilestone.Color;
            Description = dealMilestone.Description;
            Title = dealMilestone.Title;
        }

        public int SuccessProbability { get; set; }
        public DealMilestoneStatus StageType { get; set; }
        public static DealMilestoneBaseDto GetSample()
        {
            return new DealMilestoneBaseDto
            {
                Title = "Discussion",
                SortOrder = 2,
                Color = "#B9AFD3",
                Description = "The potential buyer showed his/her interest and sees how your offering meets his/her goal",
                StageType = DealMilestoneStatus.Open,
                SuccessProbability = 20
            };
        }
    }

    public class DealMilestoneDto : DealMilestoneBaseDto, IMapFrom<DealMilestone>
    {
        public DealMilestoneDto()
        {
        }

        public DealMilestoneDto(DealMilestone dealMilestone)
            : base(dealMilestone)
        {
        }
        public int RelativeItemsCount { get; set; }

        public new static DealMilestoneDto GetSample()
        {
            return new DealMilestoneDto
            {
                Title = "Discussion",
                SortOrder = 2,
                Color = "#B9AFD3",
                Description = "The potential buyer showed his/her interest and sees how your offering meets his/her goal",
                StageType = DealMilestoneStatus.Open,
                SuccessProbability = 20,
                RelativeItemsCount = 1
            };
        }

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<DealMilestone, DealMilestoneDto>().ConvertUsing<DealMilestoneDtoTypeConverter>();
        }
    }

    #endregion

    #region Task Category

    public class TaskCategoryBaseDto : ListItemDto
    {
        public TaskCategoryBaseDto()
        {
        }

        public TaskCategoryBaseDto(ListItem listItem) : base(listItem)
        {

        }

        public String ImagePath { get; set; }

        public static TaskCategoryBaseDto GetSample()
        {
            return new TaskCategoryBaseDto
            {
                Title = "Appointment",
                SortOrder = 2,
                Description = "",
                ImagePath = "path to image"
            };
        }



    }

    public class TaskCategoryDto : TaskCategoryBaseDto
    {
        public TaskCategoryDto()
        {
        }

        public TaskCategoryDto(ListItem listItem) : base(listItem)
        {
        }



        public int RelativeItemsCount { get; set; }

        public new static TaskCategoryDto GetSample()
        {
            return new TaskCategoryDto
            {
                Id = 30,
                Title = "Appointment",
                SortOrder = 2,
                Description = "",
                ImagePath = "path to image",
                RelativeItemsCount = 1
            };
        }
    }







    #endregion

    #region Contact Status

    public class ContactStatusBaseDto : ListItemDto
    {
        public ContactStatusBaseDto()
        {
        }

        public ContactStatusBaseDto(ListItem listItem)
            : base(listItem)
        {
        }

        public static ContactStatusBaseDto GetSample()
        {
            return new ContactStatusBaseDto
            {
                Title = "Cold",
                SortOrder = 2,
                Description = ""
            };
        }
    }

    public class ContactStatusDto : ContactStatusBaseDto
    {
        public ContactStatusDto()
        {
        }

        public ContactStatusDto(ListItem listItem)
            : base(listItem)
        {
        }


        public int RelativeItemsCount { get; set; }

        public new static ContactStatusDto GetSample()
        {
            return new ContactStatusDto
            {
                Title = "Cold",
                SortOrder = 2,
                Description = "",
                RelativeItemsCount = 1
            };
        }
    }

    #endregion

    #region Contact Type

    public class ContactTypeBaseDto : ListItemDto
    {
        public ContactTypeBaseDto()
        {

        }

        public ContactTypeBaseDto(ListItem listItem)
            : base(listItem)
        {
        }

        public static ContactTypeBaseDto GetSample()
        {
            return new ContactTypeBaseDto
            {
                Id = 30,
                Title = "Client",
                SortOrder = 2,
                Description = ""
            };
        }
    }

    public class ContactTypeDto : ContactTypeBaseDto
    {
        public ContactTypeDto()
        {
        }

        public ContactTypeDto(ListItem listItem)
            : base(listItem)
        {
        }


        public int RelativeItemsCount { get; set; }

        public new static ContactTypeDto GetSample()
        {
            return new ContactTypeDto
            {
                Id = 30,
                Title = "Client",
                SortOrder = 2,
                Description = "",
                RelativeItemsCount = 1
            };
        }
    }

    #endregion

    #region Tags

    public class TagDto
    {
        public TagDto()
        {
            Title = String.Empty;
            RelativeItemsCount = 0;
        }

        public TagDto(String tag, int relativeItemsCount = 0)
        {
            Title = tag;
            RelativeItemsCount = relativeItemsCount;
        }

        public String Title { get; set; }
        public int RelativeItemsCount { get; set; }
        public static TagDto GetSample()
        {
            return new TagDto
            {
                Title = "Tag",
                RelativeItemsCount = 1
            };
        }
    }

    #endregion

    public class ListItemDto : IMapFrom<ListItem>
    {
        public ListItemDto()
        {

        }

        protected ListItemDto(ListItem listItem)
        {
            Id = listItem.ID;
            Title = listItem.Title;
            Description = listItem.Description;
            Color = listItem.Color;
            SortOrder = listItem.SortOrder;
        }

        public int Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String Color { get; set; }
        public int SortOrder { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<ListItem, TaskCategoryBaseDto>().ConvertUsing<ListItemDtoTypeConverter>();
            profile.CreateMap<ListItem, TaskCategoryDto>().ConvertUsing<ListItemDtoTypeConverter>();
            profile.CreateMap<ListItem, HistoryCategoryDto>().ConvertUsing<ListItemDtoTypeConverter>();
            profile.CreateMap<ListItem, ContactStatusDto>().ConvertUsing<ListItemDtoTypeConverter>();
            profile.CreateMap<ListItem, ContactTypeDto>().ConvertUsing<ListItemDtoTypeConverter>();
        }
    }
}