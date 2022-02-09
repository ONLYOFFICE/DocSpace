import styled, { css } from "styled-components";
import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, desktop, mobile } from "@appserver/components/utils/device";

const StyledContainer = styled.div`
  .header-container {
    margin-top: 14px;
    position: relative;

    align-items: center;
    max-width: ${(props) => props.width}px;

    ${(props) =>
      props.title &&
      css`
        display: grid;
        grid-template-columns: ${(props) =>
          props.isRootFolder
            ? "auto auto 1fr"
            : props.canCreate
            ? "auto auto auto auto 1fr"
            : "auto auto auto 1fr"};

        @media ${tablet} {
          grid-template-columns: ${(props) =>
            props.isRootFolder
              ? "1fr auto"
              : props.canCreate
              ? "auto 1fr auto auto"
              : "auto 1fr auto"};
        }

        ${isMobile &&
        css`
          grid-template-columns: ${(props) =>
            props.isRootFolder
              ? "1fr auto"
              : props.canCreate
              ? "auto 1fr auto auto"
              : "auto 1fr auto"};
        `}
      `}

    .arrow-button {
      margin-right: 15px;
      min-width: 17px;

      align-items: center;

      @media ${tablet} {
        padding: 0 0 0 8px;
        margin-left: -8px;
        margin-right: 16px;
      }

      ${isMobile &&
      css`
        padding: 0 0 0 8px;
        margin-left: -8px;
        margin-right: 16px;
      `}
    }

    .add-button {
      ${(props) =>
        !props.isRootFolder ? `margin-left: 16px` : `margin-left: 6px`};

      @media ${tablet} {
        display: none;
      }

      ${isMobile &&
      css`
        display: none;
      `}
    }

    .trash-button {
      margin-left: 6px;
    }

    .option-button {
      @media (min-width: 1024px) {
        margin-left: 8px;
      }

      @media ${tablet} {
        & > div:first-child {
          padding: 0px 8px 8px 8px;
          margin-right: -8px;
        }
      }

      ${isMobile &&
      css`
        & > div:first-child {
          padding: 0px 8px 8px 8px;
          margin-right: -8px;
        }
      `}
    }
  }
`;

export default StyledContainer;
