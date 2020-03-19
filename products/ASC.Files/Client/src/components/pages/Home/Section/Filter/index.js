import React from "react";
import { connect } from "react-redux";
import { fetchFiles } from "../../../../../store/files/actions";
import find from "lodash/find";
import result from "lodash/result";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import { getFilterByLocation } from "../../../../../helpers/converters";
import { constants, FilterInput } from 'asc-web-common';
import store from "../../../../../store/store";

const { FilterType } = constants;

const getFilterType = filterValues => {
  const filterType = result(
    find(filterValues, value => {
      return value.group === "filter-filterType";
    }),
    "key"
  );

  return filterType ? +filterType : null;
};

const getAuthorType = filterValues => {
  const authorType = result(
    find(filterValues, value => {
      return value.group === "filter-author";
    }),
    "key"
  );

  return authorType ? authorType : null;
};

const getSelectedItem = (filterValues, type) => {
  const selectedItem = filterValues.find(item => item.key === type);
  return selectedItem || null;
};

const getSearchParams = filterValues => {
  const searchParams = result(
    find(filterValues, value => {
      return value.group === "filter-folders";
    }),
    "key"
  );

  return searchParams || 'true';
};

class SectionFilterContent extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      isReady: false,
      selectedItem: {
      }
    }

  }
  componentDidMount() {
    const { location, filter, onLoading, selectedFolderId } = this.props;

    const newFilter = getFilterByLocation(location);

    if (!newFilter || newFilter.equals(filter)) return;
    this.setState({ isReady: true })
    onLoading(true);
    fetchFiles(selectedFolderId, newFilter, store.dispatch)
      .finally(() => onLoading(false));
  }

  onFilter = data => {
    const { onLoading, filter, selectedFolderId } = this.props;

    const filterType = getFilterType(data.filterValues) || null;
    const search = data.inputValue || null;
    const sortBy = data.sortId;
    const sortOrder =
      data.sortDirection === "desc" ? "descending" : "ascending";
    const authorType = getAuthorType(data.filterValues);
    const withSubfolders = getSearchParams(data.filterValues);


    const selectedItem = authorType ? getSelectedItem(data.filterValues, authorType) : null;
    const selectedFilterItem = {};
    if (selectedItem) {
      selectedFilterItem.key = selectedItem.selectedItem.key;
      selectedFilterItem.label = selectedItem.selectedItem.label;
      selectedFilterItem.type = selectedItem.typeSelector;
    }

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.sortBy = sortBy;
    newFilter.sortOrder = sortOrder;
    newFilter.filterType = filterType;
    newFilter.search = search;
    newFilter.authorType = authorType;
    newFilter.withSubfolders = withSubfolders;
    newFilter.selectedItem = selectedFilterItem;

    onLoading(true);
    fetchFiles(selectedFolderId, newFilter, store.dispatch)
      .finally(() => onLoading(false));
  };

  getData = () => {
    const { t, settings, user, selectedItem } = this.props;
    const { usersCaption, groupsCaption } = settings.customNames;

    const options = [
      {
        key: "filter-filterType",
        group: "filter-filterType",
        label: t("Type"),
        isHeader: true
      },
      {
        key: FilterType.FoldersOnly.toString(),
        group: "filter-filterType",
        label: t("Folders")
      },
      {
        key: FilterType.DocumentsOnly.toString(),
        group: "filter-filterType",
        label: t("Documents")
      },
      {
        key: FilterType.PresentationsOnly.toString(),
        group: "filter-filterType",
        label: t("Presentations")
      },
      {
        key: FilterType.SpreadsheetsOnly.toString(),
        group: "filter-filterType",
        label: t("Spreadsheets")
      },
      {
        key: FilterType.ImagesOnly.toString(),
        group: "filter-filterType",
        label: t("Images")
      },
      {
        key: FilterType.MediaOnly.toString(),
        group: "filter-filterType",
        label: t("Media")
      },
      {
        key: FilterType.ArchiveOnly.toString(),
        group: "filter-filterType",
        label: t("Archives")
      },
      {
        key: FilterType.FilesOnly.toString(),
        group: "filter-filterType",
        label: t("AllFiles")
      }
    ];

    const filterOptions = [
      ...options,
      {
        key: "filter-author",
        group: "filter-author",
        label: t("Author"),
        isHeader: true,
      },
      {
        key: "user",
        group: "filter-author",
        label: usersCaption,
        isSelector: true,
        defaultOptionLabel: t("DefaultOptionLabel"),
        defaultSelectLabel: t("LblSelect"),
        groupsCaption,
        defaultOption: user,
        selectedItem
      },
      {
        key: "group",
        group: "filter-author",
        label: groupsCaption,
        defaultSelectLabel: t("LblSelect"),
        isSelector: true,
        selectedItem
      },
      {
        key: "filter-folders",
        group: "filter-folders",
        label: t("Folders"),
        isHeader: true
      },
      {
        key: "false",
        group: "filter-folders",
        label: t('NoSubfolders')
      },
    ];

    //console.log("getData (filterOptions)", filterOptions);

    return filterOptions;
  };

  getSortData = () => {
    const { t } = this.props;

    return [
      { key: "lastModifiedDate", label: t("ByLastModifiedDate"), default: true },
      { key: "creationDate", label: t("ByCreationDate"), default: true },
      { key: "title", label: t("ByTitle"), default: true },
      { key: "type", label: t("ByType"), default: true },
      { key: "size", label: t("BySize"), default: true },
      { key: "author", label: t("ByAuthor"), default: true },
    ];
  };

  getSelectedFilterData = () => {
    const { filter } = this.props;
    const selectedFilterData = {
      filterValues: [],
      sortDirection: filter.sortOrder === "ascending" ? "asc" : "desc",
      sortId: filter.sortBy
    };

    selectedFilterData.inputValue = filter.search;

    if (filter.filterType >= 0) {
      selectedFilterData.filterValues.push({
        key: `${filter.filterType}`,
        group: "filter-filterType"
      });
    }

    if (filter.authorType) {
      selectedFilterData.filterValues.push({
        key: `${filter.authorType}`,
        group: "filter-author"
      });
    }

    if (filter.withSubfolders === 'false') {
      selectedFilterData.filterValues.push({
        key: filter.withSubfolders,
        group: "filter-folders"
      });
    }

    // if (filter.group) {
    //   selectedFilterData.filterValues.push({
    //     key: filter.group,
    //     group: "filter-group"
    //   });
    // }

    return selectedFilterData;
  };

  needForUpdate = (currentProps, nextProps) => {
    if (currentProps.language !== nextProps.language) {
      return true;
    }
    return false;
  };


  render() {
    const selectedFilterData = this.getSelectedFilterData();
    const { t, i18n } = this.props;
    return (
      <FilterInput
        getFilterData={this.getData}
        getSortData={this.getSortData}
        selectedFilterData={selectedFilterData}
        onFilter={this.onFilter}
        directionAscLabel={t("DirectionAscLabel")}
        directionDescLabel={t("DirectionDescLabel")}
        placeholder={t("Search")}
        needForUpdate={this.needForUpdate}
        language={i18n.language}
        isReady={this.state.isReady}
      />
    );
  }
}

function mapStateToProps(state) {
  return {
    user: state.auth.user,
    folders: state.files.folders,
    filter: state.files.filter,
    settings: state.auth.settings,
    selectedFolderId: state.files.selectedFolder.id,
    selectedItem: state.files.filter.selectedItem
  };
}

export default connect(
  mapStateToProps,
  { fetchFiles }
)(withRouter(withTranslation()(SectionFilterContent)));
