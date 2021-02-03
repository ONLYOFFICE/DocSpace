import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import styled from "styled-components";
import { Icons, Badge } from "asc-web-components";
import { canWebEdit, canConvert } from "../../../../../store/files/selectors";

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

const mapStateToProps = (state, props) => {
  return {
    canWebEdit: canWebEdit(props.item.fileExst)(state),
    canConvert: canConvert(props.item.fileExst)(state),
  };
};

export default connect(mapStateToProps, {})(withRouter(BadgesFileTile));
