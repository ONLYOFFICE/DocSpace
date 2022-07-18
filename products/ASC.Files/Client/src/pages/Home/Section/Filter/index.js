import React from "react";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import find from "lodash/find";
import result from "lodash/result";

import { getUser } from "@appserver/common/api/people";
import { FilterType, RoomsType } from "@appserver/common/constants";
import Loaders from "@appserver/common/components/Loaders";
import FilterInput from "@appserver/common/components/FilterInput";
import { withLayoutSize } from "@appserver/common/utils";

import withLoader from "../../../../HOCs/withLoader";

const getFilterType = (filterValues) => {
  const filterType = result(
    find(filterValues, (value) => {
      return value.group === "filter-filterType";
    }),
    "key"
  );

  return filterType ? +filterType : null;
};

const getAuthorType = (filterValues) => {
  const authorType = result(
    find(filterValues, (value) => {
      return value.group === "filter-author";
    }),
    "key"
  );

  return authorType ? authorType : null;
};

const getSearchParams = (filterValues) => {
  const searchParams = result(
    find(filterValues, (value) => {
      return value.group === "filter-folders";
    }),
    "key"
  );

  return searchParams || "true";
};

const getTypes = (filterValues) => {
  const filterTypes = filterValues.find(
    (value) => value.group === "filter-types"
  )?.key;

  const types =
    typeof filterTypes === "number"
      ? [filterTypes]
      : filterTypes?.length > 0
      ? filterTypes.map((type) => +type)
      : null;

  return types;
};

const getOwner = (filterValues) => {
  const filterOwner = result(
    find(filterValues, (value) => {
      return value.group === "filter-owner";
    }),
    "key"
  );

  return filterOwner ? filterOwner : null;
};

const getTags = (filterValues) => {
  const filterTags = filterValues.find((value) => value.group === "filter-tags")
    ?.key;

  const tags = filterTags?.length > 0 ? filterTags : null;

  return tags;
};

const SectionFilterContent = ({
  t,
  filter,
  roomsFilter,
  personal,
  isRecentFolder,
  isFavoritesFolder,
  sectionWidth,
  viewAs,
  createThumbnails,
  setViewAs,
  setIsLoading,
  selectedFolderId,
  fetchFiles,
  fetchRooms,
  fetchTags,
  infoPanelVisible,
  isRooms,
  userId,
}) => {
  const onFilter = React.useCallback(
    (data) => {
      if (isRooms) {
        const types = getTypes(data) || null;

        const owner = getOwner(data) || null;

        const subjectId = owner === "me" || owner === "other" ? userId : owner;

        const tags = getTags(data) || null;

        setIsLoading(true);

        const newFilter = roomsFilter.clone();

        newFilter.page = 0;
        newFilter.types = types ? types : null;
        newFilter.subjectId = subjectId ? subjectId : null;
        newFilter.tags = tags ? tags : null;

        fetchRooms(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      } else {
        const filterType = getFilterType(data) || null;
        const authorType = !!getAuthorType(data)
          ? getAuthorType(data).includes("user_")
            ? getAuthorType(data)
            : `user_${getAuthorType(data)}`
          : null;
        const withSubfolders = getSearchParams(data);

        const newFilter = filter.clone();
        newFilter.page = 0;
        newFilter.filterType = filterType;
        newFilter.authorType = authorType;
        newFilter.withSubfolders = withSubfolders;

        setIsLoading(true);

        fetchFiles(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      }
    },
    [
      fetchFiles,
      fetchRooms,
      setIsLoading,
      roomsFilter,
      filter,
      selectedFolderId,
    ]
  );

  const onSearch = React.useCallback(
    (data = "") => {
      if (isRooms) {
        const newFilter = this.filter.clone();

        newFilter.page = 0;
        newFilter.filterValue = data;

        fetchRooms(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      } else {
        const newFilter = filter.clone();
        newFilter.page = 0;
        newFilter.search = data;

        setIsLoading(true);

        fetchFiles(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      }
    },
    [
      setIsLoading,
      fetchFiles,
      fetchRooms,
      selectedFolderId,
      filter,
      roomsFilter,
    ]
  );

  const onSort = React.useCallback(
    (sortId, sortDirection) => {
      const sortBy = sortId;
      const sortOrder = sortDirection === "desc" ? "descending" : "ascending";

      const newFilter = isRooms ? roomsFilter.clone() : filter.clone();
      newFilter.page = 0;
      newFilter.sortBy = sortBy;
      newFilter.sortOrder = sortOrder;

      setIsLoading(true);

      if (isRooms) {
        fetchRooms(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      } else {
        fetchFiles(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      }
    },
    [
      setIsLoading,
      fetchFiles,
      fetchRooms,
      selectedFolderId,
      filter,
      roomsFilter,
    ]
  );

  const onChangeViewAs = React.useCallback(
    (view) => {
      if (view === "row") {
        if (
          (sectionWidth < 1025 && !infoPanelVisible) ||
          (sectionWidth < 625 && infoPanelVisible) ||
          isMobile
        ) {
          setViewAs("row");
        } else {
          setViewAs("table");
        }
      } else {
        setViewAs(view);
      }
    },
    [sectionWidth, infoPanelVisible, setViewAs]
  );

  const getSelectedInputValue = React.useCallback(() => {
    return isRooms ? roomsFilter.filterValue : filter.search;
  }, [isRooms, roomsFilter.filterValue, filter.search]);

  const getSelectedSortData = React.useCallback(() => {
    const currentFilter = isRooms ? roomsFilter : filter;
    return {
      sortDirection: currentFilter.sortOrder === "ascending" ? "asc" : "desc",
      sortId: currentFilter.sortBy,
    };
  }, [
    isRooms,
    filter.sortOrder,
    filter.sortBy,
    roomsFilter.sortOrder,
    roomsFilter.sortBy,
  ]);

  const getSelectedFilterData = React.useCallback(async () => {
    const filterValues = [];

    if (isRooms) {
      if (roomsFilter.types) {
        const key =
          typeof roomsFilter.types === "object"
            ? roomsFilter.types[0]
            : roomsFilter.types; //Remove it if filter types will be multi select

        let label = "";

        switch (key) {
          case RoomsType.CustomRoom:
            label = "Custom room";
            break;
          case RoomsType.FillingFormsRoom:
            label = "Filling form";
            break;
          case RoomsType.EditingRoom:
            label = "Editing";
            break;
          case RoomsType.ReviewRoom:
            label = "Review";
            break;
          case RoomsType.ReadOnlyRoom:
            label = "View-only";
            break;
        }

        filterValues.push({
          key: key,
          label: label,
          group: "filter-types",
        });
      }

      // TODO: add logic to other key
      if (roomsFilter.subjectId) {
        const isMe = userId === roomsFilter.subjectId;
        let label = null;

        if (!isMe) {
          const user = await getUser(roomsFilter.subjectId);

          label = user.displayName;
        }

        filterValues.push({
          key: isMe ? "me" : roomsFilter.subjectId,
          group: "filter-owner",
          label: label,
        });
      }

      if (roomsFilter.tags) {
        filterValues.push({
          key: roomsFilter.tags,
          group: "filter-tags",
          isMultiSelect: true,
        });
      }
    } else {
      if (filter.filterType) {
        let label = "";

        switch (filter.filterType) {
          case FilterType.DocumentsOnly.toString():
            label = t("Common:Documents");
            break;
          case FilterType.FoldersOnly.toString():
            label = t("Translations:Folders");
            break;
          case FilterType.SpreadsheetsOnly.toString():
            label = t("Translations:Spreadsheets");
            break;
          case FilterType.ArchiveOnly.toString():
            label = t("Archives");
            break;
          case FilterType.PresentationsOnly.toString():
            label = t("Translations:Presentations");
            break;
          case FilterType.ImagesOnly.toString():
            label = t("Images");
            break;
          case FilterType.MediaOnly.toString():
            label = t("Media");
            break;
          case FilterType.FilesOnly.toString():
            label = t("AllFiles");
            break;
        }

        filterValues.push({
          key: `${filter.filterType}`,
          label: label,
          group: "filter-filterType",
        });
      }

      if (filter.authorType) {
        const user = await getUser(filter.authorType.replace("user_", ""));
        filterValues.push({
          key: `${filter.authorType}`,
          group: "filter-author",
          label: user.displayName,
        });
      }

      if (filter.withSubfolders === "false") {
        filterValues.push({
          key: filter.withSubfolders,
          label: "Exclude subfolders",
          group: "filter-folders",
        });
      }
    }

    return filterValues;
  }, [
    filter.withSubfolders,
    filter.authorType,
    filter.filterType,
    roomsFilter.types,
    roomsFilter.subjectId,
    roomsFilter.tags,
    roomsFilter.tags?.length,
    userId,
    isRooms,
  ]);

  const getFilterData = React.useCallback(async () => {
    const folders =
      !isFavoritesFolder && !isRecentFolder
        ? [
            {
              key: FilterType.FoldersOnly.toString(),
              group: "filter-filterType",
              label: t("Translations:Folders"),
            },
          ]
        : "";

    const allFiles =
      !isFavoritesFolder && !isRecentFolder
        ? [
            {
              key: FilterType.FilesOnly.toString(),
              group: "filter-filterType",
              label: t("AllFiles"),
            },
          ]
        : "";

    const images = !isRecentFolder
      ? [
          {
            key: FilterType.ImagesOnly.toString(),
            group: "filter-filterType",
            label: t("Images"),
          },
        ]
      : "";

    const archives = !isRecentFolder
      ? [
          {
            key: FilterType.ArchiveOnly.toString(),
            group: "filter-filterType",
            label: t("Archives"),
          },
        ]
      : "";

    const media = !isRecentFolder
      ? [
          {
            key: FilterType.MediaOnly.toString(),
            group: "filter-filterType",
            label: t("Media"),
          },
        ]
      : "";

    const typeOptions = isRooms
      ? [
          {
            key: "filter-filterType",
            group: "filter-types",
            label: t("Common:Type"),
            isHeader: true,
          },
          {
            key: RoomsType.CustomRoom,
            group: "filter-types",
            label: "Custom room",
          },
          {
            key: RoomsType.FillingFormsRoom,
            group: "filter-types",
            label: "Filling form",
          },
          {
            key: RoomsType.EditingRoom,
            group: "filter-types",
            label: "Editing",
          },
          {
            key: RoomsType.ReviewRoom,
            group: "filter-types",
            label: "Review",
          },
          {
            key: RoomsType.ReadOnlyRoom,
            group: "filter-types",
            label: "View-only",
          },
        ]
      : [
          {
            key: "filter-filterType",
            group: "filter-filterType",
            label: t("Common:Type"),
            isHeader: true,
          },
          {
            key: FilterType.DocumentsOnly.toString(),
            group: "filter-filterType",
            label: t("Common:Documents"),
          },
          ...folders,
          {
            key: FilterType.SpreadsheetsOnly.toString(),
            group: "filter-filterType",
            label: t("Translations:Spreadsheets"),
          },
          ...archives,
          {
            key: FilterType.PresentationsOnly.toString(),
            group: "filter-filterType",
            label: t("Translations:Presentations"),
          },
          ...images,
          ...media,
          ...allFiles,
        ];

    const ownerOptions = [
      {
        key: "filter-owner",
        group: "filter-owner",
        label: t("ByAuthor"),
        isHeader: true,
      },
      {
        key: "me",
        group: "filter-owner",
        label: "Me",
      },
      {
        key: "other",
        group: "filter-owner",
        label: "Other",
      },
      {
        key: "user",
        group: "filter-owner",
        label: t("Translations:AddAuthor"),
        isSelector: true,
      },
    ];

    const filterOptions = [];

    if (isRooms) {
      filterOptions.push(...ownerOptions);

      filterOptions.push(...typeOptions);

      const tags = await fetchTags();

      if (tags) {
        const tagsOptions = tags.map((tag) => ({
          key: tag,
          group: "filter-tags",
          label: tag,
          isMultiSelect: true,
        }));

        filterOptions.push({
          key: "filter-tags",
          group: "filter-tags",
          label: "Tags",
          isHeader: true,
          isLast: true,
        });

        filterOptions.push(...tagsOptions);
      }
    } else {
      if (!personal) {
        filterOptions.push(
          {
            key: "filter-author",
            group: "filter-author",
            label: t("ByAuthor"),
            isHeader: true,
          },
          {
            key: "user",
            group: "filter-author",
            label: t("Translations:AddAuthor"),
            isSelector: true,
          }
        );
      }

      filterOptions.push(...typeOptions);

      if (!isRecentFolder && !isFavoritesFolder) {
        filterOptions.push(
          {
            key: "filter-folders",
            group: "filter-folders",
            label: t("Translations:Folders"),
            isHeader: true,
            withoutHeader: true,
            isLast: true,
          },
          {
            key: "false",
            group: "filter-folders",
            label: t("NoSubfolders"),
            isToggle: true,
          }
        );
      }
    }

    return filterOptions;
  }, [isFavoritesFolder, isRecentFolder, isRooms, t, personal]);

  const getViewSettingsData = React.useCallback(() => {
    const viewSettings = [
      {
        value: "row",
        label: t("ViewList"),
        icon: "/static/images/view-rows.react.svg",
      },
      {
        value: "tile",
        label: t("ViewTiles"),
        icon: "/static/images/view-tiles.react.svg",
        callback: createThumbnails,
      },
    ];

    return viewSettings;
  }, [createThumbnails]);

  const getSortData = React.useCallback(() => {
    const commonOptions = isRooms
      ? [
          { key: "AZ", label: "Name", default: true },
          { key: "Type", label: t("Common:Type"), default: true },
          { key: "Tags", label: "Tags", default: true },
          { key: "Author", label: "Owner", default: true },
          { key: "DateAndTime", label: t("ByLastModifiedDate"), default: true },
        ]
      : [
          { key: "AZ", label: t("ByTitle"), default: true },
          { key: "Type", label: t("Common:Type"), default: true },
          { key: "Size", label: t("Common:Size"), default: true },
          {
            key: "DateAndTimeCreation",
            label: t("ByCreationDate"),
            default: true,
          },
          { key: "DateAndTime", label: t("ByLastModifiedDate"), default: true },
        ];

    if (!personal && !isRooms) {
      commonOptions.splice(1, 0, {
        key: "Author",
        label: t("ByAuthor"),
        default: true,
      });
    }
    return commonOptions;
  }, [personal, isRooms, t]);

  const removeSelectedItem = React.useCallback(
    ({ key, group }) => {
      if (isRooms) {
        setIsLoading(true);

        const newFilter = roomsFilter.clone();

        if (group === "filter-types") {
          const newTypes = newFilter.types;

          const idx = newTypes.findIndex((type) => type === key);

          newTypes.splice(idx, 1);

          newFilter.types = newTypes.length > 0 ? newTypes : null;
        }

        if (group === "filter-owner") {
          newFilter.subjectId = null;
        }

        if (group === "filter-tags") {
          const newTags = newFilter.tags;

          const idx = newTags.findIndex((tag) => tag === key);

          newTags.splice(idx, 1);

          newFilter.tags = newTags.length > 0 ? newTags : null;
        }

        newFilter.page = 0;
        // newFilter.types = types ? types : null;
        // newFilter.subjectId = subjectId ? subjectId : null;
        // newFilter.tags = tags ? tags : null;

        fetchRooms(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      } else {
        const newFilter = filter.clone();
        if (group === "filter-filterType") {
          newFilter.filterType = null;
        }
        if (group === "filter-author") {
          newFilter.authorType = null;
        }
        if (group === "filter-folders") {
          newFilter.withSubfolders = null;
        }

        newFilter.page = 0;

        setIsLoading(true);

        fetchFiles(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      }
    },
    [
      fetchFiles,
      fetchRooms,
      setIsLoading,
      roomsFilter,
      filter,
      selectedFolderId,
    ]
  );

  return (
    <FilterInput
      t={t}
      onFilter={onFilter}
      getFilterData={getFilterData}
      getSelectedFilterData={getSelectedFilterData}
      onSort={onSort}
      getSortData={getSortData}
      getSelectedSortData={getSelectedSortData}
      viewAs={viewAs}
      viewSelectorVisible={true}
      onChangeViewAs={onChangeViewAs}
      getViewSettingsData={getViewSettingsData}
      onSearch={onSearch}
      getSelectedInputValue={getSelectedInputValue}
      filterHeader={t("Filter")}
      placeholder={t("Common:Search")}
      view={t("Common:View")}
      headerLabel={t("Translations:AddAuthor")}
      isFavoritesFolder={isFavoritesFolder}
      isRecentFolder={isRecentFolder}
      removeSelectedItem={removeSelectedItem}
    />
  );
};

export default inject(
  ({ auth, filesStore, treeFoldersStore, selectedFolderStore, tagsStore }) => {
    const {
      fetchFiles,
      filter,
      fetchRooms,
      roomsFilter,
      setIsLoading,
      setViewAs,
      viewAs,
      createThumbnails,
    } = filesStore;

    const { fetchTags } = tagsStore;

    const { user } = auth.userStore;
    const { customNames, personal } = auth.settingsStore;
    const {
      isFavoritesFolder,
      isRecentFolder,
      isRoomsFolder,
      isArchiveFolder,
    } = treeFoldersStore;

    const isRooms = isRoomsFolder || isArchiveFolder;

    const { isVisible: infoPanelVisible } = auth.infoPanelStore;

    return {
      customNames,
      user,
      userId: user.id,
      selectedFolderId: selectedFolderStore.id,
      selectedItem: filter.selectedItem,
      filter,
      roomsFilter,
      viewAs,

      isFavoritesFolder,
      isRecentFolder,
      isRooms,

      setIsLoading,
      fetchFiles,
      fetchRooms,
      fetchTags,
      setViewAs,
      createThumbnails,

      personal,
      infoPanelVisible,
    };
  }
)(
  withRouter(
    withLayoutSize(
      withTranslation(["Home", "Common", "Translations"])(
        withLoader(observer(SectionFilterContent))(<Loaders.Filter />)
      )
    )
  )
);
