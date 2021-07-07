import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import PropTypes from "prop-types";

import stores from "../../../store/index";
import SelectFileDialog from "../SelectFileDialog";
import StyledComponent from "./styledSelectFileInput";
import SimpleFileInput from "../../SimpleFileInput";

class SelectFile extends React.PureComponent {
  constructor(props) {
    super(props);
    this.inputRef = React.createRef();
    this.state = {
      fileName: "",
    };
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
      iconUrl,
      filterType,
      filterValue,
      withSubfolders,
      onSelectFile,
      folderId,
      header,
      isImageOnly,
      isArchiveOnly,
      isDocumentsOnly,
      searchParam,
      isPresentationOnly,
      isTablesOnly,
      isMediaOnly,
      loadingLabel,
    } = this.props;
    const { fileName } = this.state;
    const zIndex = 310;

    return (
      <StyledComponent>
        <SimpleFileInput
          name={name}
          className="file-input"
          textField={fileName}
          isDisabled={isDisabled}
          isError={isError}
          onClickInput={onClickInput}
        />

        {isPanelVisible && (
          <SelectFileDialog
            zIndex={zIndex}
            onClose={onClose}
            isPanelVisible={isPanelVisible}
            foldersType={foldersType}
            onSetFileName={this.onSetFileName}
            withoutProvider={withoutProvider}
            iconUrl={iconUrl}
            filterValue={filterValue}
            withSubfolders={withSubfolders}
            filterType={filterType}
            onSelectFile={onSelectFile}
            folderId={folderId}
            header={header}
            searchParam={searchParam}
            isImageOnly={isImageOnly}
            isArchiveOnly={isArchiveOnly}
            isDocumentsOnly={isDocumentsOnly}
            isPresentation={isPresentationOnly}
            isTables={isTablesOnly}
            isMediaOnly={isMediaOnly}
            loadingLabel={loadingLabel}
          />
        )}
      </StyledComponent>
    );
  }
}

SelectFile.propTypes = {
  onClickInput: PropTypes.func.isRequired,
  onClose: PropTypes.func.isRequired,
};

SelectFile.defaultProps = {
  withoutProvider: false,
  isDisabled: false,
};

class SelectFileModal extends React.Component {
  render() {
    return (
      <MobxProvider {...stores}>
        <SelectFile {...this.props} />
      </MobxProvider>
    );
  }
}

export default SelectFileModal;
