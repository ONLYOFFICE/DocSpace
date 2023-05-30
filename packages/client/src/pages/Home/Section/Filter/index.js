import React, { useCallback, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useLocation } from "react-router-dom";
import { isMobile, isMobileOnly } from "react-device-detect";
import { withTranslation } from "react-i18next";
import find from "lodash/find";
import result from "lodash/result";

import FilterInput from "@docspace/common/components/FilterInput";
import Loaders from "@docspace/common/components/Loaders";
import { withLayoutSize } from "@docspace/common/utils";
import { getUser } from "@docspace/common/api/people";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import {
  FilterGroups,
  FilterKeys,
  FilterType,
  RoomsType,
  RoomsProviderType,
  RoomsProviderTypeName,
  FilterSubject,
  RoomSearchArea,
  EmployeeType,
  EmployeeStatus,
  PaymentsType,
  AccountLoginType,
} from "@docspace/common/constants";

import { getDefaultRoomName } from "SRC_DIR/helpers/filesUtils";
import withLoader from "SRC_DIR/HOCs/withLoader";
import {
  TableVersions,
  SortByFieldName,
  SSO_LABEL,
} from "SRC_DIR/helpers/constants";

import ViewRowsReactSvgUrl from "PUBLIC_DIR/images/view-rows.react.svg?url";
import ViewTilesReactSvgUrl from "PUBLIC_DIR/images/view-tiles.react.svg?url";

import { showLoader, hideLoader } from "./FilterUtils";
import { getRoomInfo } from "@docspace/common/api/rooms";

const getAccountLoginType = (filterValues) => {
  const accountLoginType = result(
    find(filterValues, (value) => {
      return value.group === "filter-login-type";
    }),
    "key"
  );

  return accountLoginType || null;
};

const getFilterType = (filterValues) => {
  const filterType = result(
    find(filterValues, (value) => {
      return value.group === FilterGroups.filterType;
    }),
    "key"
  );

  return filterType?.toString() ? +filterType : null;
};

const getSubjectFilter = (filterValues) => {
  const subjectFilter = result(
    find(filterValues, (value) => {
      return value.group === FilterGroups.roomFilterOwner;
    }),
    "key"
  );

  return subjectFilter?.toString() ? subjectFilter?.toString() : null;
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

const getRoomId = (filterValues) => {
  const filterRoomId = result(
    find(filterValues, (value) => {
      return value.group === FilterGroups.filterRoom;
    }),
    "key"
  );

  return filterRoomId || null;
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

const getSubjectId = (filterValues) => {
  const filterOwner = result(
    find(filterValues, (value) => {
      return value.group === FilterGroups.roomFilterSubject;
    }),
    "key"
  );

  return filterOwner ? filterOwner : null;
};

const getStatus = (filterValues) => {
  const employeeStatus = result(
    find(filterValues, (value) => {
      return value.group === "filter-status";
    }),
    "key"
  );

  return employeeStatus ? +employeeStatus : null;
};

const getRole = (filterValues) => {
  const employeeStatus = result(
    find(filterValues, (value) => {
      return value.group === "filter-type";
    }),
    "key"
  );

  return employeeStatus || null;
};

const getPayments = (filterValues) => {
  const employeeStatus = result(
    find(filterValues, (value) => {
      return value.group === "filter-account";
    }),
    "key"
  );

  return employeeStatus || null;
};

const getGroup = (filterValues) => {
  const groupId = result(
    find(filterValues, (value) => {
      return value.group === "filter-other";
    }),
    "key"
  );

  return groupId || null;
};

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

const TABLE_TRASH_COLUMNS = `trashTableColumns_ver-${TableVersions.Trash}`;

const COLUMNS_TRASH_SIZE_INFO_PANEL = `trashColumnsSizeInfoPanel_ver-${TableVersions.Trash}`;

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
  isTrash,
  userId,
  isPersonalRoom,
  setCurrentRoomsFilter,
  providers,
  isLoadedEmptyPage,
  isEmptyPage,
  clearSearch,
  setClearSearch,
  setMainButtonMobileVisible,
  isArchiveFolder,
  accountsViewAs,
  groups,
  fetchPeople,
  accountsFilter,
}) => {
  const location = useLocation();

  const isAccountsPage = location.pathname.includes("accounts");

  const [selectedFilterValues, setSelectedFilterValues] = React.useState(null);

  const onFilter = React.useCallback(
    (data) => {
      if (isAccountsPage) {
        setIsLoading(true);
        const status = getStatus(data);

        const role = getRole(data);
        const group = getGroup(data);
        const payments = getPayments(data);
        const accountLoginType = getAccountLoginType(data);

        const newFilter = accountsFilter.clone();

        if (status === 3) {
          newFilter.employeeStatus = EmployeeStatus.Disabled;
          newFilter.activationStatus = null;
        } else if (status === 2) {
          newFilter.employeeStatus = EmployeeStatus.Active;
          newFilter.activationStatus = status;
        } else {
          newFilter.employeeStatus = null;
          newFilter.activationStatus = status;
        }

        newFilter.page = 0;

        newFilter.role = role;

        newFilter.group = group;

        newFilter.payments = payments;

        newFilter.accountLoginType = accountLoginType;

        //console.log(newFilter);

        fetchPeople(newFilter, true).finally(() => setIsLoading(false));
      } else if (isRooms) {
        const type = getType(data) || null;

        const subjectId = getSubjectId(data) || null;

        const subjectFilter = getSubjectFilter(data) || null;

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

        newFilter.subjectFilter = null;
        newFilter.subjectId = null;

        if (subjectId) {
          newFilter.subjectId = subjectId;

          if (subjectId === FilterKeys.me) {
            newFilter.subjectId = `${userId}`;
          }

          newFilter.subjectFilter = subjectFilter?.toString()
            ? subjectFilter.toString()
            : FilterSubject.Member;
        }

        if (tags) {
          if (!tags?.length) {
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

        const roomId = getRoomId(data);

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

        if (isTrash) {
          newFilter.roomId = roomId;
        }

        setIsLoading(true);

        fetchFiles(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      }
    },
    [
      isRooms,
      isAccountsPage,
      isTrash,
      fetchFiles,
      fetchRooms,
      fetchPeople,
      setIsLoading,
      roomsFilter,
      accountsFilter,
      filter,
      selectedFolderId,
    ]
  );

  const onClearFilter = useCallback(() => {
    if (isAccountsPage) {
      return;
    }

    if (isRooms) {
      const newFilter = RoomsFilter.getDefault();
      newFilter.searchArea = roomsFilter.searchArea;

      fetchRooms(selectedFolderId, newFilter).finally(() =>
        setIsLoading(false)
      );
    } else {
      const newFilter = filter.clone();
      newFilter.page = 0;
      newFilter.filterValue = "";

      setIsLoading(true);

      fetchFiles(selectedFolderId, newFilter).finally(() => {
        setIsLoading(false);
      });
    }
  }, [
    isRooms,
    setIsLoading,
    fetchFiles,
    fetchRooms,

    selectedFolderId,
    filter,

    roomsFilter,
    isAccountsPage,
  ]);

  const onSearch = React.useCallback(
    (data = "") => {
      if (isAccountsPage) {
        const newFilter = accountsFilter.clone();
        newFilter.page = 0;
        newFilter.search = data;

        setIsLoading(true);

        fetchPeople(newFilter, true).finally(() => setIsLoading(false));
      } else if (isRooms) {
        const newFilter = roomsFilter.clone();

        newFilter.page = 0;
        newFilter.filterValue = data;

        setIsLoading(true);

        fetchRooms(selectedFolderId, newFilter).finally(() =>
          setIsLoading(false)
        );
      } else {
        const newFilter = filter.clone();
        newFilter.page = 0;
        newFilter.search = data;

        setIsLoading(true);

        fetchFiles(selectedFolderId, newFilter).finally(() => {
          setIsLoading(false);
        });
      }
    },
    [
      isRooms,
      isAccountsPage,
      setIsLoading,
      fetchFiles,
      fetchRooms,
      fetchPeople,
      selectedFolderId,
      filter,
      roomsFilter,
      accountsFilter,
    ]
  );

  const onSort = React.useCallback(
    (sortId, sortDirection) => {
      const sortBy = sortId;
      const sortOrder = sortDirection === "desc" ? "descending" : "ascending";

      const newFilter = isAccountsPage
        ? accountsFilter.clone()
        : isRooms
        ? roomsFilter.clone()
        : filter.clone();
      newFilter.page = 0;
      newFilter.sortBy = sortBy;
      newFilter.sortOrder = sortOrder;

      setIsLoading(true);

      if (isAccountsPage) {
        fetchPeople(newFilter, true).finally(() => setIsLoading(false));
      } else if (isRooms) {
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
      isAccountsPage,
      setIsLoading,
      fetchFiles,
      fetchRooms,
      fetchPeople,
      selectedFolderId,
      filter,
      roomsFilter,
      accountsFilter,
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
    return isAccountsPage
      ? accountsFilter.search
        ? accountsFilter.search
        : ""
      : isRooms
      ? roomsFilter.filterValue
        ? roomsFilter.filterValue
        : ""
      : filter.search
      ? filter.search
      : "";
  }, [
    isRooms,
    isAccountsPage,
    roomsFilter.filterValue,
    filter.search,
    accountsFilter.search,
  ]);

  const getSelectedSortData = React.useCallback(() => {
    const currentFilter = isAccountsPage
      ? accountsFilter
      : isRooms
      ? roomsFilter
      : filter;
    return {
      sortDirection: currentFilter.sortOrder === "ascending" ? "asc" : "desc",
      sortId: currentFilter.sortBy,
    };
  }, [
    isRooms,
    isAccountsPage,
    filter.sortOrder,
    filter.sortBy,
    roomsFilter.sortOrder,
    roomsFilter.sortBy,
    accountsFilter.sortOrder,
    accountsFilter.sortBy,
  ]);

  const getSelectedFilterData = React.useCallback(async () => {
    const filterValues = [];

    if (isAccountsPage) {
      if (accountsFilter.employeeStatus || accountsFilter.activationStatus) {
        const key =
          accountsFilter.employeeStatus === 2
            ? 3
            : accountsFilter.activationStatus;
        let label = "";

        switch (key) {
          case 1:
            label = t("Common:Active");
            break;
          case 2:
            label = t("PeopleTranslations:PendingTitle");
            break;
          case 3:
            label = t("PeopleTranslations:DisabledEmployeeStatus");
            break;
        }

        filterValues.push({
          key,
          label,
          group: "filter-status",
        });
      }

      if (accountsFilter.role) {
        let label = null;

        switch (+accountsFilter.role) {
          case EmployeeType.Admin:
            label = t("Common:DocSpaceAdmin");
            break;
          case EmployeeType.User:
            label = t("Common:RoomAdmin");
            break;
          case EmployeeType.Collaborator:
            label = t("Common:PowerUser");
            break;
          case EmployeeType.Guest:
            label = t("Common:User");
            break;
          default:
            label = "";
        }

        filterValues.push({
          key: +accountsFilter.role,
          label: label,
          group: "filter-type",
        });
      }

      if (accountsFilter?.payments?.toString()) {
        filterValues.push({
          key: accountsFilter.payments.toString(),
          label:
            PaymentsType.Paid === accountsFilter.payments.toString()
              ? t("Common:Paid")
              : t("SmartBanner:Price"),
          group: "filter-account",
        });
      }

      if (accountsFilter?.accountLoginType?.toString()) {
        const label =
          AccountLoginType.SSO === accountsFilter.accountLoginType.toString()
            ? SSO_LABEL
            : AccountLoginType.LDAP ===
              accountsFilter.accountLoginType.toString()
            ? t("PeopleTranslations:LDAPLbl")
            : t("PeopleTranslations:StandardLogin");
        filterValues.push({
          key: accountsFilter.accountLoginType.toString(),
          label: label,
          group: "filter-login-type",
        });
      }

      if (accountsFilter.group) {
        const group = groups.find((group) => group.id === accountsFilter.group);

        if (group) {
          filterValues.push({
            key: accountsFilter.group,
            label: group.name,
            group: "filter-other",
          });
        }
      }

      const currentFilterValues = [];

      setSelectedFilterValues((value) => {
        if (!value) {
          currentFilterValues.push(...filterValues);
          return filterValues.map((f) => ({ ...f }));
        }

        const items = value.map((v) => {
          const item = filterValues.find((f) => f.group === v.group);

          if (item) {
            if (item.isMultiSelect) {
              let isEqual = true;

              item.key.forEach((k) => {
                if (!v.key.includes(k)) {
                  isEqual = false;
                }
              });

              if (isEqual) return item;

              return false;
            } else {
              if (item.key === v.key) return item;
              return false;
            }
          } else {
            return false;
          }
        });

        const newItems = filterValues.filter(
          (v) => !items.find((i) => i.group === v.group)
        );

        items.push(...newItems);

        currentFilterValues.push(...items.filter((i) => i));

        return items.filter((i) => i);
      });

      return currentFilterValues;
    }

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

      if (roomsFilter.subjectId) {
        const user = await getUser(roomsFilter.subjectId);
        const isMe = userId === roomsFilter.subjectId;

        let label = isMe ? t("Common:MeLabel") : user.displayName;

        const subject = {
          key: isMe ? FilterKeys.me : roomsFilter.subjectId,
          group: FilterGroups.roomFilterSubject,
          label: label,
        };

        if (roomsFilter.subjectFilter?.toString()) {
          if (roomsFilter.subjectFilter.toString() === FilterSubject.Owner) {
            subject.selectedLabel = t("Common:Owner") + ": " + label;
          }

          filterValues.push(subject);

          filterValues.push({
            key: roomsFilter?.subjectFilter?.toString(),
            group: FilterGroups.roomFilterOwner,
          });
        } else {
          filterValues.push(subject);
        }
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

      if (roomsFilter?.tags?.length > 0) {
        filterValues.push({
          key: roomsFilter.tags,
          group: FilterGroups.roomFilterTags,
          isMultiSelect: true,
        });
      }

      if (roomsFilter.provider) {
        const provider = +roomsFilter.provider;

        const label = RoomsProviderTypeName[provider];

        filterValues.push({
          key: provider,
          label: label,
          group: FilterGroups.roomFilterProviderType,
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

      if (filter.roomId) {
        const room = await getRoomInfo(filter.roomId);
        const label = room.title;

        filterValues.push({
          key: filter.roomId,
          group: FilterGroups.filterRoom,
          label: label,
        });
      }
    }

    // return filterValues;
    const currentFilterValues = [];

    setSelectedFilterValues((value) => {
      if (!value) {
        currentFilterValues.push(...filterValues);
        return filterValues.map((f) => ({ ...f }));
      }

      const items = value.map((v) => {
        const item = filterValues.find((f) => f.group === v.group);

        if (item) {
          if (item.isMultiSelect) {
            let isEqual = true;

            item.key.forEach((k) => {
              if (!v.key.includes(k)) {
                isEqual = false;
              }
            });

            if (isEqual) return item;

            return false;
          } else {
            if (item.key === v.key) return item;
            return false;
          }
        } else {
          return false;
        }
      });

      const newItems = filterValues.filter(
        (v) => !items.find((i) => i.group === v.group)
      );

      items.push(...newItems);

      currentFilterValues.push(...items.filter((i) => i));

      return items.filter((i) => i);
    });

    return currentFilterValues;
  }, [
    filter.withSubfolders,
    filter.authorType,
    filter.roomId,
    filter.filterType,
    filter.searchInContent,
    filter.excludeSubject,
    roomsFilter.provider,
    roomsFilter.type,
    roomsFilter.subjectId,
    roomsFilter.subjectFilter,
    roomsFilter.tags,
    roomsFilter.tags?.length,
    roomsFilter.excludeSubject,
    roomsFilter.withoutTags,
    // roomsFilter.withSubfolders,
    // roomsFilter.searchInContent,
    userId,
    isRooms,
    isAccountsPage,
    accountsFilter.employeeStatus,
    accountsFilter.activationStatus,
    accountsFilter.role,
    accountsFilter.payments,
    accountsFilter.group,
    accountsFilter.accountLoginType,
    t,
  ]);

  const getFilterData = React.useCallback(async () => {
    if (isAccountsPage) {
      const statusItems = [
        {
          id: "filter_status-user",
          key: "filter-status",
          group: "filter-status",
          label: t("People:UserStatus"),
          isHeader: true,
        },
        {
          id: "filter_status-active",
          key: 1,
          group: "filter-status",
          label: t("Common:Active"),
        },
        {
          id: "filter_status-pending",
          key: 2,
          group: "filter-status",
          label: t("PeopleTranslations:PendingTitle"),
        },
        {
          id: "filter_status-disabled",
          key: 3,
          group: "filter-status",
          label: t("PeopleTranslations:DisabledEmployeeStatus"),
        },
      ];

      const typeItems = [
        {
          key: "filter-type",
          group: "filter-type",
          label: t("Common:Type"),
          isHeader: true,
        },
        {
          id: "filter_type-docspace-admin",
          key: EmployeeType.Admin,
          group: "filter-type",
          label: t("Common:DocSpaceAdmin"),
        },
        {
          id: "filter_type-room-admin",
          key: EmployeeType.User,
          group: "filter-type",
          label: t("Common:RoomAdmin"),
        },
        {
          id: "filter_type-room-admin",
          key: EmployeeType.Collaborator,
          group: "filter-type",
          label: t("Common:PowerUser"),
        },
        {
          id: "filter_type-user",
          key: EmployeeType.Guest,
          group: "filter-type",
          label: t("Common:User"),
        },
      ];

      // const roleItems = [
      //   {
      //     key: "filter-role",
      //     group: "filter-role",
      //     label: "Role in room",
      //     isHeader: true,
      //   },
      //   { key: "1", group: "filter-role", label: "Room manager" },
      //   { key: "2", group: "filter-role", label: "Co-worker" },
      //   { key: "3", group: "filter-role", label: "Editor" },
      //   { key: "4", group: "filter-role", label: "Form filler" },
      //   { key: "5", group: "filter-role", label: "Reviewer" },
      //   { key: "6", group: "filter-role", label: "Commentator" },
      //   { key: "7", group: "filter-role", label: "Viewer" },
      // ];

      const accountItems = [
        {
          key: "filter-account",
          group: "filter-account",
          label: t("ConnectDialog:Account"),
          isHeader: true,
          isLast: false,
        },
        {
          key: PaymentsType.Paid,
          group: "filter-account",
          label: t("Common:Paid"),
        },
        {
          key: PaymentsType.Free,
          group: "filter-account",
          label: t("SmartBanner:Price"),
        },
      ];

      // const roomItems = [
      //   {
      //     key: "filter-status",
      //     group: "filter-status",
      //     label: t("People:UserStatus"),
      //     isHeader: true,
      //   },
      //   {
      //     key: "1",
      //     group: "filter-status",
      //     label: t("Common:Active"),
      //     isSelector: true,
      //     selectorType: "room",
      //   },
      // ];

      const accountLoginTypeItems = [
        {
          key: "filter-login-type",
          group: "filter-login-type",
          label: t("PeopleTranslations:AccountLoginType"),
          isHeader: true,
          isLast: true,
        },
        {
          key: AccountLoginType.SSO,
          group: "filter-login-type",
          label: SSO_LABEL,
        },
        //TODO: uncomment after ldap be ready
        /*{
          key: AccountLoginType.LDAP,
          group: "filter-login-type",
          label: t("PeopleTranslations:LDAPLbl"),
        },*/
        {
          key: AccountLoginType.STANDART,
          group: "filter-login-type",
          label: t("PeopleTranslations:StandardLogin"),
        },
      ];

      const filterOptions = [];

      filterOptions.push(...statusItems);
      filterOptions.push(...typeItems);
      // filterOptions.push(...roleItems);
      filterOptions.push(...accountItems);
      // filterOptions.push(...roomItems);
      filterOptions.push(...accountLoginTypeItems);

      return filterOptions;
    }

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
              id: "filter_type-folders",
              key: FilterType.FoldersOnly.toString(),
              group: FilterGroups.filterType,
              label: t("Translations:Folders").toLowerCase(),
            },
          ]
        : "";

    const images = !isRecentFolder
      ? [
          {
            id: "filter_type-images",
            key: FilterType.ImagesOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Images").toLowerCase(),
          },
        ]
      : "";

    const archives = !isRecentFolder
      ? [
          {
            id: "filter_type-archive",
            key: FilterType.ArchiveOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Archives").toLowerCase(),
          },
        ]
      : "";

    const media = !isRecentFolder
      ? [
          {
            id: "filter_type-media",
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
          ...Object.values(RoomsType).map((roomType) => {
            switch (roomType) {
              case RoomsType.FillingFormsRoom:
                return {
                  id: "filter_type-filling-form",
                  key: RoomsType.FillingFormsRoom,
                  group: FilterGroups.roomFilterType,
                  label: t("FillingFormRooms"),
                };
              case RoomsType.EditingRoom:
                return {
                  id: "filter_type-collaboration",
                  key: RoomsType.EditingRoom,
                  group: FilterGroups.roomFilterType,
                  label: t("CollaborationRooms"),
                };
              case RoomsType.ReviewRoom:
                return {
                  id: "filter_type-review",
                  key: RoomsType.ReviewRoom,
                  group: FilterGroups.roomFilterType,
                  label: t("Common:Review"),
                };
              case RoomsType.ReadOnlyRoom:
                return {
                  id: "filter_type-view-only",
                  key: RoomsType.ReadOnlyRoom,
                  group: FilterGroups.roomFilterType,
                  label: t("ViewOnlyRooms"),
                };
              case RoomsType.CustomRoom:
              default:
                return {
                  id: "filter_type-custom",
                  key: RoomsType.CustomRoom,
                  group: FilterGroups.roomFilterType,
                  label: t("CustomRooms"),
                };
            }
          }),
        ]
      : [
          {
            key: FilterGroups.filterType,
            group: FilterGroups.filterType,
            label: t("Common:Type"),
            isHeader: true,
            isLast: !isTrash,
          },
          ...folders,
          {
            id: "filter_type-documents",
            key: FilterType.DocumentsOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Common:Documents").toLowerCase(),
          },
          {
            id: "filter_type-presentations",
            key: FilterType.PresentationsOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Translations:Presentations").toLowerCase(),
          },
          {
            id: "filter_type-spreadsheets",
            key: FilterType.SpreadsheetsOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Translations:Spreadsheets").toLowerCase(),
          },
          {
            id: "filter_type-form-templates",
            key: FilterType.OFormTemplateOnly.toString(),
            group: FilterGroups.filterType,
            label: t("FormsTemplates").toLowerCase(),
          },
          {
            id: "filter_type-forms",
            key: FilterType.OFormOnly.toString(),
            group: FilterGroups.filterType,
            label: t("Forms").toLowerCase(),
          },
          ...archives,

          ...images,
          ...media,
          {
            id: "filter_type-all-files",
            key: FilterType.FilesOnly.toString(),
            group: FilterGroups.filterType,
            label: t("AllFiles").toLowerCase(),
          },
        ];

    const subjectOptions = [
      {
        key: FilterGroups.roomFilterSubject,
        group: FilterGroups.roomFilterSubject,
        label: t("Common:Member"),
        isHeader: true,
        withoutSeparator: true,
        withMultiItems: true,
      },
      {
        id: "filter_author-me",
        key: FilterKeys.me,
        group: FilterGroups.roomFilterSubject,
        label: t("Common:MeLabel"),
      },
      {
        id: "filter_author-other",
        key: FilterKeys.other,
        group: FilterGroups.roomFilterSubject,
        label: t("Common:OtherLabel"),
      },
      {
        id: "filter_author-user",
        key: FilterKeys.user,
        group: FilterGroups.roomFilterSubject,
        displaySelectorType: "link",
      },
    ];

    const ownerOptions = [
      {
        key: FilterGroups.roomFilterOwner,
        group: FilterGroups.roomFilterOwner,
        isHeader: true,
        withoutHeader: true,
      },
      {
        id: "filter_author-user",
        key: FilterSubject.Owner,
        group: FilterGroups.roomFilterOwner,
        label: t("Translations:SearchByOwner"),
        isCheckbox: true,
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

      filterOptions.push(...subjectOptions);
      filterOptions.push(...ownerOptions);

      filterOptions.push(...typeOptions);

      if (tags.length > 0) {
        const tagsOptions = tags.map((tag) => ({
          key: tag,
          group: FilterGroups.roomFilterTags,
          label: tag,
          isMultiSelect: true,
        }));

        const isLast = connectedThirdParty.length === 0;

        filterOptions.push({
          key: FilterGroups.roomFilterTags,
          group: FilterGroups.roomFilterTags,
          label: t("Common:Tags"),
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
      if (!isRecentFolder && !isFavoritesFolder && !isTrash) {
        const foldersOptions = [
          {
            key: FilterGroups.filterFolders,
            group: FilterGroups.filterFolders,
            label: t("Common:Search"),
            isHeader: true,
            withoutSeparator: true,
          },
          {
            id: "filter_folders",
            key: "folders",
            group: FilterGroups.filterFolders,
            label: "",
            withOptions: true,
            options: [
              {
                id: "filter_folders_with-subfolders",
                key: FilterKeys.withSubfolders,
                label: t("WithSubfolders"),
              },
              {
                id: "filter_folders_exclude-subfolders",
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
            id: "filter_search-by-file-contents",
            key: "true",
            group: FilterGroups.filterContent,
            label: t("SearchByContent"),
            isCheckbox: true,
          },
        ];
        filterOptions.push(...foldersOptions);
        filterOptions.push(...contentOptions);
      }

      const authorOption = [
        {
          key: FilterGroups.filterAuthor,
          group: FilterGroups.filterAuthor,
          label: t("ByAuthor"),
          isHeader: true,
          withMultiItems: true,
        },
        {
          id: "filter_author-me",
          key: FilterKeys.me,
          group: FilterGroups.filterAuthor,
          label: t("Common:MeLabel"),
        },
        {
          id: "filter_author-other",
          key: FilterKeys.other,
          group: FilterGroups.filterAuthor,
          label: t("Common:OtherLabel"),
        },
        {
          id: "filter_author-user",
          key: FilterKeys.user,
          group: FilterGroups.filterAuthor,
          displaySelectorType: "link",
        },
      ];

      filterOptions.push(...authorOption);
      filterOptions.push(...typeOptions);

      if (isTrash) {
        const roomOption = [
          {
            id: "filter_search-by-room-content-header",
            key: "filter_search-by-room-content-header",
            group: FilterGroups.filterRoom,
            label: "Room",
            isHeader: true,
            isLast: true,
          },
          {
            id: "filter_search-by-room-content",
            key: "filter_search-by-room-content",
            group: FilterGroups.filterRoom,
            withoutHeader: true,
            label: "Select room",
            displaySelectorType: "button",
            isLast: true,
          },
        ];
        filterOptions.push(...roomOption);
      }
    }
    return filterOptions;
  }, [
    t,
    personal,
    providers,
    isPersonalRoom,
    isRooms,
    isAccountsPage,
    isFavoritesFolder,
    isRecentFolder,
    isTrash,
  ]);

  const getViewSettingsData = React.useCallback(() => {
    const viewSettings = [
      {
        id: "view-switch_rows",
        value: "row",
        label: t("ViewList"),
        icon: ViewRowsReactSvgUrl,
      },
      {
        id: "view-switch_tiles",
        value: "tile",
        label: t("ViewTiles"),
        icon: ViewTilesReactSvgUrl,
        callback: createThumbnails,
      },
    ];

    return viewSettings;
  }, [createThumbnails]);

  const getSortData = React.useCallback(() => {
    if (isAccountsPage) {
      return [
        {
          id: "sory-by_first-name",
          key: "firstname",
          label: t("Common:ByFirstNameSorting"),
          default: true,
        },
        {
          id: "sory-by_last-name",
          key: "lastname",
          label: t("Common:ByLastNameSorting"),
          default: true,
        },
        {
          id: "sory-by_type",
          key: "type",
          label: t("Common:Type"),
          default: true,
        },
        {
          id: "sory-by_email",
          key: "email",
          label: t("Common:Email"),
          default: true,
        },
      ];
    }

    const commonOptions = [];

    const name = {
      id: "sort-by_name",
      key: SortByFieldName.Name,
      label: t("Common:Name"),
      default: true,
    };
    const modifiedDate = {
      id: "sort-by_modified",
      key: SortByFieldName.ModifiedDate,
      label: t("Common:LastModifiedDate"),
      default: true,
    };
    const room = {
      id: "sort-by_room",
      key: SortByFieldName.Room,
      label: t("Common:Room"),
      default: true,
    };
    const authorOption = {
      id: "sort-by_author",
      key: SortByFieldName.Author,
      label: t("ByAuthor"),
      default: true,
    };
    const creationDate = {
      id: "sort-by_created",
      key: SortByFieldName.CreationDate,
      label: t("InfoPanel:CreationDate"),
      default: true,
    };
    const owner = {
      id: "sort-by_owner",
      key: SortByFieldName.Author,
      label: t("Common:Owner"),
      default: true,
    };
    const erasure = {
      id: "sort-by_erasure",
      key: SortByFieldName.ModifiedDate,
      label: t("ByErasure"),
      default: true,
    };
    const tags = {
      id: "sort-by_tags",
      key: SortByFieldName.Tags,
      label: t("Common:Tags"),
      default: true,
    };
    const size = {
      id: "sort-by_size",
      key: SortByFieldName.Size,
      label: t("Common:Size"),
      default: true,
    };
    const type = {
      id: "sort-by_type",
      key: SortByFieldName.Type,
      label: t("Common:Type"),
      default: true,
    };
    const roomType = {
      id: "sort-by_room-type",
      key: SortByFieldName.RoomType,
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
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(roomType);
        }

        if (availableSort?.includes("Tags")) {
          const idx = availableSort.findIndex((x) => x === "Tags");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(tags);
        }

        if (availableSort?.includes("Owner")) {
          const idx = availableSort.findIndex((x) => x === "Owner");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(owner);
        }

        if (availableSort?.includes("Activity")) {
          const idx = availableSort.findIndex((x) => x === "Activity");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(modifiedDate);
        }
      } else if (isTrash) {
        const availableSort = localStorage
          ?.getItem(`${TABLE_TRASH_COLUMNS}=${userId}`)
          ?.split(",");

        const infoPanelColumnsSize = localStorage
          ?.getItem(`${COLUMNS_TRASH_SIZE_INFO_PANEL}=${userId}`)
          ?.split(" ");

        if (availableSort?.includes("Room")) {
          const idx = availableSort.findIndex((x) => x === "Room");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(room);
        }
        if (availableSort?.includes("AuthorTrash")) {
          const idx = availableSort.findIndex((x) => x === "AuthorTrash");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(authorOption);
        }
        if (availableSort?.includes("CreatedTrash")) {
          const idx = availableSort.findIndex((x) => x === "CreatedTrash");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(creationDate);
        }
        if (availableSort?.includes("Erasure")) {
          const idx = availableSort.findIndex((x) => x === "Erasure");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(erasure);
        }
        if (availableSort?.includes("SizeTrash")) {
          const idx = availableSort.findIndex((x) => x === "SizeTrash");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(size);
        }
        if (availableSort?.includes("TypeTrash")) {
          const idx = availableSort.findIndex((x) => x === "TypeTrash");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(type);
        }
      } else {
        const availableSort = localStorage
          ?.getItem(`${TABLE_COLUMNS}=${userId}`)
          ?.split(",");

        const infoPanelColumnsSize = localStorage
          ?.getItem(`${COLUMNS_SIZE_INFO_PANEL}=${userId}`)
          ?.split(" ");

        if (availableSort?.includes("Author")) {
          const idx = availableSort.findIndex((x) => x === "Author");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(authorOption);
        }
        if (availableSort?.includes("Created")) {
          const idx = availableSort.findIndex((x) => x === "Created");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(creationDate);
        }
        if (availableSort?.includes("Modified")) {
          const idx = availableSort.findIndex((x) => x === "Modified");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(modifiedDate);
        }
        if (availableSort?.includes("Size")) {
          const idx = availableSort.findIndex((x) => x === "Size");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(size);
        }
        if (availableSort?.includes("Type")) {
          const idx = availableSort.findIndex((x) => x === "Type");
          const hide =
            infoPanelVisible &&
            infoPanelColumnsSize &&
            infoPanelColumnsSize[idx] === "0px";

          !hide && commonOptions.push(type);
        }
      }
    } else {
      if (isRooms) {
        commonOptions.push(roomType);
        commonOptions.push(tags);
        commonOptions.push(owner);
        commonOptions.push(modifiedDate);
      } else if (isTrash) {
        commonOptions.push(authorOption);
        commonOptions.push(creationDate);
        commonOptions.push(erasure);
        commonOptions.push(size);
        commonOptions.push(type);
      } else {
        commonOptions.push(authorOption);
        commonOptions.push(creationDate);
        commonOptions.push(modifiedDate);
        commonOptions.push(size);
        commonOptions.push(type);
      }
    }

    return commonOptions;
  }, [
    personal,
    isRooms,
    isAccountsPage,
    t,
    userId,
    infoPanelVisible,
    viewAs,
    isPersonalRoom,
    isTrash,
  ]);

  const removeSelectedItem = React.useCallback(
    ({ key, group }) => {
      if (isAccountsPage) {
        const newFilter = accountsFilter.clone();
        newFilter.page = 0;

        if (group === "filter-status") {
          newFilter.employeeStatus = null;
          newFilter.activationStatus = null;
        }

        if (group === "filter-type") {
          newFilter.role = null;
        }

        if (group === "filter-other") {
          newFilter.group = null;
        }

        if (group === "filter-account") {
          newFilter.payments = null;
        }

        if (group === "filter-login-type") {
          newFilter.accountLoginType = null;
        }

        setIsLoading(true);
        fetchPeople(newFilter, true).finally(() => setIsLoading(false));
      } else if (isRooms) {
        setIsLoading(true);

        const newFilter = roomsFilter.clone();

        if (group === FilterGroups.roomFilterProviderType) {
          newFilter.provider = null;
        }

        if (group === FilterGroups.roomFilterType) {
          newFilter.type = null;
        }

        if (group === FilterGroups.roomFilterSubject) {
          newFilter.subjectId = null;
          newFilter.excludeSubject = false;
          newFilter.filterSubject = null;
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
        if (group === FilterGroups.filterRoom) {
          newFilter.roomId = null;
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
      isAccountsPage,
      fetchFiles,
      fetchRooms,
      fetchPeople,
      setIsLoading,
      roomsFilter,
      filter,
      accountsFilter,
      selectedFolderId,
    ]
  );

  const onSortButtonClick = (isOpen) => {
    if (isMobileOnly) {
      setMainButtonMobileVisible(isOpen);
    }
  };

  const clearAll = () => {
    if (isAccountsPage) {
      setIsLoading(true);
      fetchPeople(null, true).finally(() => setIsLoading(false));
    } else if (isRooms) {
      setIsLoading(true);

      const newFilter = RoomsFilter.getDefault();

      if (isArchiveFolder) {
        newFilter.searchArea = RoomSearchArea.Archive;
      }

      fetchRooms(selectedFolderId, newFilter).finally(() =>
        setIsLoading(false)
      );
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
      viewAs={isAccountsPage ? accountsViewAs : viewAs}
      viewSelectorVisible={!isAccountsPage}
      onChangeViewAs={onChangeViewAs}
      getViewSettingsData={getViewSettingsData}
      onSearch={onSearch}
      onClearFilter={onClearFilter}
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
      filterTitle={t("Filter")}
      sortByTitle={t("Common:SortBy")}
      clearSearch={clearSearch}
      setClearSearch={setClearSearch}
      onSortButtonClick={onSortButtonClick}
    />
  );
};

export default inject(
  ({
    auth,
    filesStore,
    treeFoldersStore,
    selectedFolderStore,
    tagsStore,
    peopleStore,
  }) => {
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
      setMainButtonMobileVisible,
      thirdPartyStore,
      clearSearch,
      setClearSearch,
      isLoadedEmptyPage,
      isEmptyPage,
    } = filesStore;

    const { providers } = thirdPartyStore;

    const { fetchTags } = tagsStore;

    const { user } = auth.userStore;
    const { personal } = auth.settingsStore;
    const {
      isFavoritesFolder,
      isRecentFolder,
      isRoomsFolder,
      isArchiveFolder,
      isPersonalRoom,
      isTrashFolder: isTrash,
    } = treeFoldersStore;

    const isRooms = isRoomsFolder || isArchiveFolder;

    const { isVisible: infoPanelVisible } = auth.infoPanelStore;

    const {
      filterStore,
      usersStore,
      groupsStore,
      viewAs: accountsViewAs,
    } = peopleStore;

    const { groups } = groupsStore;

    const { getUsersList: fetchPeople } = usersStore;
    const { filter: accountsFilter } = filterStore;

    return {
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
      isTrash,
      isArchiveFolder,

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

      isLoadedEmptyPage,
      isEmptyPage,

      clearSearch,
      setClearSearch,

      setMainButtonMobileVisible,

      user,

      accountsViewAs,
      groups,
      fetchPeople,
      accountsFilter,
    };
  }
)(
  withLayoutSize(
    withTranslation([
      "Files",
      "Settings",
      "Common",
      "Translations",
      "InfoPanel",
      "People",
      "PeopleTranslations",
      "ConnectDialog",
      "SmartBanner",
    ])(withLoader(observer(SectionFilterContent))(<Loaders.Filter />))
  )
);
