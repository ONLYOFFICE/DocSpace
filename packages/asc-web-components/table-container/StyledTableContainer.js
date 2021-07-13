import styled from "styled-components";

const StyledTableContainer = styled.table`
  width: 100%;
  max-width: 100%;
  margin-top: -18px;
  border-collapse: collapse;

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

const StyledTableRow = styled.tr`
  height: 47px;
  max-height: 47px;
  border-bottom: 1px solid #eceef1;
`;

const StyledTableHeader = styled.thead``;
const StyledTableBody = styled.tbody``;
const StyledTableCell = styled.td``;

export {
  StyledTableContainer,
  StyledTableRow,
  StyledTableBody,
  StyledTableHeader,
  StyledTableCell,
};
