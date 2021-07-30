import React from "react";
import TableHeader from "@appserver/components/table-container/TableHeader";
import TableGroupMenu from "@appserver/components/table-container/TableGroupMenu";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import DropDownItem from "@appserver/components/drop-down-item";
import styled from "styled-components";
import { Consumer } from "@appserver/components/utils/context";

const TABLE_COLUMNS = "peopleTableColumns";

const StyledTableHeader = styled(TableHeader)`
  .table-container_header-checkbox {
    margin-left: 12px;
  }
`;

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
        minWidth: 180,
        onClick: this.onFilter,
        onIconClick: this.onIconClick,
      },
      {
        key: "Department",
        title: t("Common:Department"),
        enable: true,
        resizable: true,
        onChange: this.onColumnChange,
      },
      {
        key: "Role",
        title: t("Common:Role"),
        enable: true,
        resizable: true,
        onChange: this.onColumnChange,
      },
      {
        key: "Phone",
        title: t("Common:Phone"),
        enable: true,
        resizable: true,
        onChange: this.onColumnChange,
      },
      {
        key: "Mail",
        title: t("Common:Mail"),
        enable: true,
        resizable: true,
        onChange: this.onColumnChange,
      },
    ];

    const columns = this.getColumns(defaultColumns);

    this.state = { columns };
  }

  getColumns = (defaultColumns) => {
    const storageColumns = localStorage.getItem(TABLE_COLUMNS);
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
    localStorage.setItem(TABLE_COLUMNS, tableColumns);
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

  onChange = (checked) => {
    this.props.setSelected(checked ? "all" : "none");
  };

  onSelect = (e) => {
    const key = e.currentTarget.dataset.key;
    this.props.setSelected(key);
  };

  render() {
    const { columns } = this.state;
    const {
      t,
      containerRef,
      isHeaderVisible,
      isHeaderChecked,
      isHeaderIndeterminate,
      getHeaderMenu,
      setSelected,
      filter,
      sectionWidth,
    } = this.props;
    const { sortOrder } = filter;

    const checkboxOptions = (
      <>
        <DropDownItem
          label={t("Common:Active")}
          data-key="active"
          onClick={this.onSelect}
        />
        <DropDownItem
          label={t("Translations:DisabledEmployeeStatus")}
          data-key="disabled"
          onClick={this.onSelect}
        />
        <DropDownItem
          label={t("LblInvited")}
          data-key="invited"
          onClick={this.onSelect}
        />
      </>
    );

    return isHeaderVisible ? (
      <TableGroupMenu
        checkboxOptions={checkboxOptions}
        containerRef={containerRef}
        onSelect={this.onSelect}
        onChange={this.onChange}
        isChecked={isHeaderChecked}
        isIndeterminate={isHeaderIndeterminate}
        headerMenu={getHeaderMenu(t)}
      />
    ) : (
      <StyledTableHeader
        checkboxSize="48px"
        sorted={sortOrder === "descending"}
        setSelected={setSelected}
        containerRef={containerRef}
        columns={columns}
        columnStorageName="peopleColumnsSize"
        sectionWidth={sectionWidth}
      />
    );
  }
}

export default inject(({ peopleStore }) => {
  const {
    selectionStore,
    headerMenuStore,
    filterStore,
    usersStore,
    loadingStore,
    getHeaderMenu,
  } = peopleStore;

  const { setSelected } = selectionStore;
  const {
    isHeaderVisible,
    isHeaderChecked,
    isHeaderIndeterminate,
  } = headerMenuStore;
  const { filter } = filterStore;
  const { getUsersList } = usersStore;
  const { setIsLoading } = loadingStore;

  return {
    setSelected,
    isHeaderVisible,
    filter,
    fetchPeople: getUsersList,
    setIsLoading,
    getHeaderMenu,
    isHeaderChecked,
    isHeaderIndeterminate,
  };
})(
  withTranslation(["Home", "Common", "Translations"])(
    observer(PeopleTableHeader)
  )
);
