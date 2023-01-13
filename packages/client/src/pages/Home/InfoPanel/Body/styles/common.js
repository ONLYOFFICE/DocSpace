import styled, { css } from "styled-components";

import { Base } from "@docspace/components/themes";
import { mobile, tablet } from "@docspace/components/utils/device";

const StyledInfoPanelBody = styled.div`
  ${({ isAccounts }) =>
    isAccounts
      ? css`
          padding: 0px 3px 0 20px;
          @media ${mobile} {
            padding: 0px 8px 0 16px;
          }
        `
      : css`
          padding: 80px 3px 0 20px;
          @media ${mobile} {
            padding: 80px 8px 0 16px;
          }
        `}

  height: auto;
  background-color: ${(props) => props.theme.infoPanel.backgroundColor};
  color: ${(props) => props.theme.infoPanel.textColor};

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
`;

const StyledTitle = styled.div`
  position: fixed;
  margin-top: -80px;
  margin-left: -20px;
  width: calc(100% - 40px);
  padding: 24px 0 24px 20px;
  background: ${(props) => props.theme.infoPanel.backgroundColor};
  z-index: 100;

  @media ${tablet} {
    width: 440px;
    padding: 24px 0 24px 20px;
  }

  @media ${mobile} {
    width: calc(100% - 32px);
    padding: 24px 0 24px 16px;
  }

  display: flex;
  flex-wrap: no-wrap;
  flex-direction: row;
  align-items: center;
  height: 32px;

  ${(props) =>
    props.withBottomBorder &&
    css`
      width: calc(100% + 20px);
      margin: 0 -20px 0 -20px;
      padding: 23px 0 23px 20px;
      border-bottom: ${(props) =>
        `solid 1px ${props.theme.infoPanel.borderColor}`};

      @media ${mobile} {
        width: calc(100% + 16px);
        padding: 23px 0 23px 16px;
        margin: 0 -16px 0 -16px;
      }
    `}

  .item-icon {
    height: 32px;
    width: 32px;
  }
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

  .context-menu-button {
    margin: 0 20px 0 auto;
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
        max-width: 195px;
        margin: 0;
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
    }
  }
`;

StyledInfoPanelBody.defaultProps = { theme: Base };
StyledTitle.defaultProps = { theme: Base };

export { StyledInfoPanelBody, StyledTitle, StyledSubtitle, StyledProperties };
