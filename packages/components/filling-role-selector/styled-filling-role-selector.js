import styled from "styled-components";
import Base from "../themes/base";
import { AddRoleButton, EveryoneRoleIcon } from "./svg";

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

const StyledEveryoneRoleIcon = styled(EveryoneRoleIcon)`
  width: 32px;
  height: 32px;
`;

const StyledRole = styled.div`
  font-weight: 600;
  font-size: 14px;
  line-height: 16px;
`;

const StyledEveryoneRoleContainer = styled.div`
  display: flex;
  flex-direction: column;

  .title {
    display: flex;
  }

  .comment {
    padding-left: 4px;
    color: rgba(170, 170, 170, 1);
  }

  .role-description {
    font-weight: 400;
    font-size: 10px;
    line-height: 14px;
    color: #657077;
  }
`;

const StyledTooltip = styled.div`
  background: #f8f7bf;
  border-radius: 6px;
  font-weight: 400;
  font-size: 12px;
  line-height: 16px;
  padding: 8px 12px;
`;

StyledFillingRoleSelector.defaultProps = { theme: Base };

export {
  StyledFillingRoleSelector,
  StyledRow,
  StyledNumber,
  StyledAddRoleButton,
  StyledEveryoneRoleIcon,
  StyledRole,
  StyledEveryoneRoleContainer,
  StyledTooltip,
};
