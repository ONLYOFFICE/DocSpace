import styled, { css } from "styled-components";

import { Base } from "@docspace/components/themes";
import { mobile } from "@docspace/components/utils/device";

const StyledInfoPanelBody = styled.div`
  padding: 0 3px 0 20px;
  @media ${mobile} {
    padding: 0 8px 0 16px;
  }
  height: auto;
  background-color: ${(props) => props.theme.infoPanel.backgroundColor};
  color: ${(props) => props.theme.infoPanel.textColor};

  .no-item {
    text-align: center;
  }

  .no-thumbnail-img-wrapper {
    height: auto;
    width: 100%;
    display: flex;
    justify-content: center;
    .no-thumbnail-img {
      height: 96px;
      width: 96px;
    }
    .is-room {
      border-radius: 16px;
    }
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
  display: flex;
  flex-wrap: no-wrap;
  flex-direction: row;
  align-items: center;
  width: 100%;
  height: 32px;
  padding: 24px 0;

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
      margin: auto 0;

      font-weight: 600;
      font-size: 13px;
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
  }
`;

StyledInfoPanelBody.defaultProps = { theme: Base };
StyledTitle.defaultProps = { theme: Base };

export { StyledInfoPanelBody, StyledTitle, StyledSubtitle, StyledProperties };
