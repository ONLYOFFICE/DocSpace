import React from "react";
import styled from "styled-components";
import { withRouter } from "react-router";

import TableRow from "@docspace/components/table-container/TableRow";
import TableCell from "@docspace/components/table-container/TableCell";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import ComboBox from "@docspace/components/combobox";
import DropDownItem from "@docspace/components/drop-down-item";

import withContextOptions from "SRC_DIR/HOCs/withPeopleContextOptions";
import withContent from "SRC_DIR/HOCs/withPeopleContent";

import Badges from "../Badges";

const StyledPeopleRow = styled(TableRow)`
  .table-container_cell {
    height: 46px;
    max-height: 46px;
  }

  .table-container_row-checkbox-wrapper {
    padding-right: 0px;
    padding-left: 4px;
    min-width: 46px;

    .table-container_row-checkbox {
      margin-left: -4px;
      padding: 16px 0px 16px 12px;
    }
  }

  .link-with-dropdown-group {
    margin-right: 12px;
  }

  .table-cell_username {
    margin-right: 12px;
  }

  .table-container_row-context-menu-wrapper {
    padding-right: 0px;
  }

  .type-combobox {
    width: auto;

    .combo-button {
      padding-left: 0;

      .combo-button-label {
        color: ${(props) => props.sideInfoColor};
      }

      .combo-buttons_arrow-icon {
        margin-top: 7px;

        svg {
          path {
            fill: ${(props) => props.sideInfoColor};
          }
        }
      }
    }
  }

  .room-combobox {
    width: auto;

    .combo-button {
      padding-left: 0;

      .combo-button-label {
        color: ${(props) => props.sideInfoColor};
      }

      .combo-buttons_arrow-icon {
        display: none;
      }
    }
  }
`;

const fakeRooms = [
  {
    name: "Room 1",
    role: "Viewer",
  },
  {
    name: "Room 2",
    role: "Co-worker",
  },
];

const PeopleTableRow = (props) => {
  const {
    item,
    contextOptionsProps,
    element,
    checkedProps,
    onContentRowSelect,
    onEmailClick,
    onUserNameClick,
    isAdmin,
    theme,
  } = props;
  const { displayName, email, statusType, userName, position, role } = item;

  const isPending = statusType === "pending" || statusType === "disabled";

  const nameColor = isPending
    ? theme.peopleTableRow.pendingNameColor
    : theme.peopleTableRow.nameColor;
  const sideInfoColor = theme.peopleTableRow.sideInfoColor;

  const onChange = (e) => {
    onContentRowSelect && onContentRowSelect(e.target.checked, item);
  };

  const getTypesOptions = React.useCallback(() => {
    const options = [
      {
        key: "admin",
        title: "TODO: Administrator",
        label: "TODO: Administrator",
        action: "administrator",
      },
      {
        key: "manager",
        title: "TODO: Manager",
        label: "TODO: Manager",
        action: "manager",
      },
      {
        key: "user",
        title: "TODO: User",
        label: "TODO: User",
        action: "user",
      },
    ];

    return options;
  }, []);

  // TODO: update after backend update
  const onTypeChange = React.useCallback(({ action }) => {}, []);

  const getRoomsOptions = React.useCallback(() => {
    const options = [];

    fakeRooms.forEach((room) => {
      options.push(
        <DropDownItem key={room.name} noHover={true}>
          {room.name} &nbsp;
          <Text fontSize="13px" fontWeight={600} color={sideInfoColor} truncate>
            ({room.role})
          </Text>
        </DropDownItem>
      );
    });

    return <>{options.map((option) => option)}</>;
  }, []);

  return (
    <StyledPeopleRow
      key={item.id}
      sideInfoColor={sideInfoColor}
      {...contextOptionsProps}
    >
      <TableCell>
        <TableCell
          hasAccess={isAdmin}
          className="table-container_row-checkbox-wrapper"
          checked={checkedProps.checked}
        >
          <div className="table-container_element">{element}</div>
          <Checkbox
            className="table-container_row-checkbox"
            onChange={onChange}
            isChecked={checkedProps.checked}
          />
        </TableCell>

        <Link
          type="page"
          title={displayName}
          fontWeight="600"
          fontSize="15px"
          color={nameColor}
          isTextOverflow
          href={`/accounts/view/${userName}`}
          onClick={onUserNameClick}
          className="table-cell_username"
        >
          {displayName}
        </Link>
        <Badges statusType={statusType} />
      </TableCell>
      <TableCell>
        {role === "owner" ? (
          <Text
            type="page"
            title={position}
            fontSize="13px"
            fontWeight={600}
            color={sideInfoColor}
            truncate
          >
            TODO: Owner
          </Text>
        ) : (
          <ComboBox
            className="type-combobox"
            selectedOption={getTypesOptions().find(
              (option) => option.key === role
            )}
            options={getTypesOptions()}
            onSelect={onTypeChange}
            noBorder
            displaySelectedOption
            scaled
          />
        )}
      </TableCell>
      <TableCell>
        {isPending && statusType !== "disabled" ? (
          <Text
            type="page"
            title={position}
            fontSize="13px"
            fontWeight={600}
            color={sideInfoColor}
            truncate
          >
            —
          </Text>
        ) : role === "owner" ? (
          <Link
            type="action"
            title={email}
            fontSize="13px"
            fontWeight={600}
            color={sideInfoColor}
            isTextOverflow
            isHovered
          >
            {fakeRooms[0].name} ({fakeRooms[0].role})
          </Link>
        ) : (
          <ComboBox
            className="room-combobox"
            selectedOption={{ key: "length", label: `${fakeRooms.length}` }}
            options={[]}
            onSelect={onTypeChange}
            advancedOptions={getRoomsOptions()}
            noBorder
            displaySelectedOption
            scaled
          />
        )}
      </TableCell>
      <TableCell>
        <Link
          type="page"
          title={email}
          fontSize="13px"
          fontWeight={600}
          color={sideInfoColor}
          onClick={onEmailClick}
          isTextOverflow
        >
          {email}
        </Link>
      </TableCell>
    </StyledPeopleRow>
  );
};

export default withRouter(withContextOptions(withContent(PeopleTableRow)));
