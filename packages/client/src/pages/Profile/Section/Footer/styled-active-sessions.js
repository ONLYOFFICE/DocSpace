import styled from "styled-components";
import {
  hugeMobile,
  smallTablet,
  tablet,
} from "@docspace/components/utils/device";

export const StyledFooter = styled.div`
  .session-logout {
    font-size: 13px;
    font-weight: 600;
  }
  .icon-button {
    margin-left: 4px;
  }
`;

export const Table = styled.table`
  width: 100%;
  border-collapse: collapse;
  margin-top: 4px;
`;

export const TableHead = styled.thead`
  font-size: 12px;
`;

export const TableRow = styled.tr`
  display: table-row;
`;

export const TableHeaderCell = styled.th`
  border-top: 1px solid ${(props) => props.theme.activeSessions.borderColor};
  border-bottom: 1px solid ${(props) => props.theme.activeSessions.borderColor};
  text-align: left;
  font-weight: 600;
  padding: 14px 0;
  color: #a3a9ae;
  position: relative;
  border-top: 0;

  :not(:first-child):before {
    content: "";
    position: absolute;
    top: 17px;
    left: -8px;
    width: 1px;
    height: 10px;
    background: #d0d5da;
  }
`;

export const TableBody = styled.tbody`
  font-size: 11px;
`;

export const TableDataCell = styled.td`
  border-top: 1px solid ${(props) => props.theme.activeSessions.borderColor};
  border-bottom: 1px solid ${(props) => props.theme.activeSessions.borderColor};
  text-align: left;
  font-weight: 600;
  padding: 14px 0;
  color: #a3a9ae;

  :first-child {
    font-size: 13px;
    color: ${(props) => props.theme.activeSessions.color};
    span {
      color: #a3a9ae;
      margin-left: 5px;
    }
  }

  :last-child {
    text-align: center;
  }
  .remove-icon {
    svg {
      cursor: pointer;
      width: 20px;
      height: 20px;
      path {
        fill: #a3a9ae;
      }
      @media (max-width: 575px) {
        width: 16px;
        height: 16px;
      }
    }
  }
`;
