import styled from "styled-components";

const StyledModuleTile = styled.div`
  width: auto;
  &:hover {
    .selectable {
      text-decoration: underline;
    }
  }

  .title-content {
    display: flex;
    flex-wrap: wrap;
    justify-content: center;

    .title-image-wrapper {
      padding: 0 16px;
      position: relative;

      @media (min-width: 768px) {
        flex: 0 0 auto;
        width: auto;
        max-width: 100%;
      }

      .title-image {
        border: none;
        height: 241px;
        width: 240px;
        cursor: pointer;
      }
    }

    .title-text-wrapper {
      padding: 0 16px;
      position: relative;
      width: 100%;

      @media (min-width: 768px) {
        flex: 0 0 auto;
        width: auto;
        max-width: 50%;
      }
    }
    .title-text {
      flex: 1 1 auto;
      padding: 1.25rem;

      .title-text-header {
        margin: 46px 0 11px 0;
        cursor: pointer;
      }
      .title-text-description {
        line-height: 20px;
      }
    }
  }

  .sub-title-content {
    text-align: center;
    flex: 1 1 auto;
    padding: 1.25rem;
    cursor: pointer;

    .sub-title-image {
      border: none;
      height: 100px;
      width: 100px;
    }
    .sub-title-text {
      margin: 16px 0 16px 0;
      text-align: center;
    }
  }
`;

export default StyledModuleTile;