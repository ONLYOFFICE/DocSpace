import React from "react";
import { withRouter } from "react-router";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";

import { isMobile } from "react-device-detect";

import RowContainer from "@appserver/components/row-container";

import RoomsRow from "./sub-components/RoomsRow";
import { Base } from "@appserver/components/themes";

const marginStyles = css`
  margin-left: -24px;
  margin-right: -24px;
  padding-left: 24px;
  padding-right: 24px;

  ${isMobile &&
  css`
    margin-left: -20px;
    margin-right: -20px;
    padding-left: 20px;
    padding-right: 20px;
  `}

  @media (max-width: 1024px) {
    margin-left: -16px;
    margin-right: -16px;
    padding-left: 16px;
    padding-right: 16px;
  }

  @media (max-width: 375px) {
    margin-left: -16px;
    margin-right: -8px;
    padding-left: 16px;
    padding-right: 8px;
  }
`;

const StyledRowContainer = styled(RowContainer)`
  margin-top: -15px;

  .row-selected + .row-wrapper:not(.row-selected) {
    .rooms-row {
      border-top: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      margin-top: -3px;
      ${marginStyles};
    }
  }

  .row-wrapper:not(.row-selected) + .row-selected {
    .rooms-row {
      border-top: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      margin-top: -3px;
      ${marginStyles};
    }
  }

  .row-selected:last-child {
    .rooms-row {
      border-bottom: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      padding-bottom: 1px;
      ${marginStyles};
    }
    .rooms-row::after {
      height: 0px;
    }
  }
  .row-selected:first-child {
    .rooms-row {
      border-top: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      margin-top: -3px;
      ${marginStyles};
    }
  }
`;

StyledRowContainer.defaultProps = { theme: Base };

const VirtualRoomsRow = ({
  viewAs,
  setViewAs,

  rooms,
  sectionWidth,

  history,
}) => {
  React.useEffect(() => {
    if ((viewAs !== "table" && viewAs !== "row") || !setViewAs) return;

    if (sectionWidth < 1025 || isMobile) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  return (
    <StyledRowContainer className="rooms-row-container" useReactWindow={false}>
      {rooms.map((item) => (
        <RoomsRow
          key={item.id}
          item={item}
          sectionWidth={sectionWidth}
          history={history}
        />
      ))}
    </StyledRowContainer>
  );
};

export default inject(
  ({ auth, filesStore, roomsStore, contextOptionsStore }) => {
    const { settingsStore } = auth;

    const { theme } = settingsStore;

    const { viewAs, setViewAs } = filesStore;

    const { getRoomsContextOptions } = contextOptionsStore;

    const { rooms } = roomsStore;

    return {
      theme,
      viewAs,
      setViewAs,
      getRoomsContextOptions,
      userId: auth.userStore.user.id,
      rooms,
    };
  }
)(withRouter(observer(VirtualRoomsRow)));
