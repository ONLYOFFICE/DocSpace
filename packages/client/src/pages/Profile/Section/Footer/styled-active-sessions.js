import styled from "styled-components";
import { hugeMobile } from "@docspace/components/utils/device";

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

  @media ${hugeMobile} {
    .session-browser {
      position: relative;
      top: 4px;
      max-width: 150px;
      display: inline-block;
      margin-left: 0 !important;
      overflow: hidden;
      white-space: nowrap;
      text-overflow: ellipsis;
      span {
        width: 100%;
      }
    }
  }

  :first-child {
    font-size: 13px;
    color: ${(props) => props.theme.activeSessions.color};
    span {
      color: #a3a9ae;
      margin-left: 5px;
    }
  }

  .session-date {
    position: relative;
    margin-left: 0 !important;
    margin-right: 8px;
    :after {
      content: "";
      position: absolute;
      top: 4px;
      right: -8px;
      width: 1px;
      height: 12px;
      background: #d0d5da;
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
    }
  }
`;
