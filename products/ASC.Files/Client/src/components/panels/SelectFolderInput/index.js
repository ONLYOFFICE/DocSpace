import React from "react";
import { Provider as MobxProvider, inject, observer } from "mobx-react";

import PropTypes from "prop-types";

import stores from "../../../store/index";
import SelectFolderDialog from "../SelectFolderDialog/index";
import StyledComponent from "./StyledSelectFolderInput";
import SimpleFileInput from "../../SimpleFileInput";

let path = "";

class SelectFolderInputBody extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      isLoading: false,
      baseFolderPath: "",
      fullFolderPath: "",
    };
  }
  componentDidMount() {
    const { folderPath, setFirstLoad } = this.props;

    if (folderPath.length !== 0) {
      this.setState({
        fullFolderPath: folderPath,
      });
    }

    setFirstLoad(false);
  }

  componentDidUpdate(prevProps) {
    const { isSuccessSave, isReset } = this.props;
    const { fullFolderPath, baseFolderPath } = this.state;

    if (isSuccessSave && isSuccessSave !== prevProps.isSuccessSave) {
      fullFolderPath &&
        this.setState({
          baseFolderPath: fullFolderPath,
        });
    }

    if (isReset && isReset !== prevProps.isReset) {
      this.setState({
        fullFolderPath: baseFolderPath,
      });
    }
  }

  onSetFullPath = (pathName) => {
    this.setState({
      fullFolderPath: pathName,
    });
  };

  onSetBaseFolderPath = (pathName) => {
    this.setState({
      baseFolderPath: pathName,
    });
  };

  onSetLoadingInput = (loading) => {
    this.setState({
      isLoading: loading,
    });
  };
  render() {
    const {
      name,
      onClickInput,
      isPanelVisible,
      withoutProvider,
      onClose,
      isError,
      isSavingProcess,
      isDisabled,
      onSelectFolder,
      onSetLoadingData,
      foldersType,
      folderPath,
      isNeedArrowIcon,
      isSetFolderImmediately,
      id,
      selectedFolderId,
      displayType,
      dialogWithFiles,
      modalHeightContent,
      asideHeightContent,
      zIndex,
      showButtons,
      header,
      headerName,
      footer,
      selectionButtonPrimary,
      fontSizeInput,
      maxInputWidth,
      isReset,
      foldersList,
      theme,
    } = this.props;
    const { isLoading, baseFolderPath, fullFolderPath } = this.state;

    return (
      <StyledComponent maxInputWidth={maxInputWidth} theme={theme}>
        <SimpleFileInput
          theme={theme}
          name={name}
          className="select-folder_file-input"
          textField={fullFolderPath || baseFolderPath}
          isDisabled={isLoading || isSavingProcess || isDisabled}
          isError={isError}
          onClickInput={onClickInput}
          fontSizeInput={fontSizeInput}
          maxInputWidth={maxInputWidth}
        />

        <SelectFolderDialog
          theme={theme}
          zIndex={zIndex}
          isPanelVisible={isPanelVisible}
          onClose={onClose}
          folderPath={folderPath}
          onSelectFolder={onSelectFolder}
          onSetLoadingData={onSetLoadingData}
          foldersType={foldersType}
          withoutProvider={withoutProvider}
          onSetFullPath={this.onSetFullPath}
          onSetBaseFolderPath={this.onSetBaseFolderPath}
          onSetLoadingInput={this.onSetLoadingInput}
          isNeedArrowIcon={isNeedArrowIcon}
          isSetFolderImmediately={isSetFolderImmediately}
          id={id}
          selectedFolderId={selectedFolderId}
          displayType={displayType}
          dialogWithFiles={dialogWithFiles}
          modalHeightContent={modalHeightContent}
          asideHeightContent={asideHeightContent}
          showButtons={showButtons}
          header={header}
          headerName={headerName}
          footer={footer}
          selectionButtonPrimary={selectionButtonPrimary}
          isReset={isReset}
          foldersList={foldersList}
        />
      </StyledComponent>
    );
  }
}

SelectFolderInputBody.propTypes = {
  onClickInput: PropTypes.func.isRequired,
  onClose: PropTypes.func.isRequired,
  onSelectFolder: PropTypes.func.isRequired,
  onSetLoadingData: PropTypes.func,
  isPanelVisible: PropTypes.bool.isRequired,
  name: PropTypes.string,
  withoutProvider: PropTypes.bool,
  isError: PropTypes.bool,
  isSavingProcess: PropTypes.bool,
};

SelectFolderInputBody.defaultProps = {
  withoutProvider: false,
  isDisabled: false,
  isError: false,
  folderPath: "",
};

const SelectFolderInputBodyWrapper = inject(({ filesStore }) => {
  const { setFirstLoad } = filesStore;
  return {
    setFirstLoad,
  };
})(observer(SelectFolderInputBody));
class SelectFolderInput extends React.Component {
  static setFullFolderPath = (foldersArray) => {
    path = "";
    if (foldersArray.length > 1) {
      for (let item of foldersArray) {
        if (!path) {
          path = path + `${item.title}`;
        } else path = path + " " + "/" + " " + `${item.title}`;
      }
    } else {
      for (let item of foldersArray) {
        path = `${item.title}`;
      }
    }
    return path;
  };
  render() {
    return (
      <MobxProvider {...stores}>
        <SelectFolderInputBodyWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default SelectFolderInput;
