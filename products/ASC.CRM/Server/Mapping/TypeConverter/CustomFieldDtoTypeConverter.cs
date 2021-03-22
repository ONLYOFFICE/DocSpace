using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.CRM.ApiModels;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;

using AutoMapper;

namespace ASC.CRM.Mapping
{
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

            var result = new CustomFieldDto(source)
            {
                RelativeItemsCount = _daoFactory.GetCustomFieldDao().GetContactLinkCount(source.EntityType, source.ID)
            };

            return result;
        }
    }
}
