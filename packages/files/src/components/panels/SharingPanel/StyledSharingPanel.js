import { Base } from "@docspace/components/themes";
import styled, { css } from "styled-components";

import { isMobile, isMobileOnly } from "react-device-detect";

const StyledContent = styled.div`
  width: 100%;
  height: 100%;

  display: grid;

  grid-template-columns: 100%;
  grid-template-rows: ${(props) =>
    props.isNotifyUsers
      ? "53px calc(100% - 254px) 201px"
      : "53px calc(100% - 162px) 109px"};
`;

const StyledHeaderContent = styled.div`
  width: auto;
  max-width: 100%;
  height: ${(props) => (props.isPersonal ? "40px" : "53px")};

  border-bottom: ${(props) =>
    props.isPersonal ? "none" : props.theme.filesPanels.sharing.borderBottom};

  padding: ${(props) => (props.isPersonal ? "0 4px" : "0 16px")};

  box-sizing: border-box;

  display: flex;
  align-items: center;
  justify-content: space-between;

  .sharing_panel-header-info {
    display: flex;
    align-items: center;
    justify-content: start;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;

    width: 100%;

    .sharing_panel-arrow {
      .icon-button_svg {
        width: 15px;
      }
      margin-right: 16px;
    }
  }

  ${(props) =>
    props.isEmbedding &&
    css`
      width: 100%;
      display: flex;
      align-items: center;
      justify-content: start;
      margin-bottom: 16px;
    `}

  .sharing_panel-icons-container {
    display: flex;
    margin-left: 16px;
  }
`;

StyledHeaderContent.defaultProps = { theme: Base };

const StyledBodyContent = styled.div`
  width: 100%;
  height: 100%;

  display: flex;
  flex-direction: column;
  align-items: start;

  .body-scroll-content-sharing-panel {
    width: 100%;
    height: ${(props) =>
      props.externalLinkVisible
        ? !props.externalLinkOpen
          ? "calc(100% - 125px)"
          : "calc(100% - 207px)"
        : "calc(100% - 62px)"};
    max-height: ${(props) =>
      props.externalLinkVisible
        ? !props.externalLinkOpen
          ? "calc(100% - 125px)"
          : "calc(100% - 207px)"
        : "calc(100% - 62px)"};

    ${isMobileOnly &&
    css`
      height: 100% !important;
      max-height: 100% !important;
    `}
  }
`;

const StyledExternalLink = styled.div`
  width: 100%;

  padding: ${(props) =>
    props.isPersonal
      ? props.isOpen
        ? "8px 4px 4px"
        : "8px 4px 20px"
      : "20px 16px"};

  border-bottom: ${(props) =>
    props.isPersonal ? "none" : props.theme.filesPanels.sharing.borderBottom};

  box-sizing: border-box;

  display: flex;
  flex-direction: column;
  align-items: start;

  .external-link__base-line {
    display: flex;
    align-items: center;
    justify-content: start;
    flex-direction: row;

    .external-link__text {
      font-weight: 700;
      font-size: 16px;
      line-height: 22px;

      margin-right: 16px;
    }

    .external-link__toggler {
      position: relative;
    }
  }

  .external-link__checked {
    margin-top: 16px;
    width: 100%;

    display: flex;
    align-items: center;
    justify-content: start;

    .external-link__input-link {
      flex-direction: row-reverse;
      padding-right: 0px;

      .external-link__buttons {
        position: relative;

        height: 100%;

        display: flex;
        align-items: center;

        padding: 4px 16px;

        .external-link__code-icon {
          margin-right: 12px;

          cursor: pointer;
          path {
            fill: ${(props) =>
              props.theme.filesPanels.sharing.externalLinkSvg} !important;
          }
        }

        .external-link__share-icon {
          cursor: pointer;
          path {
            fill: ${(props) =>
              props.theme.filesPanels.sharing.externalLinkSvg} !important;
          }
        }

        external-link__share-dropdown {
        }
      }

      .append {
        display: none;
      }
    }

    .external-link__copy {
      margin-left: 8px;
    }

    .panel_combo-box {
      .combo-button {
        min-width: 46px;
        height: 32px;

        .sharing-access-combo-box-icon {
          display: flex;
          align-items: center;
        }
      }
      .dropdown-container {
        top: 32px;
      }
    }
  }

  .external-link__access-rights {
    display: flex;
    align-items: center;
    justify-content: start;
    flex-direction: row;

    margin-top: 16px;

    .external-link__access-rights_text {
      color: #a3a9ae;
      margin-right: 8px;
    }
  }
`;

StyledExternalLink.defaultProps = { theme: Base };

const StyledInternalLink = styled.div`
  width: 100%;

  display: flex;
  align-items: center;
  justify-content: space-between;

  padding: 20px 16px;

  box-sizing: border-box;

  .internal-link__link-text {
    line-height: 22px !important;
    font-size: 16px !important;
    font-weight: 700 !important;
  }

  .internal-link__copy-text {
    line-height: 15px !important;
    font-weight: 600 !important;

    border-bottom: ${(props) =>
      props.theme.filesPanels.sharing.internalLinkBorder};

    cursor: pointer;
  }
`;

StyledInternalLink.defaultProps = { theme: Base };

const StyledItem = styled.div`
  width: 100%;

  display: flex;
  align-items: center;
  justify-content: space-between;

  padding: 8px 16px;

  box-sizing: border-box;

  .item__info-block {
    display: flex;
    align-items: center;
    justify-content: start;

    .info-block__text {
      margin-left: 12px;
    }
  }

  .item__change-owner {
    line-height: 15px !important;
    font-weight: 600 !important;

    border-bottom: ${(props) => props.theme.filesPanels.sharing.itemBorder};

    cursor: pointer;
  }

  .item__owner {
    color: ${(props) => props.theme.filesPanels.sharing.itemOwnerColor};
  }

  .panel_combo-box {
    .combo-button {
      min-width: 46px;
      height: 32px;

      .sharing-access-combo-box-icon {
        display: flex;
        align-items: center;
      }
    }
  }
`;

StyledItem.defaultProps = { theme: Base };

const StyledFooterContent = styled.div`
  width: 100%;

  min-height: 100px;
  border-top: ${(props) => props.theme.filesPanels.sharing.borderTop};

  position: relative;

  box-sizing: border-box;

  padding: 16px;

  display: flex;
  flex-direction: column;
  align-items: start;

  .sharing_panel-notification {
    margin-bottom: 18px;
  }

  .sharing_panel-checkbox {
    margin-bottom: 18px;
  }
  .sharing_panel-button {
    min-height: 40px;
  }
`;

StyledFooterContent.defaultProps = { theme: Base };

const StyledModalFooter = styled.div`
  width: 100%;

  padding: 16px 4px 4px;

  box-sizing: border-box;

  display: flex;
  align-items: center;
  justify-content: space-between;

  div:first-child {
    position: relative;

    display: flex;
    align-items: center;
    justify-content: space-between;

    width: 100%;

    padding: 0;
  }

  button {
    width: 100%;
    height: 40px;

    box-sizing: border-box;

    .button-content {
      display: flex;
      align-items: center;
      justify-content: center;
    }
  }

  button:first-child {
    margin-right: 8px;
  }
`;

export {
  StyledContent,
  StyledHeaderContent,
  StyledBodyContent,
  StyledExternalLink,
  StyledInternalLink,
  StyledItem,
  StyledFooterContent,
  StyledModalFooter,
};
