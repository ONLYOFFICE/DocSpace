import styled, { css } from "styled-components";

import Base from "@docspace/components/themes/base";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */

/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledSelector = styled.div`
  display: grid;

  height: 100%;
  grid-template-columns: 1fr;
  ${(props) =>
    props.isMultiSelect && props.hasSelected
      ? css`
          grid-template-rows: 53px 1fr 69px;
          grid-template-areas: "header" "column-options" "footer";
        `
      : css`
          grid-template-rows: 53px 1fr;
          grid-template-areas: "header" "column-options";
        `}

  .header {
    grid-area: header;

    height: 53px;
    min-height: 53px;

    padding: 0 16px;
    margin: 0;

    box-sizing: border-box;

    display: flex;
    align-items: center;
    justify-content: start;

    .arrow-button {
      margin-right: 12px;
    }

    svg {
      cursor: pointer;
    }
  }

  .column-options {
    grid-area: column-options;
    box-sizing: border-box;

    display: grid;

    padding: 0;
    grid-row-gap: 2px;

    overflow: hidden;

    grid-template-columns: 1fr;
    grid-template-rows: 30px 1fr;
    grid-template-areas: "header-options" "body-options";

    .header-options {
      grid-area: header-options;

      padding: 0 16px;
      margin-right: 0px !important;

      display: grid;
      grid-template-columns: 1fr;
      grid-template-rows: 30px;

      grid-template-areas: "options_searcher";

      .options_searcher {
        grid-area: options_searcher;
      }

      .options_searcher {
        div:first-child {
          :hover {
            border-color: ${(props) =>
              props.theme.advancedSelector.searcher.hoverBorderColor};
          }

          :focus,
          :focus-within {
            border-color: ${(props) =>
              props.theme.advancedSelector.searcher.focusBorderColor};
          }

          & > input::placeholder {
            color: ${(props) =>
              props.theme.advancedSelector.searcher.placeholderColor};
          }
        }
      }
    }

    .body-options {
      grid-area: body-options;
      margin-top: 8px;

      .options-list {
        div:nth-child(3) {
          right: 10px !important;
        }
      }
      .option-loader {
        width: 100%;
        height: 100%;
        margin-top: 16px;
      }
      .row-option {
        box-sizing: border-box;
        height: 48px;
        cursor: pointer;

        display: flex;
        align-items: center;
        justify-content: space-between;

        padding: 0 16px;

        &:hover {
          background-color: ${(props) =>
            props.theme.advancedSelector.hoverBackgroundColor};
        }

        .option-info {
          width: calc(100% - 32px);
          display: flex;
          align-items: center;
          justify-content: start;
        }

        .option-avatar {
          margin-right: 12px;
          min-width: 32px;
          max-width: 32px;
        }

        .option-text {
          max-width: 100%;
          line-height: 16px;
        }

        .option-text__group {
          width: auto;
          border-bottom: ${(props) =>
            props.theme.toggleContent.hoverBorderBottom};
        }

        .option-text__header {
          font-weight: 600;
        }

        .option-checkbox {
          margin-left: 8px;
          margin-right: 8px;
          min-width: 16px;
          max-width: 16px;
        }
      }
      .option-separator {
        height: 1px;
        background: ${(props) =>
          props.theme.advancedSelector.selectedBackgroundColor};
        margin: 8px 16px;
      }

      .row-header {
        cursor: auto;

        .option-checkbox {
          margin-right: 0 !important;
        }

        :hover {
          background: none;
        }
      }
    }
  }

  .footer {
    grid-area: footer;
  }
`;

StyledSelector.defaultProps = { theme: Base };

export default StyledSelector;
