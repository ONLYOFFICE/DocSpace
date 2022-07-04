import React from "react";
import styled from "styled-components";
import { ReactSVG } from "react-svg";

import TableCell from "@appserver/components/table-container/TableCell";
import RoomLogo from "@appserver/components/room-logo";
import Text from "@appserver/components/text";
import Checkbox from "@appserver/components/checkbox";
import Badge from "@appserver/components/badge";

const StyledText = styled(Text)`
  margin: 0 8px;
`;

const StyledPinIcon = styled(ReactSVG)`
  margin-right: 8px;

  width: 16px;
  height: 16px;
`;

const FileNameCell = ({
  label,
  type,
  isPrivacy,
  isChecked,
  theme,
  pinned,
  badgeLabel,
  onRoomSelect,
  onClickUnpinRoom,
  onBadgeClick,
}) => {
  return (
    <TableCell className="table-container_element-wrapper room-name_cell">
      <div className="room-name__logo-container">
        <RoomLogo
          className={"table-name_logo"}
          type={type}
          isPrivacy={isPrivacy}
          withCheckbox={true}
          isChecked={isChecked}
          isIndeterminate={false}
          onChange={onRoomSelect}
        />
      </div>

      <StyledText
        isBold={true}
        truncate={true}
        noSelect={true}
        color={theme.filesSection.tableView.fileName.linkColor}
      >
        {label}
      </StyledText>
      {pinned && (
        <StyledPinIcon
          className="room-name_pin-icon"
          onClick={onClickUnpinRoom}
          src="images/unpin.react.svg"
        />
      )}
      <Badge
        className="room-name_badge"
        label={badgeLabel}
        lineHeight={"12px"}
        fontSize={"9px"}
        onClick={onBadgeClick}
      />
    </TableCell>
  );
};

export default React.memo(FileNameCell);
