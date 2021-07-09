import React from "react";
import { Provider as MobxProvider } from "mobx-react";
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
      header,
      isImageOnly,
      isArchiveOnly,
      isDocumentsOnly,
      searchParam,
      isPresentationOnly,
      isTablesOnly,
      isMediaOnly,
      loadingLabel,
      children,
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
            withSubfolders={withSubfolders}
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
            children={children}
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
};

class SelectFileInput extends React.Component {
  render() {
    return (
      <MobxProvider {...stores}>
        <SelectFileInputBody {...this.props} />
      </MobxProvider>
    );
  }
}

export default SelectFileInput;
