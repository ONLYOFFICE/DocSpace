import styled from "styled-components";

const StyledTableContainer = styled.div`
  width: 100%;
  display: flex;

  .table-column {
    user-select: none;
    position: relative;
    min-width: 10%;
  }

  .resize-handle {
    display: block;
    position: absolute;
    cursor: ew-resize;
    height: 10px;
    width: 7px;
    right: 4px;
    top: 4px;
    z-index: 1;
    border-right: 2px solid transparent;
    border-color: #d0d5da;
  }

  .header-container {
    border-bottom: 2px solid grey;
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

const StyledTableCell = styled.div`
  height: 47px;
  max-height: 47px;
  display: flex;
  align-items: center;
  border-bottom: 1px solid #eceef1;
`;

export { StyledTableContainer, StyledTableCell };
