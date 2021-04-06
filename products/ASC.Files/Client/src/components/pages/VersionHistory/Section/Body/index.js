import React from "react";
import { withRouter } from "react-router";
import RowContainer from "@appserver/components/row-container";
import Loaders from "@appserver/common/components/Loaders";
import VersionRow from "./VersionRow";
import { inject, observer } from "mobx-react";

class SectionBodyContent extends React.Component {
  componentDidMount() {
    const { match, setFirstLoad } = this.props;
    const fileId = match.params.fileId || this.props.fileId;

    if (fileId && fileId !== this.props.fileId) {
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
    //console.log("VersionHistory SectionBodyContent render()", versions);

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
              key={`${info.id}-${index}`}
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

export default inject(({ auth, filesStore, versionHistoryStore }) => {
  const { setFirstLoad, setIsLoading, isLoading } = filesStore;
  const {
    versions,
    fetchFileVersions,
    fileId,
    setVerHistoryFileId,
  } = versionHistoryStore;

  return {
    culture: auth.settingsStore.culture,
    isLoading,
    versions,
    fileId,

    setFirstLoad,
    setIsLoading,
    fetchFileVersions,
    setVerHistoryFileId,
  };
})(withRouter(observer(SectionBodyContent)));
