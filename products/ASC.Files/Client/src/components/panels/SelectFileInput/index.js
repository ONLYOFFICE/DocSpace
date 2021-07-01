import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import PropTypes from "prop-types";
import i18n from "./i18n";
import stores from "../../../store/index";
import FileInput from "./fileInput";
import SelectFileDialog from "../SelectFileDialog";

let path = "";

const StyledComponent = styled.div`
  .file-input {
    margin: 16px 0;
  }
  .file-input,
  .file-text-input {
    width: 100%;
    max-width: 820px;
  }
`;

class SelectFile extends React.PureComponent {
  constructor(props) {
    super(props);
    this.inputRef = React.createRef();
    this.state = {
      isLoading: false,
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
      isCommonWithoutProvider,
      onClose,
      isError,
      isSavingProcess,
      isDisabled,
      foldersType,
      iconUrl,
      filterType,
      filterValue,
      withSubfolders,
      onSelectFile,
    } = this.props;
    const { isLoading, fileName } = this.state;
    const zIndex = 310;

    return (
      <StyledComponent>
        <FileInput
          name={name}
          className="file-input"
          fileName={fileName}
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
            isCommonWithoutProvider={isCommonWithoutProvider}
            iconUrl={iconUrl}
            filterValue={filterValue}
            withSubfolders={withSubfolders}
            filterType={filterType}
            onSelectFile={onSelectFile}
          />
        )}
      </StyledComponent>
    );
  }
}

SelectFile.propTypes = {
  onClickInput: PropTypes.func.isRequired,
};

SelectFile.defaultProps = {
  isCommonWithoutProvider: false,
  isDisabled: false,
};
const SelectFileWrapper = withTranslation(["SelectedFolder", "Common"])(
  SelectFile
);

class SelectFileModal extends React.Component {
  render() {
    return (
      <MobxProvider {...stores}>
        <I18nextProvider i18n={i18n}>
          <SelectFileWrapper {...this.props} />
        </I18nextProvider>
      </MobxProvider>
    );
  }
}

export default SelectFileModal;
