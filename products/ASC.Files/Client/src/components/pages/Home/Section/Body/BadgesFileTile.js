import React from "react";
import { withRouter } from "react-router";
import styled from "styled-components";
import { Icons, Badge } from "asc-web-components";
import { inject, observer } from "mobx-react";

const StyledBadgesFileTile = styled.div`
  display: flex;
  align-self: start;
  align-items: center;
  position: relative;
  margin: -5px;

  & > svg,
  & > div {
    margin: 5px;
  }
`;

class BadgesFileTile extends React.PureComponent {
  render() {
    const { item, canConvert, canWebEdit } = this.props;
    const { fileStatus, id, versionGroup } = item;

    return (
      <StyledBadgesFileTile>
        {canConvert && (
          <Icons.FileActionsConvertIcon
            className="badge"
            size="small"
            isfill={true}
            color="#A3A9AE"
          />
        )}
        {canWebEdit && (
          <Icons.AccessEditIcon
            className="badge"
            size="small"
            isfill={true}
            color="#A3A9AE"
          />
        )}
        {fileStatus === 1 && (
          <Icons.FileActionsConvertEditDocIcon
            className="badge"
            size="small"
            isfill={true}
            color="#3B72A7"
          />
        )}
        {false && (
          <Icons.FileActionsLockedIcon
            className="badge"
            size="small"
            isfill={true}
            color="#3B72A7"
          />
        )}
        {versionGroup > 1 && (
          <Badge
            className="badge-version"
            backgroundColor="#A3A9AE"
            borderRadius="11px"
            color="#FFFFFF"
            fontSize="10px"
            fontWeight={800}
            label={`Ver.${versionGroup}`}
            maxWidth="50px"
            onClick={this.onShowVersionHistory}
            padding="0 5px"
            data-id={id}
          />
        )}
        {fileStatus === 2 && (
          <Badge
            className="badge-version"
            backgroundColor="#ED7309"
            borderRadius="11px"
            color="#FFFFFF"
            fontSize="10px"
            fontWeight={800}
            label={`New`}
            maxWidth="50px"
            onClick={this.onBadgeClick}
            padding="0 5px"
            data-id={id}
          />
        )}
      </StyledBadgesFileTile>
    );
  }
}

export default inject(({ formatsStore }, { item }) => {
  const { docserviceStore } = formatsStore;

  const canWebEdit = docserviceStore.canWebEdit(item.fileExst);
  const canConvert = docserviceStore.canConvert(item.fileExst);

  return {
    canWebEdit,
    canConvert,
  };
})(withRouter(observer(BadgesFileTile)));
