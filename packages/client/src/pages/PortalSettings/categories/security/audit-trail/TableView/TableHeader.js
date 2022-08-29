import React from "react";
import TableHeader from "@docspace/components/table-container/TableHeader";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

const TABLE_VERSION = "4";
const TABLE_COLUMNS = `auditTableColumns_ver-${TABLE_VERSION}`;
const COLUMNS_SIZE = `auditColumnsSize_ver-${TABLE_VERSION}`;

class PeopleTableHeader extends React.Component {
  constructor(props) {
    super(props);

    const { t } = props;

    const defaultColumns = [
      {
        key: "User",
        title: t("Common:User"),
        resizable: true,
        enable: true,
        default: true,
        sortBy: "AZ",
        active: true,
        minWidth: 180,
      },

      {
        key: "Date",
        title: t("Common:Date"),
        enable: true,
        resizable: true,
        onChange: this.onColumnChange,
      },
      {
        key: "Room",
        title: t("Common:Room"),
        enable: true,
        resizable: true,
        onChange: this.onColumnChange,
      },
      {
        key: "Action",
        title: t("Common:Action"),
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
    const { containerRef, sectionWidth, userId } = this.props;

    return (
      <TableHeader
        checkboxSize="48px"
        containerRef={containerRef}
        columns={columns}
        columnStorageName={`${COLUMNS_SIZE}=${userId}`}
        sectionWidth={sectionWidth}
        checkboxMargin="12px"
      />
    );
  }
}

export default inject(({ auth }) => {
  return {
    userId: auth.userStore.user.id,
  };
})(
  withTranslation(["Home", "Common", "Translations"])(
    observer(PeopleTableHeader)
  )
);
