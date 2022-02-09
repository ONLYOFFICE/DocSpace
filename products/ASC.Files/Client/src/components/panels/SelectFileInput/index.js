import React from "react";
import { Provider as MobxProvider, inject, observer } from "mobx-react";
import PropTypes from "prop-types";

import stores from "../../../store/index";
import SelectFileDialog from "../SelectFileDialog";
import StyledComponent from "./StyledSelectFileInput";
import SimpleFileInput from "../../SimpleFileInput";

class SelectFileInputBody extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      fileName: "",
    };
  }

  componentDidMount() {
    this.props.setFirstLoad(false);
  }

  onSetFileName = (fileName) => {
    this.setState({
      fileName: fileName,
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
      isDisabled,
      foldersType,
      withSubfolders,
      onSelectFile,
      folderId,
      headerName,
      isImageOnly,
      isArchiveOnly,
      isDocumentsOnly,
      searchParam,
      isPresentationOnly,
      isTablesOnly,
      isMediaOnly,
      loadingLabel,
      titleFilesList,
      zIndex,
      fontSizeInput,
      maxInputWidth,
      foldersList,
    } = this.props;
    const { fileName } = this.state;

    return (
      <StyledComponent maxInputWidth={maxInputWidth}>
        <SimpleFileInput
          name={name}
          className="select-file_file-input"
          textField={fileName}
          isDisabled={isDisabled}
          isError={isError}
          onClickInput={onClickInput}
          fontSizeInput={fontSizeInput}
          maxInputWidth={maxInputWidth}
        />

        {isPanelVisible && (
          <SelectFileDialog
            zIndex={zIndex}
            onClose={onClose}
            isPanelVisible={isPanelVisible}
            foldersType={foldersType}
            onSetFileName={this.onSetFileName}
            withoutProvider={withoutProvider}
            withSubfolders={withSubfolders}
            onSelectFile={onSelectFile}
            folderId={folderId}
            headerName={headerName}
            searchParam={searchParam}
            isImageOnly={isImageOnly}
            isArchiveOnly={isArchiveOnly}
            isDocumentsOnly={isDocumentsOnly}
            isPresentation={isPresentationOnly}
            isTables={isTablesOnly}
            isMediaOnly={isMediaOnly}
            loadingLabel={loadingLabel}
            titleFilesList={titleFilesList}
            foldersList={foldersList}
          />
        )}
      </StyledComponent>
    );
  }
}

SelectFileInputBody.propTypes = {
  onClickInput: PropTypes.func.isRequired,
  onClose: PropTypes.func.isRequired,
};

SelectFileInputBody.defaultProps = {
  withoutProvider: false,
  isDisabled: false,
  zIndex: 310,
};

const SelectFileInputBodyWrapper = inject(({ filesStore }) => {
  const { setFirstLoad } = filesStore;
  return {
    setFirstLoad,
  };
})(observer(SelectFileInputBody));

class SelectFileInput extends React.Component {
  render() {
    return (
      <MobxProvider {...stores}>
        <SelectFileInputBodyWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default SelectFileInput;
