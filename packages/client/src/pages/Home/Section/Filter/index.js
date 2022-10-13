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
import {
  FilterType,
  RoomsType,
  RoomsProviderType,
  RoomsProviderTypeName,
} from "@docspace/common/constants";
import Loaders from "@docspace/common/components/Loaders";
import FilterInput from "@docspace/common/components/FilterInput";
import { withLayoutSize } from "@docspace/common/utils";
import { getDefaultRoomName } from "@docspace/client/src/helpers/filesUtils";

import withLoader from "../../../../HOCs/withLoader";
import { TableVersions } from "SRC_DIR/helpers/constants";

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

const getProviderType = (filterValues) => {
  const filterType = filterValues.find(
    (value) => value.group === FilterGroups.roomFilterProviderType
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

const getFilterContent = (filterValues) => {
  const filterContent = result(
    find(filterValues, (value) => {
      return value.group === FilterGroups.filterContent;
    }),
    "key"
  );

  return filterContent ? filterContent : null;
};

const getTags = (filterValues) => {
  const filterTags = filterValues.find(
    (value) => value.group === FilterGroups.roomFilterTags
  )?.key;

  const tags = filterTags?.length > 0 ? filterTags : null;

  return tags;
};

const TABLE_COLUMNS = `filesTableColumns_ver-${TableVersions.Files}`;

const COLUMNS_SIZE_INFO_PANEL = `filesColumnsSizeInfoPanel_ver-${TableVersions.Files}`;

const TABLE_ROOMS_COLUMNS = `roomsTableColumns_ver-${TableVersions.Rooms}`;

const COLUMNS_ROOMS_SIZE_INFO_PANEL = `roomsColumnsSizeInfoPanel_ver-${TableVersions.Rooms}`;

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
  isPersonalRoom,
  setCurrentRoomsFilter,
  providers,
}) => {
  const onFilter = React.useCallback(
    (data) => {
      if (isRooms) {
        const type = getType(data) || null;

        const owner = getOwner(data) || null;

        const subjectId =
          owner === FilterKeys.other || owner === FilterKeys.me
            ? userId
            : owner;

        const withoutMe = owner === FilterKeys.other;

        const providerType = getProviderType(data) || null;
        const tags = getTags(data) || null;

        // const withSubfolders =
        //   getFilterFolders(data) === FilterKeys.withSubfolders;

        // const withContent = getFilterContent(data) === FilterKeys.withContent;

        setIsLoading(true);

        const newFilter = roomsFilter.clone();

        newFilter.page = 0;
        newFilter.provider = providerType ? providerType : null;
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

        newFilter.excludeSubject = withoutMe;
        // newFilter.withSubfolders = withSubfolders;
        // newFilter.searchInContent = withContent;

        fetchRooms(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      } else {
        const filterType = getFilterType(data) || null;

        const authorType = getAuthorType(data);

        const withSubfolders = getSearchParams(data);
        const withContent = getFilterContent(data);

        const newFilter = filter.clone();
        newFilter.page = 0;

        newFilter.filterType = filterType;

        if (authorType === FilterKeys.me || authorType === FilterKeys.other) {
          newFilter.authorType = `user_${userId}`;
          newFilter.excludeSubject = authorType === FilterKeys.other;
        } else {
          newFilter.authorType = authorType ? `user_${authorType}` : null;
          newFilter.excludeSubject = null;
        }

        newFilter.withSubfolders =
          withSubfolders === FilterKeys.excludeSubfolders ? "false" : "true";
        newFilter.searchInContent = withContent === "true" ? "true" : null;

        setIsLoading(true);

        fetchFiles(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      }
    },
    [
      isRooms,
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
      isRooms,
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
      isRooms,
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
    return isRooms
      ? roomsFilter.filterValue
        ? roomsFilter.filterValue
        : ""
      : filter.search
      ? filter.search
      : "";
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

      if (roomsFilter.provider) {
        const provider = +roomsFilter.provider;

        const label = RoomsProviderTypeName[provider];

        filterValues.push({
          key: provider,
          label: label,
          group: FilterGroups.roomFilterProviderType,
        });
      }

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
        let label = isMe
          ? roomsFilter.excludeSubject
            ? t("Common:OtherLabel")
            : t("Common:MeLabel")
          : null;

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
      if (filter.withSubfolders === "false") {
        filterValues.push({
          key: FilterKeys.excludeSubfolders,
          label: t("ExcludeSubfolders"),
          group: FilterGroups.filterFolders,
        });
      }

      if (filter.searchInContent) {
        filterValues.push({
          key: "true",
          label: t("FileContents"),
          group: FilterGroups.filterContent,
        });
      }

      if (filter.filterType) {
        let label = "";

        switch (filter.filterType.toString()) {
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
          case FilterType.OFormTemplateOnly.toString():
            label = t("FormsTemplates");
            break;
          case FilterType.OFormOnly.toString():
            label = t("Forms");
            break;
        }

        filterValues.push({
          key: `${filter.filterType}`,
          label: label.toLowerCase(),
          group: FilterGroups.filterType,
        });
      }

      if (filter.authorType) {
        const isMe = userId === filter.authorType.replace("user_", "");

        let label = isMe
          ? filter.excludeSubject
            ? t("Common:OtherLabel")
            : t("Common:MeLabel")
          : null;

        if (!isMe) {
          const user = await getUser(filter.authorType.replace("user_", ""));

          label = user.displayName;
        }

        filterValues.push({
          key: isMe
            ? filter.excludeSubject
              ? FilterKeys.other
              : FilterKeys.me
            : filter.authorType.replace("user_", ""),
          group: FilterGroups.filterAuthor,
          label: label,
        });
      }
    }

    return filterValues;
  }, [
    filter.withSubfolders,
    filter.authorType,
    filter.filterType,
    filter.provider,
    filter.searchInContent,
    filter.excludeSubject,
    roomsFilter.provider,
    roomsFilter.type,
    roomsFilter.subjectId,
    roomsFilter.tags,
    roomsFilter.tags?.length,
    roomsFilter.excludeSubject,
    roomsFilter.withoutTags,
    // roomsFilter.withSubfolders,
    // roomsFilter.searchInContent,
    userId,
    isRooms,
  ]);

  const getFilterData = React.useCallback(async () => {
    const tags = await fetchTags();
    const connectedThirdParty = [];

    providers.forEach((item) => {
      if (connectedThirdParty.includes(item.provider_key)) return;
      connectedThirdParty.push(item.provider_key);
    });

    const isLastTypeOptionsRooms = !connectedThirdParty.length && !tags.length;

    const folders =
      !isFavoritesFolder && !isRecentFolder
        ? [
            {
              key: FilterType.FoldersOnly.toString(),
              group: FilterGroups.filterType,
              label: t("Translations:Folders").toLowerCase(),
            },
          ]
        : "";

    const images = !isRecentFolder
      ? [
          {
            key: FilterType.ImagesOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Images").toLowerCase(),
          },
        ]
      : "";

    const archives = !isRecentFolder
      ? [
          {
            key: FilterType.ArchiveOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Archives").toLowerCase(),
          },
        ]
      : "";

    const media = !isRecentFolder
      ? [
          {
            key: FilterType.MediaOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Media").toLowerCase(),
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
            isLast: isLastTypeOptionsRooms,
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
            isLast: true,
          },
          ...folders,
          {
            key: FilterType.DocumentsOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Common:Documents").toLowerCase(),
          },
          {
            key: FilterType.PresentationsOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Translations:Presentations").toLowerCase(),
          },
          {
            key: FilterType.SpreadsheetsOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Translations:Spreadsheets").toLowerCase(),
          },
          {
            key: FilterType.OFormTemplateOnly.toString(),
            group: FilterGroups.filterType,
            label: t("FormsTemplates").toLowerCase(),
          },
          {
            key: FilterType.OFormOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Forms").toLowerCase(),
          },
          ...archives,

          ...images,
          ...media,
        ];

    const ownerOptions = [
      {
        key: FilterGroups.roomFilterOwner,
        group: FilterGroups.roomFilterOwner,
        label: t("Common:Owner"),
        isHeader: true,
        withMultiItems: true,
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

      if (tags.length > 0) {
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

        const isLast = connectedThirdParty.length === 0;

        filterOptions.push({
          key: FilterGroups.roomFilterTags,
          group: FilterGroups.roomFilterTags,
          label: t("Tags"),
          isHeader: true,
          isLast,
        });

        filterOptions.push(...tagsOptions);
      }

      if (connectedThirdParty.length > 0) {
        const thirdPartyOptions = connectedThirdParty.map((thirdParty) => {
          const key = Object.entries(RoomsProviderType).find(
            (item) => item[0] === thirdParty
          )[1];

          const label = RoomsProviderTypeName[key];

          return {
            key,
            group: FilterGroups.roomFilterProviderType,
            label,
          };
        });

        filterOptions.push({
          key: FilterGroups.roomFilterProviderType,
          group: FilterGroups.roomFilterProviderType,
          label: t("Settings:ThirdPartyResource"),
          isHeader: true,
          isLast: true,
        });

        filterOptions.push(...thirdPartyOptions);
      }
    } else {
      if (!isRecentFolder && !isFavoritesFolder) {
        const foldersOptions = [
          {
            key: FilterGroups.filterFolders,
            group: FilterGroups.filterFolders,
            label: t("Common:Search"),
            isHeader: true,
            withoutSeparator: true,
          },
          {
            key: "folders",
            group: FilterGroups.filterFolders,
            label: "",
            withOptions: true,
            options: [
              { key: FilterKeys.withSubfolders, label: t("WithSubfolders") },
              {
                key: FilterKeys.excludeSubfolders,
                label: t("ExcludeSubfolders"),
              },
            ],
          },
        ];

        const contentOptions = [
          {
            key: FilterGroups.filterContent,
            group: FilterGroups.filterContent,
            isHeader: true,
            withoutHeader: true,
          },
          {
            key: "true",
            group: FilterGroups.filterContent,
            label: t("SearchByContent"),
            isCheckbox: true,
          },
        ];
        filterOptions.push(...foldersOptions);
        filterOptions.push(...contentOptions);
      }

      if (!isPersonalRoom) {
        const authorOption = [
          {
            key: FilterGroups.filterAuthor,
            group: FilterGroups.filterAuthor,
            label: t("ByAuthor"),
            isHeader: true,
            withMultiItems: true,
          },
          {
            key: FilterKeys.me,
            group: FilterGroups.filterAuthor,
            label: t("Common:MeLabel"),
          },
          {
            key: FilterKeys.other,
            group: FilterGroups.filterAuthor,
            label: t("Common:OtherLabel"),
          },
          {
            key: FilterKeys.user,
            group: FilterGroups.filterAuthor,
            label: t("Translations:AddAuthor"),
            isSelector: true,
          },
        ];

        filterOptions.push(...authorOption);
      }

      filterOptions.push(...typeOptions);
    }

    return filterOptions;
  }, [
    isFavoritesFolder,
    isRecentFolder,
    isRooms,
    t,
    personal,
    isPersonalRoom,
    providers,
  ]);

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
    const commonOptions = [];

    const name = { key: "AZ", label: t("Common:Name"), default: true };
    const modifiedDate = {
      key: "DateAndTime",
      label: t("ByLastModified"),
      default: true,
    };

    const type = { key: "Type", label: t("Common:Type"), default: true };
    const size = { key: "Size", label: t("Common:Size"), default: true };
    const creationDate = {
      key: "DateAndTimeCreation",
      label: t("ByCreation"),
      default: true,
    };
    const authorOption = {
      key: "Author",
      label: t("ByAuthor"),
      default: true,
    };

    const owner = { key: "Author", label: t("Common:Owner"), default: true };
    const tags = { key: "Tags", label: t("Tags"), default: true };
    const roomType = {
      key: "roomType",
      label: t("Common:Type"),
      default: true,
    };

    commonOptions.push(name);

    if (viewAs === "table") {
      if (isRooms) {
        const availableSort = localStorage
          ?.getItem(`${TABLE_ROOMS_COLUMNS}=${userId}`)
          ?.split(",");

        const infoPanelColumnsSize = localStorage
          ?.getItem(`${COLUMNS_ROOMS_SIZE_INFO_PANEL}=${userId}`)
          ?.split(" ");

        if (availableSort?.includes("Type")) {
          const idx = availableSort.findIndex((x) => x === "Type");
          const hide = infoPanelVisible && infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(roomType);
        }

        if (availableSort?.includes("Tags")) {
          const idx = availableSort.findIndex((x) => x === "Tags");
          const hide = infoPanelVisible && infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(tags);
        }

        if (availableSort?.includes("Owner")) {
          const idx = availableSort.findIndex((x) => x === "Owner");
          const hide = infoPanelVisible && infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(owner);
        }

        if (availableSort?.includes("Activity")) {
          const idx = availableSort.findIndex((x) => x === "Activity");
          const hide = infoPanelVisible && infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(modifiedDate);
        }
      } else {
        const availableSort = localStorage
          ?.getItem(`${TABLE_COLUMNS}=${userId}`)
          ?.split(",");

        const infoPanelColumnsSize = localStorage
          ?.getItem(`${COLUMNS_SIZE_INFO_PANEL}=${userId}`)
          ?.split(" ");

        if (availableSort?.includes("Author") && !isPersonalRoom) {
          const idx = availableSort.findIndex((x) => x === "Author");
          const hide = infoPanelVisible && infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(authorOption);
        }
        if (availableSort?.includes("Created")) {
          const idx = availableSort.findIndex((x) => x === "Created");
          const hide = infoPanelVisible && infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(creationDate);
        }
        if (availableSort?.includes("Modified")) {
          const idx = availableSort.findIndex((x) => x === "Modified");
          const hide = infoPanelVisible && infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(modifiedDate);
        }
        if (availableSort?.includes("Size")) {
          const idx = availableSort.findIndex((x) => x === "Size");
          const hide = infoPanelVisible && infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(size);
        }
        if (availableSort?.includes("Type")) {
          const idx = availableSort.findIndex((x) => x === "Type");
          const hide = infoPanelVisible && infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(type);
        }
      }
    } else {
      if (isRooms) {
        commonOptions.push(roomType);
        commonOptions.push(tags);
        commonOptions.push(owner);
        commonOptions.push(modifiedDate);
      } else {
        commonOptions.push(authorOption);
        commonOptions.push(creationDate);
        commonOptions.push(modifiedDate);
        commonOptions.push(size);
        commonOptions.push(type);
      }
    }

    return commonOptions;
  }, [personal, isRooms, t, userId, infoPanelVisible, viewAs, isPersonalRoom]);

  const removeSelectedItem = React.useCallback(
    ({ key, group }) => {
      if (isRooms) {
        setIsLoading(true);

        const newFilter = roomsFilter.clone();

        if (group === FilterGroups.roomFilterProviderType) {
          newFilter.provider = null;
        }

        if (group === FilterGroups.roomFilterType) {
          newFilter.type = null;
        }

        if (group === FilterGroups.roomFilterOwner) {
          newFilter.subjectId = null;
          newFilter.excludeSubject = false;
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
          newFilter.excludeSubject = null;
        }
        if (group === FilterGroups.filterFolders) {
          newFilter.withSubfolders = "true";
        }
        if (group === FilterGroups.filterContent) {
          newFilter.searchInContent = null;
        }

        newFilter.page = 0;

        setIsLoading(true);

        fetchFiles(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      }
    },
    [
      isRooms,
      fetchFiles,
      fetchRooms,
      setIsLoading,
      roomsFilter,
      filter,
      selectedFolderId,
    ]
  );

  const clearAll = () => {
    if (isRooms) {
      setIsLoading(true);

      fetchRooms(selectedFolderId).finally(() => setIsLoading(false));
    } else {
      setIsLoading(true);

      fetchFiles(selectedFolderId).finally(() => setIsLoading(false));
    }
  };

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
      filterHeader={t("Common:AdvancedFilter")}
      placeholder={t("Common:Search")}
      view={t("Common:View")}
      isFavoritesFolder={isFavoritesFolder}
      isRecentFolder={isRecentFolder}
      isPersonalRoom={isPersonalRoom}
      isRooms={isRooms}
      removeSelectedItem={removeSelectedItem}
      clearAll={clearAll}
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
      thirdPartyStore,
    } = filesStore;

    const { providers } = thirdPartyStore;

    const { fetchTags } = tagsStore;

    const { user } = auth.userStore;
    const { customNames, personal } = auth.settingsStore;
    const {
      isFavoritesFolder,
      isRecentFolder,
      isRoomsFolder,
      isArchiveFolder,
      isPersonalRoom,
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
      isPersonalRoom,
      infoPanelVisible,
      setCurrentRoomsFilter,
      providers,
    };
  }
)(
  withRouter(
    withLayoutSize(
      withTranslation(["Files", "Settings", "Common", "Translations"])(
        withLoader(observer(SectionFilterContent))(<Loaders.Filter />)
      )
    )
  )
);
