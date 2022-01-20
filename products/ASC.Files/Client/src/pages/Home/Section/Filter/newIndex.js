import React from 'react';
import find from 'lodash/find';
import result from 'lodash/result';
import { withTranslation } from 'react-i18next';
import { withRouter } from 'react-router';
import { FilterType } from '@appserver/common/constants';
import Loaders from '@appserver/common/components/Loaders';
import FilterInput from '@appserver/common/components/NewFilterInput';
import { withLayoutSize } from '@appserver/common/utils';
import { isMobileOnly, isMobile } from 'react-device-detect';
import { inject, observer } from 'mobx-react';
import withLoader from '../../../../HOCs/withLoader';

const getFilterType = (filterValues) => {
  const filterType = result(
    find(filterValues, (value) => {
      return value.group === 'filter-filterType';
    }),
    'key',
  );

  return filterType ? +filterType : null;
};

const getAuthorType = (filterValues) => {
  const authorType = result(
    find(filterValues, (value) => {
      return value.group === 'filter-author';
    }),
    'key',
  );

  return authorType ? authorType : null;
};

const getSelectedItem = (filterValues, type) => {
  const selectedItem = filterValues.find((item) => item.key === type);
  return selectedItem || null;
};

const getSearchParams = (filterValues) => {
  const searchParams = result(
    find(filterValues, (value) => {
      return value.group === 'filter-folders';
    }),
    'key',
  );

  return searchParams || 'true';
};

const SectionFilterContent = ({
  t,
  customNames,
  user,
  filter,
  personal,
  isRecentFolder,
  isFavoritesFolder,
  sectionWidth,
  isFiltered,
  viewAs,
  createThumbnails,
  setViewAs,
  setIsLoading,
  selectedFolderId,
  fetchFiles,
}) => {
  const filterColumnCount = window.innerWidth < 500 ? {} : { filterColumnCount: personal ? 2 : 3 };

  const onFilter = (data) => {
    const filterType = getFilterType(data) || null;
    // const authorType = getAuthorType(data.filterValues);
    const withSubfolders = getSearchParams(data);

    // const selectedItem = authorType ? getSelectedItem(data.filterValues, authorType) : null;
    // const selectedFilterItem = {};
    // if (selectedItem) {
    //   selectedFilterItem.key = selectedItem.selectedItem.key;
    //   selectedFilterItem.label = selectedItem.selectedItem.label;
    //   selectedFilterItem.type = selectedItem.typeSelector;
    // }

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.filterType = filterType;
    // newFilter.authorType = authorType;
    newFilter.withSubfolders = withSubfolders;
    // newFilter.selectedItem = selectedFilterItem;

    setIsLoading(true);

    fetchFiles(selectedFolderId, newFilter).finally(() => setIsLoading(false));
  };

  const onSearch = (data) => {};

  const onSort = (data) => {};

  const onChangeViewAs = (view) => {
    if (view === 'row') {
      setViewAs('table');
    } else {
      setViewAs(view);
    }
  };

  const getFilterData = () => {
    const { selectedItem } = filter;
    const { usersCaption, groupsCaption } = customNames;

    const folders =
      !isFavoritesFolder && !isRecentFolder
        ? [
            {
              key: FilterType.FoldersOnly.toString(),
              group: 'filter-filterType',
              label: t('Translations:Folders'),
            },
          ]
        : '';

    const allFiles =
      !isFavoritesFolder && !isRecentFolder
        ? [
            {
              key: FilterType.FilesOnly.toString(),
              group: 'filter-filterType',
              label: t('AllFiles'),
            },
          ]
        : '';

    const images = !isRecentFolder
      ? [
          {
            key: FilterType.ImagesOnly.toString(),
            group: 'filter-filterType',
            label: t('Images'),
          },
        ]
      : '';

    const archives = !isRecentFolder
      ? [
          {
            key: FilterType.ArchiveOnly.toString(),
            group: 'filter-filterType',
            label: t('Archives'),
          },
        ]
      : '';

    const media = !isRecentFolder
      ? [
          {
            key: FilterType.MediaOnly.toString(),
            group: 'filter-filterType',
            label: t('Media'),
          },
        ]
      : '';

    const typeOptions = [
      {
        key: 'filter-filterType',
        group: 'filter-filterType',
        label: t('Common:Type'),
        isHeader: true,
      },
      {
        key: FilterType.DocumentsOnly.toString(),
        group: 'filter-filterType',
        label: t('Common:Documents'),
      },
      ...folders,
      {
        key: FilterType.SpreadsheetsOnly.toString(),
        group: 'filter-filterType',
        label: t('Translations:Spreadsheets'),
      },
      ...archives,

      {
        key: FilterType.PresentationsOnly.toString(),
        group: 'filter-filterType',
        label: t('Translations:Presentations'),
      },
      ...images,
      ...media,
      ...allFiles,
    ];

    const filterOptions = [];

    if (!personal) {
      filterOptions.push(
        {
          key: 'filter-author',
          group: 'filter-author',
          label: t('ByAuthor'),
          isHeader: true,
        },
        {
          key: 'user',
          group: 'filter-author',
          label: usersCaption,
          isSelector: true,
          defaultOptionLabel: t('Common:MeLabel'),
          defaultSelectLabel: t('Common:Select'),
          groupsCaption,
          defaultOption: user,
          selectedItem,
        },
      );
    }

    filterOptions.push(...typeOptions);

    if (!isRecentFolder && !isFavoritesFolder)
      filterOptions.push(
        {
          key: 'filter-folders',
          group: 'filter-folders',
          label: t('Translations:Folders'),
          isHeader: true,
          withoutHeader: true,
          isLast: true,
        },
        {
          key: 'false',
          group: 'filter-folders',
          label: t('NoSubfolders'),
          isToggle: true,
        },
      );

    return filterOptions;
  };

  const getSelectedFilterData = () => {
    const selectedFilterData = {
      filterValues: [],
      sortDirection: filter.sortOrder === 'ascending' ? 'asc' : 'desc',
      sortId: filter.sortBy,
    };

    selectedFilterData.inputValue = filter.search;

    if (filter.filterType) {
      selectedFilterData.filterValues.push({
        key: `${filter.filterType}`,
        group: 'filter-filterType',
      });
    }

    if (filter.authorType) {
      selectedFilterData.filterValues.push({
        key: `${filter.authorType}`,
        group: 'filter-author',
      });
    }

    if (filter.withSubfolders === 'false') {
      selectedFilterData.filterValues.push({
        key: filter.withSubfolders,
        group: 'filter-folders',
      });
    }

    return selectedFilterData;
  };

  const getViewSettingsData = () => {
    const viewSettings = [
      {
        value: 'row',
        label: t('ViewList'),
        icon: '/static/images/view-rows.react.svg',
      },
      {
        value: 'tile',
        label: t('ViewTiles'),
        icon: '/static/images/view-tiles.react.svg',
        callback: createThumbnails,
      },
    ];

    return viewSettings;
  };

  const getSortData = () => {
    const commonOptions = [
      { key: 'DateAndTime', label: t('ByLastModifiedDate'), default: true },
      { key: 'DateAndTimeCreation', label: t('ByCreationDate'), default: true },
      { key: 'AZ', label: t('ByTitle'), default: true },
      { key: 'Type', label: t('Common:Type'), default: true },
      { key: 'Size', label: t('Common:Size'), default: true },
    ];

    if (!personal)
      commonOptions.push({
        key: 'Author',
        label: t('ByAuthor'),
        default: true,
      });

    return commonOptions;
  };

  return (
    <FilterInput
      sectionWidth={sectionWidth}
      getFilterData={getFilterData}
      getSortData={getSortData}
      getViewSettingsData={getViewSettingsData}
      selectedFilterData={getSelectedFilterData()}
      onFilter={onFilter}
      onChangeViewAs={onChangeViewAs}
      viewAs={viewAs}
      directionAscLabel={t('Common:DirectionAscLabel')}
      directionDescLabel={t('Common:DirectionDescLabel')}
      placeholder={t('Common:Search')}
      // isReady={isReady}
      {...filterColumnCount}
      contextMenuHeader={t('Common:AddFilter')}
    />
  );
};

export default inject(({ auth, filesStore, treeFoldersStore, selectedFolderStore }) => {
  const {
    fetchFiles,
    filter,
    setIsLoading,
    setViewAs,
    viewAs,
    files,
    folders,
    createThumbnails,
  } = filesStore;

  const { user } = auth.userStore;
  const { customNames, culture, personal } = auth.settingsStore;
  const { isFavoritesFolder, isRecentFolder } = treeFoldersStore;

  const { search, filterType, authorType } = filter;
  const isFiltered =
    (!!files.length || !!folders.length || search || filterType || authorType) &&
    !(treeFoldersStore.isPrivacyFolder && isMobile);

  return {
    customNames,
    user,
    selectedFolderId: selectedFolderStore.id,
    selectedItem: filter.selectedItem,
    filter,
    viewAs,
    isFiltered,
    isFavoritesFolder,
    isRecentFolder,

    setIsLoading,
    fetchFiles,
    setViewAs,
    createThumbnails,

    personal,
  };
})(
  withRouter(
    withLayoutSize(
      withTranslation(['Home', 'Common', 'Translations'])(
        withLoader(observer(SectionFilterContent))(<Loaders.Filter />),
      ),
    ),
  ),
);
