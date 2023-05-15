import styled from "styled-components";
import Base from "../themes/base";
import { AddRoleButton } from "./svg";

const StyledFillingRoleSelector = styled.div``;

const StyledRow = styled.div`
  height: 48px;
  display: flex;
  align-items: center;
  gap: 8px;
`;

const StyledNumber = styled.div`
  font-weight: 600;
  font-size: 14px;
  line-height: 16px;
  color: #a3a9ae;
`;

const StyledAddRoleButton = styled(AddRoleButton)`
  width: 32px;
  height: 32px;

  path {
    fill: ${(props) => props.color};
  }

  rect {
    stroke: ${(props) => props.color};
  }
`;

const StyledRole = styled.div`
  font-weight: 600;
  font-size: 14px;
  line-height: 16px;
`;

StyledFillingRoleSelector.defaultProps = { theme: Base };

export {
  StyledFillingRoleSelector,
  StyledRow,
  StyledNumber,
  StyledAddRoleButton,
  StyledRole,
};
