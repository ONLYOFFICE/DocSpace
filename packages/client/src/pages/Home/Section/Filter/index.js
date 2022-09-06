import React from "react";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import find from "lodash/find";
import result from "lodash/result";

import {
  FilterGroups,
  FilterKeys,
} from "@docspace/client/src/helpers/filesConstants";

import { getUser } from "@docspace/common/api/people";
import { FilterType, RoomsType } from "@docspace/common/constants";
import Loaders from "@docspace/common/components/Loaders";
import FilterInput from "@docspace/common/components/FilterInput";
import { withLayoutSize } from "@docspace/common/utils";
import { getDefaultRoomName } from "@docspace/client/src/helpers/filesUtils";

import withLoader from "../../../../HOCs/withLoader";

const getFilterType = (filterValues) => {
  const filterType = result(
    find(filterValues, (value) => {
      return value.group === FilterGroups.filterType;
    }),
    "key"
  );

  return filterType ? +filterType : null;
};

const getAuthorType = (filterValues) => {
  const authorType = result(
    find(filterValues, (value) => {
      return value.group === FilterGroups.filterAuthor;
    }),
    "key"
  );

  return authorType ? authorType : null;
};

const getSearchParams = (filterValues) => {
  const searchParams = result(
    find(filterValues, (value) => {
      return value.group === FilterGroups.filterFolders;
    }),
    "key"
  );

  return searchParams || "true";
};

const getType = (filterValues) => {
  const filterType = filterValues.find(
    (value) => value.group === FilterGroups.roomFilterType
  )?.key;

  const type = filterType;

  return type;
};

const getOwner = (filterValues) => {
  const filterOwner = result(
    find(filterValues, (value) => {
      return value.group === FilterGroups.roomFilterOwner;
    }),
    "key"
  );

  return filterOwner ? filterOwner : null;
};

//TODO: restore all comments if search with subfolders and in content will be available for rooms filter

// const getFilterFolders = (filterValues) => {
//   const filterFolders = result(
//     find(filterValues, (value) => {
//       return value.group === FilterGroups.roomFilterFolders;
//     }),
//     "key"
//   );

//   return filterFolders ? filterFolders : null;
// };

// const getFilterContent = (filterValues) => {
//   const filterContent = result(
//     find(filterValues, (value) => {
//       return value.group === FilterGroups.roomFilterContent;
//     }),
//     "key"
//   );

//   return filterContent ? filterContent : null;
// };

const getTags = (filterValues) => {
  const filterTags = filterValues.find(
    (value) => value.group === FilterGroups.roomFilterTags
  )?.key;

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
  setCurrentRoomsFilter,
}) => {
  const onFilter = React.useCallback(
    (data) => {
      if (isRooms) {
        const type = getType(data) || null;

        const owner = getOwner(data) || null;

        const subjectId =
          owner === FilterKeys.other
            ? null
            : owner === FilterKeys.me
            ? userId
            : owner;

        const withoutMe = owner === FilterKeys.other;

        const tags = getTags(data) || null;

        // const withSubfolders =
        //   getFilterFolders(data) === FilterKeys.withSubfolders;

        // const withContent = getFilterContent(data) === FilterKeys.withContent;

        setIsLoading(true);

        const newFilter = roomsFilter.clone();

        newFilter.page = 0;
        newFilter.type = type ? type : null;
        newFilter.subjectId = subjectId ? subjectId : null;
        if (tags) {
          if (tags.includes(t("NoTag"))) {
            newFilter.tags = null;
            newFilter.withoutTags = true;
          } else {
            newFilter.tags = tags;
            newFilter.withoutTags = false;
          }
        } else {
          newFilter.tags = null;
          newFilter.withoutTags = false;
        }

        newFilter.withoutMe = withoutMe;
        // newFilter.withSubfolders = withSubfolders;
        // newFilter.searchInContent = withContent;

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
        const newFilter = roomsFilter.clone();

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
      // if (!roomsFilter.withSubfolders) {
      //   filterValues.push({
      //     key: FilterKeys.excludeSubfolders,
      //     label: "Exclude subfolders",
      //     group: FilterGroups.roomFilterFolders,
      //   });
      // }

      // if (roomsFilter.searchInContent) {
      //   filterValues.push({
      //     key: FilterKeys.withContent,
      //     label: "File contents",
      //     group: FilterGroups.roomFilterContent,
      //   });
      // }

      if (roomsFilter.type) {
        const key = +roomsFilter.type;

        const label = getDefaultRoomName(key, t);

        filterValues.push({
          key: key,
          label: label,
          group: FilterGroups.roomFilterType,
        });
      }

      if (roomsFilter.subjectId) {
        const isMe = userId === roomsFilter.subjectId;
        let label = isMe ? t("Common:MeLabel") : null;

        if (!isMe) {
          const user = await getUser(roomsFilter.subjectId);

          label = user.displayName;
        }

        filterValues.push({
          key: isMe ? FilterKeys.me : roomsFilter.subjectId,
          group: FilterGroups.roomFilterOwner,
          label: label,
        });
      }

      if (roomsFilter.withoutMe) {
        filterValues.push({
          key: FilterKeys.other,
          group: FilterGroups.roomFilterOwner,
          label: t("Common:OtherLabel"),
        });
      }

      // if (roomsFilter.withoutTags) {
      //   filterValues.push({
      //     key: [t("NoTag")],
      //     group: FilterGroups.roomFilterTags,
      //     isMultiSelect: true,
      //   });
      // }

      if (roomsFilter?.tags?.length > 0) {
        filterValues.push({
          key: roomsFilter.tags,
          group: FilterGroups.roomFilterTags,
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
          group: FilterGroups.filterType,
        });
      }

      if (filter.authorType) {
        const user = await getUser(filter.authorType.replace("user_", ""));
        filterValues.push({
          key: `${filter.authorType}`,
          group: FilterGroups.filterAuthor,
          label: user.displayName,
        });
      }

      if (filter.withSubfolders === "false") {
        filterValues.push({
          key: filter.withSubfolders,
          label: "Exclude subfolders",
          group: FilterGroups.filterFolders,
        });
      }
    }

    return filterValues;
  }, [
    filter.withSubfolders,
    filter.authorType,
    filter.filterType,
    roomsFilter.type,
    roomsFilter.subjectId,
    roomsFilter.tags,
    roomsFilter.tags?.length,
    roomsFilter.withoutMe,
    roomsFilter.withoutTags,
    // roomsFilter.withSubfolders,
    // roomsFilter.searchInContent,
    userId,
    isRooms,
  ]);

  const getFilterData = React.useCallback(async () => {
    const folders =
      !isFavoritesFolder && !isRecentFolder
        ? [
            {
              key: FilterType.FoldersOnly.toString(),
              group: FilterGroups.filterType,
              label: t("Translations:Folders"),
            },
          ]
        : "";

    const allFiles =
      !isFavoritesFolder && !isRecentFolder
        ? [
            {
              key: FilterType.FilesOnly.toString(),
              group: FilterGroups.filterType,
              label: t("AllFiles"),
            },
          ]
        : "";

    const images = !isRecentFolder
      ? [
          {
            key: FilterType.ImagesOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Images"),
          },
        ]
      : "";

    const archives = !isRecentFolder
      ? [
          {
            key: FilterType.ArchiveOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Archives"),
          },
        ]
      : "";

    const media = !isRecentFolder
      ? [
          {
            key: FilterType.MediaOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Media"),
          },
        ]
      : "";

    const typeOptions = isRooms
      ? [
          {
            key: FilterGroups.filterType,
            group: FilterGroups.roomFilterType,
            label: t("Common:Type"),
            isHeader: true,
          },
          {
            key: RoomsType.CustomRoom,
            group: FilterGroups.roomFilterType,
            label: t("CustomRooms"),
          },
          {
            key: RoomsType.FillingFormsRoom,
            group: FilterGroups.roomFilterType,
            label: t("FillingFormRooms"),
          },
          {
            key: RoomsType.EditingRoom,
            group: FilterGroups.roomFilterType,
            label: t("CollaborationRooms"),
          },
          {
            key: RoomsType.ReviewRoom,
            group: FilterGroups.roomFilterType,
            label: t("ReviewRooms"),
          },
          {
            key: RoomsType.ReadOnlyRoom,
            group: FilterGroups.roomFilterType,
            label: t("ViewOnlyRooms"),
          },
        ]
      : [
          {
            key: FilterGroups.filterType,
            group: FilterGroups.filterType,
            label: t("Common:Type"),
            isHeader: true,
          },
          {
            key: FilterType.DocumentsOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Common:Documents"),
          },
          ...folders,
          {
            key: FilterType.SpreadsheetsOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Translations:Spreadsheets"),
          },
          ...archives,
          {
            key: FilterType.PresentationsOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Translations:Presentations"),
          },
          ...images,
          ...media,
          ...allFiles,
        ];

    const ownerOptions = [
      {
        key: FilterGroups.roomFilterOwner,
        group: FilterGroups.roomFilterOwner,
        label: t("Common:Owner"),
        isHeader: true,
      },
      {
        key: FilterKeys.me,
        group: FilterGroups.roomFilterOwner,
        label: t("Common:MeLabel"),
      },
      {
        key: FilterKeys.other,
        group: FilterGroups.roomFilterOwner,
        label: t("Common:OtherLabel"),
      },
      {
        key: FilterKeys.user,
        group: FilterGroups.roomFilterOwner,
        label: t("Translations:AddOwner"),
        isSelector: true,
      },
    ];

    // const foldersOptions = [
    //   {
    //     key: FilterGroups.roomFilterFolders,
    //     group: FilterGroups.roomFilterFolders,
    //     label: "Search",
    //     isHeader: true,
    //     withoutSeparator: true,
    //   },
    //   {
    //     key: "folders",
    //     group: FilterGroups.roomFilterFolders,
    //     label: "",
    //     withOptions: true,
    //     options: [
    //       { key: FilterKeys.withSubfolders, label: "With subfolders" },
    //       { key: FilterKeys.excludeSubfolders, label: "Exclude subfolders" },
    //     ],
    //   },
    // ];

    // const contentOptions = [
    //   {
    //     key: FilterGroups.roomFilterContent,
    //     group: FilterGroups.roomFilterContent,
    //     isHeader: true,
    //     withoutHeader: true,
    //   },
    //   {
    //     key: FilterKeys.withContent,
    //     group: FilterGroups.roomFilterContent,
    //     label: "Search by file contents",
    //     isCheckbox: true,
    //   },
    // ];

    const filterOptions = [];

    if (isRooms) {
      // filterOptions.push(...foldersOptions);
      // filterOptions.push(...contentOptions);

      filterOptions.push(...ownerOptions);

      filterOptions.push(...typeOptions);

      const tags = await fetchTags();

      if (tags) {
        const tagsOptions = tags.map((tag) => ({
          key: tag,
          group: FilterGroups.roomFilterTags,
          label: tag,
          isMultiSelect: true,
        }));

        // tagsOptions.push({
        //   key: t("NoTag"),
        //   group: FilterGroups.roomFilterTags,
        //   label: t("NoTag"),
        //   isMultiSelect: true,
        // });

        filterOptions.push({
          key: FilterGroups.roomFilterTags,
          group: FilterGroups.roomFilterTags,
          label: t("Tags"),
          isHeader: true,
          isLast: true,
        });

        filterOptions.push(...tagsOptions);
      }
    } else {
      if (!personal) {
        filterOptions.push(
          {
            key: FilterGroups.filterAuthor,
            group: FilterGroups.filterAuthor,
            label: t("ByAuthor"),
            isHeader: true,
          },
          {
            key: "user",
            group: FilterGroups.filterAuthor,
            label: t("Translations:AddAuthor"),
            isSelector: true,
          }
        );
      }

      filterOptions.push(...typeOptions);

      if (!isRecentFolder && !isFavoritesFolder) {
        filterOptions.push(
          {
            key: FilterGroups.filterFolders,
            group: FilterGroups.filterFolders,
            label: t("Translations:Folders"),
            isHeader: true,
            withoutHeader: true,
            isLast: true,
          },
          {
            key: "false",
            group: FilterGroups.filterFolders,
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
          { key: "roomType", label: t("Common:Type"), default: true },
          { key: "Tags", label: t("Tags"), default: true },
          { key: "Author", label: t("Common:Owner"), default: true },
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

        if (group === FilterGroups.roomFilterType) {
          newFilter.type = null;
        }

        if (group === FilterGroups.roomFilterOwner) {
          newFilter.subjectId = null;
          newFilter.withoutMe = false;
        }

        if (group === FilterGroups.roomFilterTags) {
          const newTags = newFilter.tags;

          if (newTags?.length > 0) {
            const idx = newTags.findIndex((tag) => tag === key);

            if (idx > -1) {
              newTags.splice(idx, 1);
            }

            newFilter.tags = newTags.length > 0 ? newTags : null;

            newFilter.withoutTags = false;
          } else {
            newFilter.tags = null;
            newFilter.withoutTags = false;
          }
        }

        // if (group === FilterGroups.roomFilterContent) {
        //   newFilter.searchInContent = false;
        // }

        // if (group === FilterGroups.roomFilterFolders) {
        //   newFilter.withSubfolders = true;
        // }

        newFilter.page = 0;

        fetchRooms(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      } else {
        const newFilter = filter.clone();
        if (group === FilterGroups.filterType) {
          newFilter.filterType = null;
        }
        if (group === FilterGroups.filterAuthor) {
          newFilter.authorType = null;
        }
        if (group === FilterGroups.filterFolders) {
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
      filterHeader={t("AdvancedFilter")}
      placeholder={t("Common:Search")}
      view={t("Common:View")}
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
      setCurrentRoomsFilter,
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
      setCurrentRoomsFilter,
    };
  }
)(
  withRouter(
    withLayoutSize(
      withTranslation(["Files", "Common", "Translations"])(
        withLoader(observer(SectionFilterContent))(<Loaders.Filter />)
      )
    )
  )
);
