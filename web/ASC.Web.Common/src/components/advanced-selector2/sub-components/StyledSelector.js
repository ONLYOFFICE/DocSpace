import React from "react";
import styled, { css } from "styled-components";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
    displayType,
    options,
    groups,
    isMultiSelect,
    hasSelected,
    ...props
  }) => <div {...props} />;
  /* eslint-enable react/prop-types */
  /* eslint-enable no-unused-vars */
  
  const StyledSelector = styled(Container)`
    display: grid;
  
    ${props =>
      props.displayType === "dropdown"
        ? css`
            grid-auto-rows: max-content;
            grid-template-areas: "column-options column-groups" "footer footer";
  
            .column-groups {
              grid-area: column-groups;
  
              ${props =>
                props.groups && props.groups.length > 0
                  ? css`
                      border-left: 1px solid #eceef1;
                    `
                  : ""}
  
              display: grid;
              /* background-color: gold; */
              padding: 16px 16px 0 16px;
              grid-row-gap: 16px;
  
              grid-template-columns: 1fr;
              grid-template-rows: 30px 0.98fr;
              grid-template-areas: "header-groups" "body-groups";
  
              .header-groups {
                grid-area: header-groups;
                /* background-color: white; */
              }
  
              .body-groups {
                grid-area: body-groups;
                margin-left: -8px;
                /* background-color: white; */
  
                .row-block {
                  padding-left: 8px;
  
                  .group_checkbox {
                    display: inline-block;
                  }
                }
              }
            }
          `
        : css`
            height: 100%;
            grid-template-columns: 1fr;
            ${props =>
              props.isMultiSelect && props.hasSelected
                ? css`
                    grid-template-rows: 0.98fr 69px;
                    grid-template-areas: "column-options" "footer";
                  `
                : css`
                    grid-template-rows: 0.98fr;
                    grid-template-areas: "column-options";
                  `}
          `}
  
    .column-options {
      grid-area: column-options;
  
      display: grid;
      /* background-color: red; */
      padding: 16px 16px 0 16px;
      grid-row-gap: 16px;
  
      grid-template-columns: 1fr;
      grid-template-rows: ${props =>
          props.displayType === "aside"
            ? props.isMultiSelect && props.options && props.options.length > 0
              ? props.groups && props.groups.length
                ? "100px"
                : "30px"
              : "70px"
            : "30px"} 0.98fr;
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
      }
  
      .body-options {
        grid-area: body-options;
        margin-left: -8px;
        /* background-color: white; */
  
        .row-block {
          padding-left: 8px;
  
          .option-info {
            position: absolute;
            top: 10px;
            right: 10px;
          }
        }
      }
    }
  
    .row-block {
      box-sizing: border-box;
      line-height: 32px;
      cursor: pointer;
  
      &:hover {
        background-color: #f8f9f9;
      }
    }
  
    .row-block.selected {
      background-color: #eceef1;
    }
  
    .footer {
      grid-area: footer;
    }
  `;

  export default StyledSelector;