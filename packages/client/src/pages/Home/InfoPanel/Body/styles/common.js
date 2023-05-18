import styled, { css } from "styled-components";

import { Base } from "@docspace/components/themes";
import { hugeMobile, tablet } from "@docspace/components/utils/device";

const StyledInfoPanelBody = styled.div`
  height: auto;
  padding: 0px 3px 0 20px;
  color: ${(props) => props.theme.infoPanel.textColor};
  background-color: ${(props) => props.theme.infoPanel.backgroundColor};

  .no-item {
    text-align: center;
  }

  .current-folder-loader-wrapper {
    width: 100%;
    display: flex;
    justify-content: center;
    height: 96px;
    margin-top: 116.56px;
  }

  @media ${hugeMobile} {
    padding: 0px 8px 0 16px;
  }
`;

const StyledTitle = styled.div`
  position: sticky;
  top: 0;
  z-index: 100;
  margin-left: -20px;
  padding: 24px 0 24px 20px;
  background: ${(props) => props.theme.infoPanel.backgroundColor};

  display: flex;
  flex-wrap: no-wrap;
  flex-direction: row;
  align-items: center;
  height: 32px;

  img {
    &.icon {
      display: flex;
      align-items: center;
      svg {
        height: 32px;
        width: 32px;
      }
    }
    &.is-room {
      border-radius: 6px;
      outline: 1px solid ${(props) => props.theme.itemIcon.borderColor};
    }
  }

  .text {
    font-weight: 600;
    font-size: 16px;
    line-height: 22px;
    max-height: 44px;
    margin: 0 8px;
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    -webkit-box-orient: vertical;
    -webkit-line-clamp: 2;
  }

  ${(props) =>
    props.withBottomBorder &&
    css`
      width: calc(100% + 20px);
      margin: 0 -20px 0 -20px;
      padding: 23px 0 23px 20px;
      border-bottom: ${(props) =>
        `solid 1px ${props.theme.infoPanel.borderColor}`};
    `}

  @media ${tablet} {
    width: 440px;
    padding: 24px 20px 24px 20px;
  }

  @media ${hugeMobile} {
    width: calc(100vw - 32px);
    padding: 24px 0 24px 16px;

    ${(props) =>
      props.withBottomBorder &&
      css`
        width: calc(100% + 16px);
        padding: 23px 0 23px 16px;
        margin: 0 -16px 0 -16px;
      `}
  }
`;

const StyledSubtitle = styled.div`
  display: flex;
  flex-direction: row;
  align-items: center;
  width: 100%;
  padding: 24px 0;
`;

const StyledProperties = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;
  gap: 8px;

  .property {
    width: 100%;
    display: grid;
    grid-template-columns: 120px 1fr;
    grid-column-gap: 24px;

    .property-title {
      font-size: 13px;
    }

    .property-content {
      max-width: 100%;
      margin: auto 0;
      font-weight: 600;
      font-size: 13px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .property-tag_list {
      display: flex;
      flex-wrap: wrap;
      gap: 4px;

      .property-tag {
        background: red;
        max-width: 195px;
        margin: 0;
        background: ${(props) => props.theme.infoPanel.details.tagBackground};
        p {
          white-space: nowrap;
          overflow: hidden;
          text-overflow: ellipsis;
        }
      }
    }

    .property-comment_editor {
      &-display {
        display: flex;
        flex-direction: column;
        gap: 4px;

        .edit_toggle {
          cursor: pointer;
          display: flex;
          flex-direction: row;
          gap: 6px;
          &-icon {
            svg {
              width: 12px;
              height: 12px;
              path {
                fill: ${(props) =>
                  props.theme.infoPanel.details.commentEditorIconColor};
              }
            }
          }
          &-text {
            text-decoration: underline;
            text-decoration-style: dashed;
            text-underline-offset: 2px;
          }
        }
      }

      &-editor {
        display: flex;
        flex-direction: column;
        gap: 8px;
        &-buttons {
          display: flex;
          flex-direction: row;
          gap: 4px;
        }
      }

      .property-content {
        white-space: pre-wrap;
        display: -webkit-box;
        display: -moz-box;
        display: -ms-box;
        word-break: break-word;
        overflow: hidden;
        -webkit-line-clamp: 3;
        -webkit-box-orient: vertical;
      }
    }
  }
`;

StyledInfoPanelBody.defaultProps = { theme: Base };
StyledTitle.defaultProps = { theme: Base };

export { StyledInfoPanelBody, StyledTitle, StyledSubtitle, StyledProperties };
