import styled from "styled-components";
import SettingsIcon from "./svg/settings.react.svg";

const StyledTableContainer = styled.div`
  width: 100%;
  max-width: 100%;
  margin-top: -18px;

  display: grid;
  grid-template-columns:
    32px
    minmax(180px, 2fr)
    minmax(150px, 1fr)
    minmax(150px, 1fr)
    minmax(150px, 1fr)
    80px
    24px;

  .table-column {
    user-select: none;
    position: relative;
    min-width: 10%;
  }

  .resize-handle {
    display: block;
    cursor: ew-resize;
    height: 10px;
    margin: 14px 4px 0 auto;
    z-index: 1;
    border-right: 2px solid #d0d5da;
  }

  .header-container {
    height: 38px;
    display: flex;
    align-items: center;
  }

  .content-container {
    overflow: hidden;
  }

  .children-wrap {
    display: flex;
    flex-direction: column;
  }

  .table-cell {
    height: 47px;
    border-bottom: 1px solid #eceef1;
  }
`;

const StyledTableRow = styled.div`
  display: contents;
`;

const StyledTableHeader = styled.div`
  display: grid;
  position: fixed;
  background: #fff;
  z-index: 1;

  .table-container_header-cell {
    border-bottom: 1px solid #eceef1;
  }
`;
const StyledTableBody = styled.div`
  display: contents;
`;
const StyledTableCell = styled.div`
  height: 40px;
  max-height: 40px;
  border-bottom: 1px solid #eceef1;

  display: flex;
  align-items: center;

  .react-svg-icon svg {
    margin-top: 2px;
  }
`;

const StyledSettingsIcon = styled(SettingsIcon)`
  margin-top: 12px;
`;

export {
  StyledTableContainer,
  StyledTableRow,
  StyledTableBody,
  StyledTableHeader,
  StyledTableCell,
  StyledSettingsIcon,
};
