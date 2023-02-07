import React, { memo } from "react";
import { withRouter } from "react-router";
import Loaders from "@docspace/common/components/Loaders";
import VersionRow from "./VersionRow";
import { inject, observer } from "mobx-react";
import { VariableSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";
import { StyledBody, StyledVersionList } from "./StyledVersionHistory";
class SectionBodyContent extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isRestoreProcess: false,
      rowSizes: {},
    };
    this.listKey = 0;
    this.listRef = React.createRef();
    this.timerId = null;
  }

  componentDidMount() {
    const { match, setFirstLoad } = this.props;
    const fileId = match.params.fileId || this.props.fileId;

    if (fileId && fileId !== this.props.fileId) {
      this.getFileVersions(fileId, this.props.fileSecurity);
      setFirstLoad(false);
    }
  }

  getFileVersions = (fileId, fileSecurity) => {
    const { fetchFileVersions, setIsLoading } = this.props;
    setIsLoading(true);
    fetchFileVersions(fileId, fileSecurity).then(() => setIsLoading(false));
  };

  onSetRestoreProcess = (restoring) => {
    const { isRestoreProcess } = this.state;

    if (restoring) {
      this.timerId = setTimeout(
        () =>
          this.setState({
            isRestoreProcess: restoring,
          }),
        100
      );
    } else {
      clearTimeout(this.timerId);
      this.timerId = null;

      restoring !== isRestoreProcess &&
        this.setState({
          isRestoreProcess: restoring,
        });
    }
  };
  onUpdateHeight = (i, itemHeight) => {
    if (this.listRef.current) {
      this.listRef.current.resetAfterIndex(i);
    }

    this.setState((prevState) => ({
      rowSizes: {
        ...prevState.rowSizes,
        [i]: itemHeight + 24, //composed of itemHeight = clientHeight of div and padding-top = 12px and padding-bottom = 12px
      },
    }));
  };

  getSize = (i) => {
    return this.state.rowSizes[i] ? this.state.rowSizes[i] : 66;
  };

  renderRow = memo(({ index, style }) => {
    const { versions, culture } = this.props;

    const prevVersion = versions[index > 0 ? index - 1 : index].versionGroup;
    let isVersion = true;

    if (index > 0 && prevVersion === versions[index].versionGroup) {
      isVersion = false;
    }
    return (
      <div style={style}>
        <VersionRow
          getFileVersions={this.getFileVersions}
          isVersion={isVersion}
          key={`${versions[index].id}-${index}`}
          info={versions[index]}
          versionsListLength={versions.length}
          index={index}
          culture={culture}
          onSetRestoreProcess={this.onSetRestoreProcess}
          onUpdateHeight={this.onUpdateHeight}
        />
      </div>
    );
  }, areEqual);
  render() {
    const { versions, isLoading } = this.props;

    const renderList = ({ height, width }) => {
      return (
        <StyledVersionList isRestoreProcess={this.state.isRestoreProcess}>
          <List
            ref={this.listRef}
            className="List"
            height={height}
            width={width}
            itemSize={this.getSize}
            itemCount={versions.length}
            itemData={versions}
            outerElementType={CustomScrollbarsVirtualList}
          >
            {this.renderRow}
          </List>
        </StyledVersionList>
      );
    };

    return (
      <StyledBody>
        {versions && !isLoading ? (
          <div className="version-list">
            <AutoSizer>{renderList}</AutoSizer>
          </div>
        ) : (
          <div className="loader-history-rows">
            <Loaders.HistoryRows title="version-history-body-loader" />
          </div>
        )}
      </StyledBody>
    );
  }
}

export default inject(({ auth, filesStore, versionHistoryStore }) => {
  const { setFirstLoad, setIsLoading, isLoading } = filesStore;
  const {
    versions,
    fetchFileVersions,
    fileId,
    fileSecurity,
  } = versionHistoryStore;

  return {
    culture: auth.settingsStore.culture,
    isLoading,
    versions,
    fileId,
    fileSecurity,
    setFirstLoad,
    setIsLoading,
    fetchFileVersions,
  };
})(withRouter(observer(SectionBodyContent)));
