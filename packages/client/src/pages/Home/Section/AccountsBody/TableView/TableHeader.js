import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import TableHeader from "@docspace/components/table-container/TableHeader";

const TABLE_VERSION = "3";
const TABLE_COLUMNS = `peopleTableColumns_ver-${TABLE_VERSION}`;

class PeopleTableHeader extends React.Component {
  constructor(props) {
    super(props);

    const { t } = props;

    const defaultColumns = [
      {
        key: "Name",
        title: t("Common:Name"),
        resizable: true,
        enable: true,
        default: true,
        sortBy: "AZ",
        minWidth: 210,
        onClick: this.onFilter,
        onIconClick: this.onIconClick,
      },
      {
        key: "Type",
        title: t("Common:Type"),
        enable: true,
        sortBy: "type",
        resizable: true,
        onChange: this.onColumnChange,
        onClick: this.onFilter,
      },
      // {
      //   key: "Room",
      //   title: t("Common:Room"),
      //   enable: true,
      //   resizable: true,
      //   onChange: this.onColumnChange,
      // },
      {
        key: "Mail",
        title: t("Common:Email"),
        enable: true,
        resizable: true,
        sortBy: "email",
        onChange: this.onColumnChange,
        onClick: this.onFilter,
      },
    ];

    const columns = this.getColumns(defaultColumns);

    this.state = { columns };
  }

  getColumns = (defaultColumns) => {
    const storageColumns = localStorage.getItem(
      `${TABLE_COLUMNS}=${this.props.userId}`
    );
    const columns = [];

    if (storageColumns) {
      const splitColumns = storageColumns.split(",");

      for (let col of defaultColumns) {
        const column = splitColumns.find((key) => key === col.key);
        column ? (col.enable = true) : (col.enable = false);

        columns.push(col);
      }
      return columns;
    } else {
      return defaultColumns;
    }
  };

  onColumnChange = (key, e) => {
    const { columns } = this.state;
    const columnIndex = columns.findIndex((c) => c.key === key);

    if (columnIndex === -1) return;

    columns[columnIndex].enable = !columns[columnIndex].enable;
    this.setState({ columns });

    const tableColumns = columns.map((c) => c.enable && c.key);
    localStorage.setItem(`${TABLE_COLUMNS}=${this.props.userId}`, tableColumns);
  };

  onFilter = (sortBy) => {
    const { filter, setIsLoading, navigate, location } = this.props;
    const newFilter = filter.clone();

    if (newFilter.sortBy === sortBy && sortBy !== "AZ") {
      newFilter.sortOrder =
        newFilter.sortOrder === "ascending" ? "descending" : "ascending";
    } else {
      newFilter.sortBy = sortBy;

      if (sortBy === "AZ") {
        if (
          newFilter.sortBy !== "lastname" &&
          newFilter.sortBy !== "firstname"
        ) {
          newFilter.sortBy = "firstname";
        } else if (newFilter.sortBy === "lastname") {
          newFilter.sortBy = "firstname";
        } else {
          newFilter.sortBy = "lastname";
        }
        newFilter.sortOrder =
          newFilter.sortOrder === "ascending" ? "descending" : "ascending";
      }
    }

    setIsLoading(true);

    navigate(`${location.pathname}?${newFilter.toUrlParams()}`);
  };

  onIconClick = () => {
    const { filter, setIsLoading, navigate, location } = this.props;
    const newFilter = filter.clone();

    newFilter.sortOrder =
      newFilter.sortOrder === "ascending" ? "descending" : "ascending";

    setIsLoading(true);

    navigate(`${location.pathname}?${newFilter.toUrlParams()}`);
  };

  render() {
    const { columns } = this.state;
    const {
      containerRef,
      filter,
      sectionWidth,
      infoPanelVisible,
      columnStorageName,
      columnInfoPanelStorageName,
      withPaging,
      setHideColumns,
    } = this.props;
    const { sortOrder } = filter;

    const sortBy =
      filter.sortBy === "firstname" || filter.sortBy === "lastname"
        ? "AZ"
        : filter.sortBy;

    return (
      <TableHeader
        checkboxSize="48px"
        sorted={sortOrder === "descending"}
        sortBy={sortBy}
        containerRef={containerRef}
        columns={columns}
        columnStorageName={columnStorageName}
        columnInfoPanelStorageName={columnInfoPanelStorageName}
        sectionWidth={sectionWidth}
        checkboxMargin="12px"
        infoPanelVisible={infoPanelVisible}
        useReactWindow={!withPaging}
        setHideColumns={setHideColumns}
      />
    );
  }
}

export default inject(({ auth, peopleStore, clientLoadingStore }) => {
  const { filterStore } = peopleStore;

  const { filter } = filterStore;

  const { isVisible: infoPanelVisible } = auth.infoPanelStore;
  const { withPaging } = auth.settingsStore;

  return {
    filter,

    setIsLoading: clientLoadingStore.setIsSectionBodyLoading,
    userId: auth.userStore.user?.id,
    infoPanelVisible,
    withPaging,
  };
})(
  withTranslation(["People", "Common", "PeopleTranslations"])(
    observer(PeopleTableHeader)
  )
);
