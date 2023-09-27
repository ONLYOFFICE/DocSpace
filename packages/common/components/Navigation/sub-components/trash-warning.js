import React from "react";
import styled, { css } from "styled-components";
import { tablet } from "@docspace/components/utils/device";

const StyledTrashWarning = styled.div`
  box-sizing: border-box;
  height: 32px;
  padding: 8px 12px;
  border-radius: 6px;

  display: flex;
  align-items: center;
  justify-content: ${({ theme }) =>
    theme.interfaceDirection === "rtl" ? `right` : `left`};

  font-weight: 400;
  font-size: 12px;
  line-height: 16px;

  color: ${({ theme }) => theme.section.header.trashErasureLabelText};
  background: ${({ theme }) =>
    theme.section.header.trashErasureLabelBackground};
`;

const TrashWarning = ({ title, isTabletView }) => {
  return (
    <StyledTrashWarning className="trash-warning" isTabletView={isTabletView}>
      {title}
    </StyledTrashWarning>
  );
};

export default TrashWarning;
