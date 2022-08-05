import styled from "styled-components";

const StyledCertificatesTable = styled.div`
  width: 100%;
  max-width: 600px;
  margin-bottom: 12px;

  .header {
    display: grid;
    align-items: center;
    grid-template-columns: repeat(3, 1fr);
    grid-auto-rows: 40px;

    border-bottom: 1px solid #eceef1;

    &-cell:nth-child(n + 2) {
      display: flex;
      align-items: center;

      border-left: 1px solid #d0d5da;
      height: 13px;
      padding-left: 8px;
    }
  }

  .row {
    width: 350px;
    display: flex;
    gap: 12px;
    align-items: center;
    padding: 11px 0 10px;
    border-bottom: 1px solid #eceef1;

    .column {
      display: flex;
      flex-direction: column;
      width: 100%;

      .column-row {
        display: flex;
      }
    }

    .context-btn {
      justify-self: flex-end;
    }
  }
`;

export default StyledCertificatesTable;
