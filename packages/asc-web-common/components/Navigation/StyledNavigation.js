import styled, { css } from "styled-components";
import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, desktop, mobile } from "@appserver/components/utils/device";

const StyledContainer = styled.div`
  .header-container {
    margin-top: 14px;
    margin-bottom: 6px;
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
          margin-top: 17px;
          grid-template-columns: ${(props) =>
            props.isRootFolder
              ? "1fr auto"
              : props.canCreate
              ? "auto 1fr auto auto"
              : "auto 1fr auto"};
        }

        ${isMobile &&
        css`
          margin-top: 17px;
          grid-template-columns: ${(props) =>
            props.isRootFolder
              ? "1fr auto"
              : props.canCreate
              ? "auto 1fr auto auto"
              : "auto 1fr auto"};
        `}

        @media ${mobile} {
          margin-top: 12px;
          padding-bottom: 7px;
        }

        ${isMobileOnly &&
        css`
          margin-top: 12px;
          padding-bottom: 7px;
        `}
      `}

    .arrow-button {
      margin-right: 12px;
      min-width: 17px;

      align-items: center;
    }

    .add-button {
      margin-right: 10px;
      min-width: 17px;

      @media ${tablet} {
        display: none;
      }

      ${isMobile &&
      css`
        display: none;
      `}
    }

    .trash-button {
      min-width: 17px;
      margin-left: 6px;
    }

    .option-button {
      min-width: 17px;
    }
  }
`;

export default StyledContainer;
