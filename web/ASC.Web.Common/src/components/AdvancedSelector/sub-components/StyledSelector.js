import React from "react";
import styled, { css } from "styled-components";
import { utils } from "asc-web-components";
const { tablet } = utils.device;

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

  ${(props) =>
    props.groups && props.groups.length > 0
      ? css`
          grid-template-areas: "column-options splitter column-groups" "footer footer footer";
        `
      : css`
          grid-template-areas: "column-options column-groups" "footer footer";
        `};

  .column-groups {
    box-sizing: border-box;
    grid-area: column-groups;

    display: grid;
    /* background-color: gold; */
    padding: 16px 16px 0 16px;
    grid-row-gap: 2px;

    grid-template-columns: 1fr;
    grid-template-rows: 30px 1fr;
    grid-template-areas: "header-groups" "body-groups";

    .header-groups {
      grid-area: header-groups;

      .group_header {
        line-height: 30px;
      }
      /* background-color: white; */
    }

    .body-groups {
      grid-area: body-groups;
      margin-left: -8px;
      /* background-color: white; */

      .row-group:first-child {
        font-weight: 700;
      }

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
          background-color: #eceef1;
          border-radius: 3px;
        }
      }

      .row-group.selected {
        background-color: #eceef1;
        border-radius: 3px;
      }
    }
  }

  ${(props) =>
    props.groups &&
    props.groups.length > 0 &&
    css`
      .splitter {
        grid-area: splitter;
        border-left: 1px solid #eceef1;
        margin-top: 16px;
      }
    `}
`;

const asideStyles = css`
  height: 100%;
  grid-template-columns: 1fr;
  ${(props) =>
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

  ${(props) =>
    props.displayType === "dropdown" ? dropdownStyles : asideStyles}

  .column-options {
    grid-area: column-options;
    box-sizing: border-box;

    display: grid;
    /* background-color: red; */
    padding: 16px 16px 0 16px;
    grid-row-gap: 2px;

    grid-template-columns: 1fr;
    grid-template-rows: ${(props) =>
        props.displayType === "aside"
          ? props.isMultiSelect &&
            props.allowGroupSelection &&
            props.options &&
            props.options.length > 0
            ? props.groups && props.groups.length > 0
              ? "100px"
              : "30px"
            : props.groups && props.groups.length > 0
            ? "70px"
            : "30px"
          : "30px"} 1fr;
    grid-template-areas: "header-options" "body-options";

    .header-options {
      grid-area: header-options;
      margin-right: 2px;
      /* background-color: white; */

      ${(props) =>
        props.displayType === "aside" &&
        css`
          display: grid;
          grid-row-gap: 17px;
          grid-template-columns: 1fr;
          grid-template-rows: 30px 30px ${(props) =>
              props.isMultiSelect &&
              props.options &&
              props.options.length > 0 &&
              "30px"};
          ${(props) =>
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

          ${(props) =>
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
            border-color: #d0d5da;
          }

          :focus,
          :focus-within {
            border-color: #2da7db;
          }

          & > input::placeholder {
            color: #a3a9ae;
          }
        }
      }
    }

    .body-options {
      grid-area: body-options;
      margin-left: -8px;
      margin-top: 5px;

      @media ${tablet} {
        width: 290px;
      }

      /* background-color: white; */

      .row-option {
        padding-left: 8px;
        padding-top: 8px;
        box-sizing: border-box;
        height: 32px;
        margin-top: 16px;
        cursor: pointer;

        &:hover {
          background-color: #eceef1;
          border-radius: 3px;
        }

        .option_checkbox {
          width: 265px;
        }

        .option-info {
          position: absolute;
          top: 12px;
          right: 10px;
          padding: 8px 0 8px 8px;
          margin-top: -8px;
        }

        /* .__react_component_tooltip {
          left: 8px !important;
        } */
      }
    }
  }

  .footer {
    grid-area: footer;
  }
`;

export default StyledSelector;
