import React from "react";
import styled, { css } from "styled-components";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
  displayType,
  options,
  groups,
  isMultiSelect,
  allowGroupSelection,
  hasSelected,
  ...props
}) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const dropdownStyles = css`
  grid-auto-rows: max-content;
  grid-template-areas: "column-options column-groups" "footer footer";

  .column-groups {
    box-sizing: border-box;
    grid-area: column-groups;

    ${props =>
      props.groups && props.groups.length > 0
        ? css`
            border-left: 1px solid #eceef1;
          `
        : ""}

    display: grid;
    /* background-color: gold; */
    padding: 0 16px 0 16px;
    grid-row-gap: 2px;

    grid-template-columns: 1fr;
    grid-template-rows: 30px 1fr;
    grid-template-areas: "header-groups" "body-groups";

    .header-groups {
      grid-area: header-groups;
      /* background-color: white; */
    }

    .body-groups {
      grid-area: body-groups;
      margin-left: -8px;
      /* background-color: white; */

      .row-group {
        box-sizing: border-box;
        height: 32px;
        cursor: pointer;
        padding-top: 8px;
        padding-left: 8px;

        .group_checkbox {
          display: inline-block;
        }

        &:hover {
          background-color: #f8f9f9;
          border-radius: 3px;
        }
      }

      .row-group.selected {
        background-color: #eceef1;
        border-radius: 3px;
      }
    }
  }
`;

const asideStyles = css`
  height: 100%;
  grid-template-columns: 1fr;
  ${props =>
    props.isMultiSelect && props.hasSelected
      ? css`
          grid-template-rows: 1fr 69px;
          grid-template-areas: "column-options" "footer";
        `
      : css`
          grid-template-rows: 1fr;
          grid-template-areas: "column-options";
        `}
`;

const StyledSelector = styled(Container)`
  display: grid;

  ${props => (props.displayType === "dropdown" ? dropdownStyles : asideStyles)}

  padding-top: 16px;

  .column-options {
    grid-area: column-options;
    box-sizing: border-box;

    display: grid;
    /* background-color: red; */
    padding: 0 16px 0 16px;
    grid-row-gap: 2px;

    grid-template-columns: 1fr;
    grid-template-rows: ${props =>
        props.displayType === "aside"
          ? props.isMultiSelect && props.allowGroupSelection && props.options && props.options.length > 0
            ? props.groups && props.groups.length > 0
              ? "100px"
              : "30px"
            :  
              props.groups && props.groups.length > 0
                ? "70px"
                : "30px"
          : "30px"} 1fr;
    grid-template-areas: "header-options" "body-options";

    .header-options {
      grid-area: header-options;
      /* background-color: white; */

      ${props =>
        props.displayType === "aside" &&
        css`
          display: grid;
          grid-row-gap: 12px;
          grid-template-columns: 1fr;
          grid-template-rows: 30px 30px ${props =>
              props.isMultiSelect &&
              props.options &&
              props.options.length > 0 &&
              "30px"};
          ${props =>
            props.isMultiSelect && props.options && props.options.length > 0
              ? css`
                  grid-template-areas: "options_searcher" "options_group_selector" "options_group_select_all";
                `
              : css`
                  grid-template-areas: "options_searcher" "options_group_selector";
                `}

          .options_searcher {
            grid-area: options_searcher;
          }

          .options_group_selector {
            grid-area: options_group_selector;
          }

          ${props =>
            props.isMultiSelect &&
            props.options &&
            props.options.length > 0 &&
            css`
              .options_group_select_all {
                grid-area: options_group_select_all;
              }
            `}
        `}

        .options_searcher {

          div:first-child {
            
            :hover {
              border-color: #D0D5DA;
            }
  
            :focus, :focus-within {
              border-color: #2DA7DB;
            }

            & > input {
              color: #A3A9AE;
            }
          }
        }
    }

    .body-options {
      grid-area: body-options;
      margin-left: -8px;
      /* background-color: white; */

      .row-option {
        padding-left: 8px;
        padding-top: 8px;
        box-sizing: border-box;
        height: 32px;
        cursor: pointer;

        .option-info {
          position: absolute;
          top: 10px;
          right: 10px;
        }
      }
    }
  }

  .footer {
    grid-area: footer;
  }
`;

export default StyledSelector;
