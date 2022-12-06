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
        active: true,
        minWidth: 210,
        onClick: this.onFilter,
        onIconClick: this.onIconClick,
      },
      {
        key: "Type",
        title: t("Common:Type"),
        enable: true,
        resizable: true,
        onChange: this.onColumnChange,
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
        onChange: this.onColumnChange,
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

  onFilter = () => {
    const { filter, setIsLoading, fetchPeople } = this.props;
    const newFilter = filter.clone();

    if (newFilter.sortBy === "lastname") {
      newFilter.sortBy = "firstname";
    } else {
      newFilter.sortBy = "lastname";
    }

    setIsLoading(true);
    fetchPeople(newFilter).finally(() => setIsLoading(false));
  };

  onIconClick = () => {
    const { filter, setIsLoading, fetchPeople } = this.props;
    const newFilter = filter.clone();

    if (newFilter.sortOrder === "ascending") {
      newFilter.sortOrder = "descending";
    } else {
      newFilter.sortOrder = "ascending";
    }

    setIsLoading(true);
    fetchPeople(newFilter).finally(() => setIsLoading(false));
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
    } = this.props;
    const { sortOrder } = filter;

    return (
      <TableHeader
        checkboxSize="48px"
        sorted={sortOrder === "descending"}
        containerRef={containerRef}
        columns={columns}
        columnStorageName={columnStorageName}
        columnInfoPanelStorageName={columnInfoPanelStorageName}
        sectionWidth={sectionWidth}
        checkboxMargin="12px"
        infoPanelVisible={infoPanelVisible}
        useReactWindow={!withPaging}
      />
    );
  }
}

export default inject(({ auth, peopleStore }) => {
  const { filterStore, usersStore, loadingStore } = peopleStore;

  const { filter } = filterStore;
  const { getUsersList } = usersStore;
  const { setIsLoading } = loadingStore;

  const { isVisible: infoPanelVisible } = auth.infoPanelStore;
  const { withPaging } = auth.settingsStore;

  return {
    filter,
    fetchPeople: getUsersList,
    setIsLoading,
    userId: auth.userStore.user.id,
    infoPanelVisible,
    withPaging,
  };
})(
  withTranslation(["People", "Common", "PeopleTranslations"])(
    observer(PeopleTableHeader)
  )
);
