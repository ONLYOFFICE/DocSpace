import styled from "styled-components";
import { isMobileOnly } from "react-device-detect";
import { Base } from "@docspace/components/themes";

const StyledThumbnail = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: ${isMobileOnly ? "188" : "240"}px;
  img {
    border: ${(props) => `solid 1px ${props.theme.infoPanel.borderColor}`};
    border-radius: 6px;
    width: 100%;
    height: 100%;
    object-fit: none;
    object-position: top;
  }
`;

const StyledNoThumbnail = styled.div`
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
  .custom-logo {
    outline: 1px solid ${(props) =>
      props.theme.infoPanel.details.customLogoBorderColor};
`;

const StyledAccess = styled.div`
  display: flex;
  flex-wrap: wrap;
  flex-direction: row;
  gap: 8px;
  align-items: center;
  .divider {
    background: ${(props) => props.theme.infoPanel.borderColor};
    margin: 2px 4px;
    width: 1px;
    height: 28px;
  }

  .show-more-users {
    position: static;
    width: 101px;
    height: 16px;
    left: 120px;
    top: 8px;
    padding-left: 1px;

    font-family: "Open Sans";
    font-style: normal;
    font-weight: normal;
    font-size: 12px;
    line-height: 16px;
    text-align: left;

    color: ${(props) => props.theme.infoPanel.showAccessUsersTextColor};

    flex: none;
    order: 3;
    flex-grow: 0;

    cursor: pointer;
    &:hover {
      text-decoration: underline;
    }
  }
`;

const StyledAccessItem = styled.div`
  width: 32px;
  height: 32px;
  border-radius: 50%;

  .access-item-tooltip {
    cursor: pointer;
    width: 100%;
    height: 100%;

    .item-group {
      border-radius: 50%;
      background-color: ${(props) => props.theme.infoPanel.accessGroupBg};
      width: 100%;
      height: 100%;
      display: flex;
      align-items: center;
      justify-content: center;

      span {
        font-family: "Open Sans";
        font-weight: 700;
        font-size: 12px;
        color: ${(props) => props.theme.infoPanel.accessGroupText};
        line-height: 16px;
      }
    }

    .item-user {
      img {
        border-radius: 50%;
        width: 100%;
        height: 100%;
      }
    }
  }
`;

const StyledOpenSharingPanel = styled.div`
  position: static;
  width: auto;
  height: 15px;
  left: 0px;
  top: 2px;

  font-family: "Open Sans";
  font-style: normal;
  font-weight: 600;
  font-size: 13px;
  line-height: 15px;

  color: ${(props) => props.theme.infoPanel.showAccessPanelTextColor};

  display: flex;
  margin: 16px 0px;

  cursor: pointer;
  text-decoration: underline;
  text-decoration-style: dashed;
`;

StyledThumbnail.defaultProps = { theme: Base };
StyledNoThumbnail.defaultProps = { theme: Base };
StyledAccess.defaultProps = { theme: Base };
StyledAccessItem.defaultProps = { theme: Base };
StyledOpenSharingPanel.defaultProps = { theme: Base };

export {
  StyledThumbnail,
  StyledNoThumbnail,
  StyledAccess,
  StyledAccessItem,
  StyledOpenSharingPanel,
};
