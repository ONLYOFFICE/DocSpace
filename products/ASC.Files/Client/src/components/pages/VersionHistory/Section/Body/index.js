import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";

import { RowContainer } from "asc-web-components";
import { Loaders } from "asc-web-common";
import VersionRow from "./VersionRow";

import {
  fetchFileVersions,
  setIsLoading,
  setFirstLoad,
} from "../../../../../store/files/actions";
import {
  getFileVersions,
  getIsLoading,
} from "../../../../../store/files/selectors";

class SectionBodyContent extends React.Component {
  componentDidMount() {
    const { match, setFirstLoad } = this.props;
    const { fileId } = match.params;

    if (fileId) {
      this.getFileVersions(fileId);
      setFirstLoad(false);
    }
  }

  getFileVersions = (fileId) => {
    const { fetchFileVersions, setIsLoading } = this.props;
    setIsLoading(true);
    fetchFileVersions(fileId).then(() => setIsLoading(false));
  };
  render() {
    const { versions, culture, isLoading } = this.props;
    console.log("VersionHistory SectionBodyContent render()", versions);

    let itemVersion = null;

    return versions && !isLoading ? (
      <RowContainer useReactWindow={false}>
        {versions.map((info, index) => {
          let isVersion = true;
          if (itemVersion === info.versionGroup) {
            isVersion = false;
          } else {
            itemVersion = info.versionGroup;
          }

          return (
            <VersionRow
              getFileVersions={this.getFileVersions}
              isVersion={isVersion}
              key={info.id}
              info={info}
              index={index}
              culture={culture}
            />
          );
        })}
      </RowContainer>
    ) : (
      <Loaders.HistoryRows title="version-history-body-loader" />
    );
  }
}

const mapStateToProps = (state) => {
  return {
    versions: getFileVersions(state),
    isLoading: getIsLoading(state),
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchFileVersions: (fileId) => dispatch(fetchFileVersions(fileId)),
    setIsLoading: (isLoading) => dispatch(setIsLoading(isLoading)),
    setFirstLoad: (isFirstLoad) => dispatch(setFirstLoad(isFirstLoad)),
  };
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(withRouter(SectionBodyContent));
