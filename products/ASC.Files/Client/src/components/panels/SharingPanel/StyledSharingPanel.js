import { Base } from "@appserver/components/themes";
import styled, { css } from "styled-components";

import { isMobile, isMobileOnly } from "react-device-detect";

const StyledContainer = styled.div`
  transition: unset;
  transform: translateX(${(props) => (props.visible ? "0" : "480px")});
  width: 480px;
  height: 100%;
  position: fixed;
  max-width: 480px;
  overflow-y: hidden;

  right: 0;
  bottom: 0;

  background: #333333;

  z-index: 311;

  @media (max-width: 500px) {
    position: fixed;
    top: unset;

    bottom: 0;
    right: 0;

    width: 100%;
    height: calc(100% - 64px);
  }

  .panel_combo-box {
    .optionalBlock {
      margin-right: -3px;
    }
  }
`;

const StyledContent = styled.div`
  width: 100%;
  height: 100%;

  display: grid;

  grid-template-columns: 1fr;
  grid-template-rows: ${(props) =>
    props.isNotifyUsers
      ? "53px calc(100% - 253px) 200px"
      : "53px calc(100% - 161px) 108px"};
`;

const StyledHeaderContent = styled.div`
  width: 100%;
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

    width: calc(100% - 33px);
    max-width: calc(100% - 33px);

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

  button {
    height: 40px;

    box-sizing: border-box;
  }

  button:first-child {
    margin-right: 8px;
  }
`;

export {
  StyledContainer,
  StyledContent,
  StyledHeaderContent,
  StyledBodyContent,
  StyledExternalLink,
  StyledInternalLink,
  StyledItem,
  StyledFooterContent,
  StyledModalFooter,
};
