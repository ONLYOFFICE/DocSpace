import React from "react";

import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import find from "lodash/find";
import result from "lodash/result";

import { getUser } from "@appserver/common/api/people";
import { RoomsType } from "@appserver/common/constants";
import Loaders from "@appserver/common/components/Loaders";
import FilterInput from "@appserver/common/components/FilterInput";
import { withLayoutSize } from "@appserver/common/utils";

import withLoader from "../../../../HOCs/withLoader";

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
  sectionWidth,

  userId,
  infoPanelVisible,

  filter,
  sortRooms,
  filterRooms,
  searchRooms,
  tags,
  fetchTags,

  viewAs,
  setViewAs,
  setIsLoading,
}) => {
  const onFilter = React.useCallback(
    (data) => {
      const types = getTypes(data) || null;

      const owner = getOwner(data) || null;

      const subjectId = owner === "me" || owner === "other" ? userId : owner;

      const tags = getTags(data) || null;

      setIsLoading(true);

      filterRooms(types, subjectId, tags).finally(() => {
        setIsLoading(false);
      });
    },
    [filterRooms, setIsLoading]
  );

  const onSearch = React.useCallback(
    (data = "") => {
      setIsLoading(true);

      searchRooms(data).finally(() => {
        setIsLoading(false);
      });
    },
    [searchRooms, setIsLoading]
  );

  const onSort = React.useCallback(
    (sortId, sortDirection) => {
      const sortBy = sortId;
      const sortOrder = sortDirection === "desc" ? "descending" : "ascending";

      setIsLoading(true);

      sortRooms(sortBy, sortOrder).finally(() => {
        setIsLoading(false);
      });
    },
    [sortRooms, setIsLoading]
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
    return filter.filterValue;
  }, [filter.filterValue]);

  const getSelectedSortData = React.useCallback(() => {
    return {
      sortDirection: filter.sortOrder === "ascending" ? "asc" : "desc",
      sortId: filter.sortBy,
    };
  }, [filter.sortOrder, filter.sortBy]);

  const getSelectedFilterData = React.useCallback(async () => {
    const filterValues = [];

    if (filter.types) {
      const key =
        typeof filter.types === "object" ? filter.types[0] : filter.types; //Remove it if filter types will be multi select

      filterValues.push({
        key: key,
        group: "filter-types",
      });
    }

    // TODO: add logic to other key
    if (filter.subjectId) {
      const isMe = userId === filter.subjectId;
      let label = null;

      if (!isMe) {
        const user = await getUser(filter.subjectId);

        label = user.displayName;
      }

      filterValues.push({
        key: isMe ? "me" : filter.subjectId,
        group: "filter-owner",
        label: label,
      });
    }

    if (filter.tags) {
      filterValues.push({
        key: filter.tags,
        group: "filter-tags",
      });
    }

    return filterValues;
  }, [filter.types, filter.subjectId, filter.tags, userId]);

  const getFilterData = React.useCallback(async () => {
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
        key: RoomsType.CustomRoom,
        group: "filter-types",
        label: "Custom room",
        isMultiSelect: false,
      },
      {
        key: RoomsType.FillingFormsRoom,
        group: "filter-types",
        label: "Filling form",
        isMultiSelect: false,
      },
      {
        key: RoomsType.EditingRoom,
        group: "filter-types",
        label: "Editing",
        isMultiSelect: false,
      },
      {
        key: RoomsType.ReviewRoom,
        group: "filter-types",
        label: "Review",
        isMultiSelect: false,
      },
      {
        key: RoomsType.ReadOnlyRoom,
        group: "filter-types",
        label: "View-only",
        isMultiSelect: false,
      },
    ];

    const filterOptions = [];

    filterOptions.push(...ownerOptions);

    filterOptions.push(...typeOptions);

    // const tags = await fetchTags();

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

    return filterOptions;
  }, [tags]);

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
      },
    ];

    return viewSettings;
  }, []);

  // TODO: Remove comments after backend fix
  const getSortData = React.useCallback(() => {
    const commonOptions = [
      { key: "AZ", label: "Name", default: true },
      // { key: "Type", label: t("Common:Type"), default: true },
      // { key: "Tags", label: "Tags", default: true },
      { key: "Author", label: "Owner", default: true },
      { key: "DateAndTime", label: "Last modified", default: true },
    ];

    return commonOptions;
  }, []);

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
      selectorLabel={t("Translations:AddAuthor")}
    />
  );
};

export default inject(({ auth, filesStore, roomsStore }) => {
  const { setIsLoading, setViewAs, viewAs } = filesStore;

  const {
    tags,
    fetchTags,
    filter,
    sortRooms,
    filterRooms,
    searchRooms,
  } = roomsStore;

  const { user } = auth.userStore;

  const { isVisible: infoPanelVisible } = auth.infoPanelStore;

  return {
    userId: user.id,
    infoPanelVisible,

    tags,
    fetchTags,
    filter,
    sortRooms,
    filterRooms,
    searchRooms,

    viewAs,
    setViewAs,
    setIsLoading,
  };
})(
  withRouter(
    withLayoutSize(
      withTranslation(["Home", "Common", "Translations"])(
        withLoader(observer(SectionFilterContent))(<Loaders.Filter />)
      )
    )
  )
);
