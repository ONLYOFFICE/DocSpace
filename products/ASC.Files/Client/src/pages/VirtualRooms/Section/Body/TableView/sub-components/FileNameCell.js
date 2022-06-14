import React from "react";
import styled from "styled-components";

import TableCell from "@appserver/components/table-container/TableCell";
import RoomLogo from "@appserver/components/room-logo";
import Text from "@appserver/components/text";

const StyledText = styled(Text)`
  margin-left: 8px;
`;

const FileNameCell = ({ label, type, isPrivacy, theme }) => {
  return (
    <TableCell className="table-container_element-wrapper">
      <RoomLogo type={type} isPrivacy={isPrivacy} />
      <StyledText
        isBold={true}
        truncate={true}
        noSelect={true}
        color={theme.filesSection.tableView.fileName.linkColor}
      >
        {label}
      </StyledText>
    </TableCell>
  );
};

export default React.memo(FileNameCell);
