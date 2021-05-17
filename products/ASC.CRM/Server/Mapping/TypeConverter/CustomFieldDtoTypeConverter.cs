using System;

using ASC.Common;
using ASC.CRM.ApiModels;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;

using AutoMapper;

namespace ASC.CRM.Mapping
{
    [Scope]
    public class CustomFieldDtoTypeConverter : ITypeConverter<CustomField, CustomFieldDto>
    {
        private readonly DaoFactory _daoFactory;

        public CustomFieldDtoTypeConverter(DaoFactory daoFactory)
        {
            _daoFactory = daoFactory;

        }

        public CustomFieldDto Convert(CustomField source, CustomFieldDto destination, ResolutionContext context)
        {
            if (destination != null)
                throw new NotImplementedException();

            var result = new CustomFieldDto
            {
                Id = source.ID,
                EntityId = source.EntityID,
                FieldType = source.Type,
                FieldValue = source.Value,
                Label = source.Label,
                Mask = source.Mask,
                Position = source.SortOrder,
                RelativeItemsCount = _daoFactory.GetCustomFieldDao().GetContactLinkCount(source.EntityType, source.ID)
            };

            return result;
        }
    }
}
