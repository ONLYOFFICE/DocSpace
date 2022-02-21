import styled from "styled-components";

const StyledCertificatesTable = styled.div`
  width: 100%;
  max-width: 600px;
  margin-top: -13px;
  margin-bottom: 16px;

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
    display: grid;
    align-items: center;
    grid-template-columns: repeat(3, 1fr);
    grid-auto-rows: 50px;

    border-bottom: 1px solid #eceef1;

    .column:nth-of-type(n + 2) {
      margin-left: 8px;
    }

    .name {
      display: flex;
      align-items: center;

      p {
        margin-left: 12px;
      }
    }

    .status {
      display: flex;
      justify-content: space-between;
    }
  }
`;

export default StyledCertificatesTable;
