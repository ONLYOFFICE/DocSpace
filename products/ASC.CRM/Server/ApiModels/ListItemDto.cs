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


using ASC.Common;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;
using System;
using System.Runtime.Serialization;

namespace ASC.CRM.ApiModels
{

    #region History Category

    [DataContract(Name = "historyCategoryBase", Namespace = "")]
    public class HistoryCategoryBaseDto : ListItemDto
    {
        public HistoryCategoryBaseDto()
        {

        }

        public HistoryCategoryBaseDto(ListItem listItem)
            : base(listItem)
        {
        }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember]
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

    [DataContract(Name = "historyCategory", Namespace = "")]
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

    [Scope]
    public sealed class HistoryCategoryDtoHelper
    {
        public HistoryCategoryDtoHelper(WebImageSupplier webImageSupplier)
        {
            WebImageSupplier = webImageSupplier;
        }

        public WebImageSupplier WebImageSupplier { get; }

        public HistoryCategoryBaseDto Get(ListItem listItem)
        {
            return new HistoryCategoryBaseDto(listItem)
            {
                ImagePath = WebImageSupplier.GetAbsoluteWebPath(listItem.AdditionalParams, ProductEntryPoint.ID)
            };
        }
    }

    #endregion

    #region Deal Milestone

    [DataContract(Name = "opportunityStagesBase", Namespace = "")]
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

        [DataMember]
        public int SuccessProbability { get; set; }

        [DataMember]
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

    [DataContract(Name = "opportunityStages", Namespace = "")]
    public class DealMilestoneDto : DealMilestoneBaseDto
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
    }

    #endregion

    #region Task Category

    [DataContract(Name = "taskCategoryBase", Namespace = "")]
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

    [DataContract(Name = "taskCategory", Namespace = "")]
    public class TaskCategoryDto : TaskCategoryBaseDto
    {
        public TaskCategoryDto()
        {
        }

        public TaskCategoryDto(ListItem listItem): base(listItem)
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


    [Scope]
    public sealed class TaskCategoryDtoHelper
    {
        public TaskCategoryDtoHelper(WebImageSupplier webImageSupplier)
        {
            WebImageSupplier = webImageSupplier;                
        }

        public WebImageSupplier WebImageSupplier { get; }

        public TaskCategoryBaseDto Get(ListItem listItem)
        {
            return new TaskCategoryBaseDto(listItem)
            {
                ImagePath = WebImageSupplier.GetAbsoluteWebPath(listItem.AdditionalParams, ProductEntryPoint.ID)
            };
        }
    }







    #endregion

    #region Contact Status

    [DataContract(Name = "contactStatusBase", Namespace = "")]
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

    [DataContract(Name = "contactStatus", Namespace = "")]
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

    [DataContract(Name = "contactTypeBase", Namespace = "")]
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

    [DataContract(Name = "contactType", Namespace = "")]
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
                    Id= 30,
                    Title = "Client",
                    SortOrder = 2,
                    Description = "",
                    RelativeItemsCount = 1
                };
        }
    }

    #endregion

    #region Tags

    [DataContract(Name = "tagDto", Namespace = "")]
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

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
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

    [DataContract(Name = "listItem", Namespace = "")]
    public abstract class ListItemDto 
    {
        protected ListItemDto()
        {

        }

        protected ListItemDto(ListItem listItem)
        {
            Title = listItem.Title;
            Description = listItem.Description;
            Color = listItem.Color;
            SortOrder = listItem.SortOrder;
        }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        
        public String Title { get; set; }

        
        public String Description { get; set; }

        
        public String Color { get; set; }

        
        public int SortOrder { get; set; }
    }
}