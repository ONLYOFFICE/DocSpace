import styled from "styled-components";

const StyledModuleTile = styled.div`
  width: auto;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  user-select: none;

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

      @media (min-width: 500px) {
        width: auto;
        max-width: 50%;
      }

      a {
        text-decoration: none;
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
    cursor: pointer;

    .sub-title-image {
      border: none;
      height: 64px;
      width: 64px;
    }
    .sub-title-text {
      text-align: center;
      font-weight: 600;
    }

    @media (max-width: 500px) {
      .sub-title-image {
        height: 48px;
        width: 48px;
      }

      .sub-title-text {
        font-size: 14px;
      }
    }

    a {
      text-decoration: none;
    }
  }
`;

export default StyledModuleTile;
