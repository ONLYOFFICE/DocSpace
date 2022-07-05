import React from "react";
import find from "lodash/find";
import result from "lodash/result";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import { FilterType, RoomsType } from "@appserver/common/constants";
import Loaders from "@appserver/common/components/Loaders";
import FilterInput from "@appserver/common/components/FilterInput";
import { withLayoutSize } from "@appserver/common/utils";
import { isMobileOnly, isMobile } from "react-device-detect";
import { inject, observer } from "mobx-react";
import { getUser } from "@appserver/common/api/people";

const getTypes = (filterValues) => {
  const types = result(
    find(filterValues, (value) => {
      return value.group === "filter-types";
    }),
    "key"
  );

  return types ? +types : null;
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

const SectionFilterContent = ({
  t,
  user,
  filter,
  personal,
  isRecentFolder,
  isFavoritesFolder,
  sectionWidth,
  viewAs,
  createThumbnails,
  setViewAs,
  setIsLoading,
  selectedFolderId,
  fetchRooms,
  infoPanelVisible,
}) => {
  const [loading, setLoading] = React.useState(false);

  const filterColumnCount =
    window.innerWidth < 500
      ? { filterColumnCount: 3 }
      : { filterColumnCount: personal ? 2 : 3 };

  const onFilter = (data) => {
    const types = getTypes(data) || null;
    const authorType = !!getAuthorType(data)
      ? getAuthorType(data).includes("user_")
        ? getAuthorType(data)
        : `user_${getAuthorType(data)}`
      : null;
    const withSubfolders = getSearchParams(data);

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.types = types ? [types] : null;
    newFilter.authorType = authorType;
    newFilter.withSubfolders = withSubfolders;

    setIsLoading(true);

    fetchRooms(newFilter.searchArea, newFilter).finally(() =>
      setIsLoading(false)
    );
  };

  const onSearch = (data = "") => {
    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.filterValue = data;

    setIsLoading(true);

    fetchRooms(newFilter.searchArea, newFilter).finally(() =>
      setIsLoading(false)
    );
  };

  const onSort = (sortId, sortDirection) => {
    const sortBy = sortId;
    const sortOrder = sortDirection === "desc" ? "descending" : "ascending";

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.sortBy = sortBy;
    newFilter.sortOrder = sortOrder;

    setIsLoading(true);

    fetchRooms(newFilter.searchArea, newFilter).finally(() =>
      setIsLoading(false)
    );
  };

  const onChangeViewAs = (view) => {
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
  };

  const getFilterData = () => {
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

    const typeOptions = [
      {
        key: "filter-filterType",
        group: "filter-types",
        label: t("Common:Type"),
        isHeader: true,
      },
      {
        key: RoomsType.CustomRoom.toString(),
        group: "filter-types",
        label: "Custom room",
      },
      {
        key: RoomsType.FillingFormsRoom.toString(),
        group: "filter-types",
        label: "Filling form",
      },
      {
        key: RoomsType.EditingRoom.toString(),
        group: "filter-types",
        label: "Editing",
      },
      {
        key: RoomsType.ReviewRoom.toString(),
        group: "filter-types",
        label: "Review",
      },
      {
        key: RoomsType.ReadOnlyRoom.toString(),
        group: "filter-types",
        label: "View-only",
      },
    ];

    const filterOptions = [];

    filterOptions.push(...ownerOptions);

    filterOptions.push(...typeOptions);

    filterOptions.push({
      key: "filter-tags",
      group: "filter-tags",
      label: "Tags",
      isHeader: true,
      isLast: true,
    });

    return filterOptions;
  };

  const getSelectedFilterData = async () => {
    const selectedFilterData = {
      filterValues: [],
      sortDirection: filter.sortOrder === "ascending" ? "asc" : "desc",
      sortId: filter.sortBy,
    };

    selectedFilterData.inputValue = filter.filterValue;

    if (filter.types) {
      selectedFilterData.filterValues.push({
        key: `${filter.types}`,
        group: "filter-types",
      });
    }

    if (filter.authorType) {
      const user = await getUser(filter.authorType.replace("user_", ""));
      selectedFilterData.filterValues.push({
        key: `${filter.authorType}`,
        group: "filter-owner",
        label: user.displayName,
      });
    }

    return selectedFilterData;
  };

  const getViewSettingsData = () => {
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
      },
    ];

    return viewSettings;
  };

  const getSortData = () => {
    const commonOptions = [
      { key: "AZ", label: "Name", default: true },
      // { key: "Type", label: t("Common:Type"), default: true },
      // { key: "Tags", label: "Tags", default: true },
      { key: "Author", label: "Owner", default: true },
      { key: "DateAndTime", label: "Last modified", default: true },
    ];

    return commonOptions;
  };

  return (
    <FilterInput
      t={t}
      sectionWidth={sectionWidth}
      getFilterData={getFilterData}
      getSortData={getSortData}
      getViewSettingsData={getViewSettingsData}
      getSelectedFilterData={getSelectedFilterData}
      onFilter={onFilter}
      onSearch={onSearch}
      onSort={onSort}
      onChangeViewAs={onChangeViewAs}
      viewAs={viewAs}
      placeholder={t("Common:Search")}
      {...filterColumnCount}
      contextMenuHeader={t("Filter")}
      headerLabel={t("Translations:AddAuthor")}
      viewSelectorVisible={true}
      isFavoritesFolder={isFavoritesFolder}
      isRecentFolder={isRecentFolder}
      isRoomFolder={true}
      isLoading={loading}
    />
  );
};

export default inject(({ auth, filesStore, roomsStore }) => {
  const { setIsLoading, setViewAs, viewAs } = filesStore;

  const { fetchRooms, filter, rooms } = roomsStore;

  const { user } = auth.userStore;

  const { search, filterType, authorType } = filter;

  // const isFiltered =
  //   (!!files.length ||
  //     !!folders.length ||
  //     search ||
  //     filterType ||
  //     authorType) &&
  //   !(treeFoldersStore.isPrivacyFolder && isMobile);

  return {
    user,

    fetchRooms,
    filter,

    viewAs,
    setViewAs,
    setIsLoading,
  };
})(
  withRouter(
    withLayoutSize(
      withTranslation(["Home", "Common", "Translations"])(
        observer(SectionFilterContent)
      )
    )
  )
);
