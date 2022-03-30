import styled from "styled-components";

import { Base } from "@appserver/components/themes";

const StyledInfoRoomBody = styled.div`
  padding: 0px 0px 0 16px;
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
  flex-direction: row;
  align-items: center;
  width: 100%;
  height: auto;
  padding: 29px 0;

  .icon {
    svg {
      height: 32px;
      width: 32px;
    }
  }

  .text {
    font-family: "Open Sans";
    line-height: 22px;
    margin: 0 8px;
  }
`;

const StyledThumbnail = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  width: calc(100% + 6px -16px);
  height: 200px;
  padding: 4px;
  border: ${(props) => `solid 1px ${props.theme.infoPanel.borderColor}`};
  border-radius: 6px;
  margin: 0 0 24px 0;
  img {
    height: 100%;
    width: auto;
    max-height: auto;
    max-width: 100%;
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
    grid-template-columns: 150px 1fr;
    grid-column-gap: 24px;

    .property-title {
      font-size: 13px;
    }

    .property-content {
      display: flex;
      align-items: center;

      font-weight: 600;
      font-size: 13px;
    }
  }
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

    font-family: Open Sans;
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

const StyledAccessUser = styled.div`
  width: 32px;
  height: 32px;
  border-radius: 50%;

  a {
    img {
      border-radius: 50%;
      width: 100%;
      height: 100%;
    }
  }
`;

const StyledOpenSharingPanel = styled.div`
  position: static;
  width: auto;
  height: 15px;
  left: 0px;
  top: 2px;

  font-family: Open Sans;
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

StyledInfoRoomBody.defaultProps = { theme: Base };
StyledThumbnail.defaultProps = { theme: Base };
StyledAccess.defaultProps = { theme: Base };
StyledOpenSharingPanel.defaultProps = { theme: Base };

export {
  StyledInfoRoomBody,
  StyledTitle,
  StyledThumbnail,
  StyledSubtitle,
  StyledProperties,
  StyledAccess,
  StyledAccessUser,
  StyledOpenSharingPanel,
};
